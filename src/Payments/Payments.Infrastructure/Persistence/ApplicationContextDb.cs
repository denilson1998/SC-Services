using Ardalis.EFCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Payments.Domain;
using Payments.Domain.Entities;
using SharedKernel;
using SharedKernel.AbstractEntities;
using SharedKernel.Constants;
using SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Payments.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IMultiTenantService _multiTenantService;
        // private readonly IEventDispatcher _eventDispatcher;

        public ApplicationDbContext(
            DbContextOptions options,
            ICurrentUserService currentUserService,
            IMultiTenantService multiTenantService
            // IEventDispatcher eventDispatcher
        ) : base(options)
        {
            _currentUserService = currentUserService;
            _multiTenantService = multiTenantService;
            // _eventDispatcher = eventDispatcher;
        }

        public DbSet<Voucher> Vouchers => Set<Voucher>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<BankQr> BankQrs => Set<BankQr>();
        public DbSet<Bill> Bills => Set<Bill>();
        public DbSet<QrPayment> QrPayments => Set<QrPayment>();
        public DbSet<VoucherPayment> VoucherPayments => Set<VoucherPayment>();
        public DbSet<Worker> Workers => Set<Worker>();

        private void SetMultiTenancyFields()
        {
            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<IMultiTenant> entry in
                     ChangeTracker
                         .Entries<IMultiTenant>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.OrganizationId = _multiTenantService.GetOrganizationId();
                }
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditableEntity> entry in ChangeTracker
                         .Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        entry.Entity.CreatedAt = DateTime.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        entry.Entity.LastModified = DateTime.Now;
                        break;
                }
            }

            List<BaseEntity> entities = ChangeTracker
                .Entries()
                .Where(x => x.Entity is BaseEntity)
                .Select(x => (BaseEntity)x.Entity)
                .ToList();
            SetSoftDeleteColumns();
            SetMultiTenancyFields();
            var result = await base.SaveChangesAsync(cancellationToken);
            // foreach (BaseEntity entity in entities)
            // {
            //     await _eventDispatcher.DispatchAsync(entity.DomainEvents);
            //     entity.ClearDomainEvents();
            // }

            return result;
        }

        public async Task<int> SaveChangesOverridingAuditableEntityAsync(
            CancellationToken cancellationToken = new CancellationToken())
        {
            SetMultiTenancyFields();
            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>()
                .HavePrecision(DecimalPrecisionScale.Precision, DecimalPrecisionScale.Scale);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyAllConfigurationsFromCurrentAssembly();
            Expression<Func<ISoftDelete, bool>> deletedFilter = entity => entity.IsDeleted == false;
            Expression<Func<IMultiTenant, bool>> tenantFilter = entity =>
                entity.OrganizationId == _multiTenantService.GetOrganizationId();
            builder.Entity<Payment>()
                .HasDiscriminator<int>("PaymentMethod")
                .HasValue<QrPayment>((int)PaymentMethod.QR)
                .HasValue<VoucherPayment>((int)PaymentMethod.Voucher);

            builder.Entity<QrPayment>(b =>
            {
                b.HasBaseType<Payment>();
                b.Property(prop => prop.PayerName).HasColumnName("PayerName");
                b.Property(prop => prop.VoucherNumber).HasColumnName("VoucherNumber");
                b.Property(prop => prop.PayerAccountNumber).HasColumnName("PayerAccountNumber");
                b.Property(prop => prop.BankPayId).HasColumnName("BankPayId");
            });

            builder.Entity<VoucherPayment>(b =>
            {
                b.HasBaseType<Payment>();
            });

            builder.AddQueryFilters(deletedFilter, tenantFilter);
            builder.Entity<Bill>(entity => entity.HasIndex(b => b.OrganizationId));
        }

        private void SetSoftDeleteColumns()
        {
            var entriesDeleted = ChangeTracker
                .Entries()
                .Where(e => e.Entity is ISoftDelete
                            && e.State == EntityState.Deleted);

            foreach (var entityEntry in entriesDeleted)
            {
                ((ISoftDelete)entityEntry.Entity).IsDeleted = true;
                ((ISoftDelete)entityEntry.Entity).DeletionDateTime = DateTime.Now;
                entityEntry.State = EntityState.Modified;
            }
        }
    }
}