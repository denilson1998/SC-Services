using Ardalis.EFCore.Extensions;
using Delivery.Domain.Entities;
using Delivery.Domain.Webhooks;
using Microsoft.EntityFrameworkCore;
using SharedKernel.AbstractEntities;
using SharedKernel.Extensions;
using SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IMultiTenantService _multiTenantService;

        public ApplicationDbContext(
            DbContextOptions options,
            ICurrentUserService currentUserService,
            IMultiTenantService multiTenantService
            ) : base(options)
        {
            _currentUserService = currentUserService;
            _multiTenantService = multiTenantService;
        }

        public DbSet<CourierTask> CourierTasks => Set<CourierTask>();
        public DbSet<Webhook> Webhooks => Set<Webhook>();
        public DbSet<Worker> Workers => Set<Worker>();

        // public DbSet<Address> Addresses => Set<Address>();
        // public DbSet<Route> Routes => Set<Route>();
        public DbSet<Fare> Fares => Set<Fare>();

        public DbSet<Pricing> Pricings => Set<Pricing>();

        public DbSet<User> Users => Set<User>();

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
            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
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
            SetSoftDeleteColumns();
            SetMultiTenancyFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> SaveChangesOverridingAuditableEntityAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            List<BaseEntity> entities = ChangeTracker
               .Entries()
               .Where(x => x.Entity is BaseEntity)
               .Select(x => (BaseEntity)x.Entity)
               .ToList();
            SetSoftDeleteColumns();
            SetMultiTenancyFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyAllConfigurationsFromCurrentAssembly();
            Expression<Func<ISoftDelete, bool>> deletedFilter = entity => entity.IsDeleted == false;
            Expression<Func<IMultiTenant, bool>> tenantFilter = entity => entity.OrganizationId == _multiTenantService.GetOrganizationId();
            builder.Entity<CourierTask>()
           .HasIndex(c => c.ExternalTaskId);
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    entityType.AddSoftDeleteQueryFilter(deletedFilter);
                }
                if (typeof(IMultiTenant).IsAssignableFrom(entityType.ClrType))
                {
                    entityType.AddMultiTenancyQueryFilter(tenantFilter);
                }
            }
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