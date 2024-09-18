using SharedKernel;
using SharedKernel.AbstractEntities;
using SharedKernel.Constants;
using SharedKernel.Interfaces;
using System;

namespace Payments.Domain.Entities;
public class Voucher : AuditableEntity, IMultiTenant, ISoftDelete
{
    public int OrganizationId { get; set; }
    public decimal Value { get; protected set; }
    public Currency Currency { get; protected set; }
    public bool IsPercentage { get; protected set; }
    public int Quantity { get; protected set; }
    public int QuantityUsed { get; protected set; }
    public DateTime? ValidSince { get; protected set; }
    public DateTime? ValidUntil { get; protected set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletionDateTime { get; set; }

    public Voucher() {}
    public Voucher(
        int organizationId,
        decimal value,
        Currency currency,
        bool isPercentage,
        int quantity
    ) {
        if (isPercentage)
        {
            if (value > (decimal)1.0 || value  < (decimal)0.0)
            {
                throw new Exception("Value must be between 0.00 and 1.00");
            }
        }
        Value = value;
        OrganizationId = organizationId;
        Currency = currency;
        IsPercentage = isPercentage;
        Quantity = quantity;
        QuantityUsed = 0;
    }

    public void SetValidDates(
        DateTime? validSince,
        DateTime? validUntil
    ) {
        if (validSince > ValidUntil)
        {
            throw new Exception("Valid since date must be before valid until date");
        }
        ValidSince = validSince;
        ValidUntil = validUntil;
    }

    public void RegisterVoucherUse()
    {
        if (Quantity <= QuantityUsed)
        {
            throw new Exception("Voucher has reached maximum availability");
        }

        QuantityUsed++;
    }
}
