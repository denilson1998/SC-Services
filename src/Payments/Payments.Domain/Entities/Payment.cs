using SharedKernel;
using SharedKernel.AbstractEntities;
using SharedKernel.Constants;
using SharedKernel.Interfaces;
using System;

namespace Payments.Domain.Entities;
public abstract class Payment : AuditableEntity, IMultiTenant, ISoftDelete
{
    public int OrganizationId { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public int PaymentMethod { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletionDateTime { get; set; }
    protected Payment(){}
    protected Payment(
        int organizationId,
        decimal amount,
        Currency currency,
        PaymentMethod paymentMethod
    ) {
        OrganizationId = organizationId;
        Amount = amount;
        Currency = currency;
        PaymentMethod = (int)paymentMethod;
    }
}

public class QrPayment : Payment
{
    public string PayerName { get; set; }
    public string PayerAccountNumber { get; set; }
    public string VoucherNumber { get; set; }
    public Guid BankPayId { get; set; } = new();
    private QrPayment(){}
    public QrPayment(
        int organizationId,
        decimal amount,
        Currency currency,
        string payerName,
        string payerAccountNumber,
        string voucherNumber,
        Guid bankPayId
    ) : base(
            organizationId,
            amount,
            currency,
            SharedKernel.Constants.PaymentMethod.QR
    ) {
        PayerName = payerName;
        PayerAccountNumber = payerAccountNumber;
        VoucherNumber = voucherNumber;
        BankPayId = bankPayId;
    }
}

public class VoucherPayment : Payment
{
    public Voucher Voucher { get; set; }
    public virtual int VoucherId { get; }
    private VoucherPayment(){}

    public VoucherPayment(
        int organizationId,
        decimal amount,
        Currency currency,
        Voucher voucher
    ) : base (
        organizationId,
        amount,
        currency,
        SharedKernel.Constants.PaymentMethod.Voucher
    ) {
        Voucher = voucher;
    }
}
