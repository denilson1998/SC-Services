// using Ardalis.GuardClauses;
using SharedKernel;
using SharedKernel.Constants;
using SharedKernel.Extensions;
using SharedKernel.Interfaces;
using SharedKernel.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedKernel.AbstractEntities;

namespace Payments.Domain.Entities;

public class Bill : AuditableEntity, IMultiTenant, ISoftDelete
{
    public int OrganizationId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public Currency Currency { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletionDateTime { get; set; }
    public List<QrPayment> QrPayments { get; set; } = new List<QrPayment>();
    public List<VoucherPayment> VoucherPayments { get; set; } = new List<VoucherPayment>();
    public List<Payment> Payments {
        get
        {
            if (QrPayments is null)
            {
                return new List<Payment>();
            }
            return QrPayments.ConvertAll( q => (Payment)q);
        }
    }
    public virtual List<BankQr> BankQrs { get; set; }
    public virtual decimal RemainingAmount { get {
        return TotalAmount - PaidAmount;
    }}

    public Bill() {}
    public Bill(int organizationId, decimal totalAmount, Currency currency) {
        OrganizationId = organizationId;
        TotalAmount = totalAmount;
        Currency = currency;
    }

    // public void RegisterPayment(Payment payment)
    // {
    //     // Guard.Against.InvalidPaymentAmount(_payments, payment, Amount, nameof(payment.Amount));
    //     Payments.Add(payment);
    //     TriggerEvent();
    // }

    public void RegisterPayment(QrPayment payment)
    {
        var paymentIsAlreadyRegistered = QrPayments.Any(p => p.BankPayId == payment.BankPayId);
        if (paymentIsAlreadyRegistered)
        {
            return;
        }
        // Guard.Against.InvalidPaymentAmount(_payments, payment, Amount, nameof(payment.Amount));
        QrPayments.Add(payment);
        CheckIfCompleted();
    }

    public void RegisterPayment(Voucher voucher)
    {
        if(VoucherPayments.Count > 0)
        {
            throw new Exception("Can't register more than 1 voucher per bill");
        }

        var amount = voucher.IsPercentage
            ? TotalAmount * voucher.Value
            : TotalAmount - voucher.Value;
        var newVoucherPayment = new VoucherPayment(
            OrganizationId,
            amount,
            Currency,
            voucher
        );
        VoucherPayments.Add(newVoucherPayment);
        CheckIfCompleted();
    }

    private void CheckIfCompleted()
    {
        if (IsCompleted)
        {
            return;
        }
        CompletedAt = DateTime.Now;
        var paymentsSumTotalAmount = Payments.Select(p => p.Amount).Sum();
        IsCompleted = paymentsSumTotalAmount.IsNerlyEqualThan(TotalAmount);
    }
}
