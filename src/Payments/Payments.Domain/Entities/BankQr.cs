using System;
using System.Collections.Generic;
using System.Linq;
using SharedKernel;
using SharedKernel.AbstractEntities;
using SharedKernel.Constants;
using SharedKernel.Interfaces;

namespace Payments.Domain.Entities;
public class BankQr : AuditableEntity, IMultiTenant, ISoftDelete
{
    public int ClientId { get; set; }
    public string QrId { get; set; }
    public string ClientName { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public int BankAccountNumber { get; set; }
    public Currency Currency { get; set; }
    public BankAccountType BankAccountType { get; set; }
    public bool IsPaid { get; set; }
    public string EncryptedQrString { get; set; }
    public DateTime ExpirationDate { get; set; }
    public int OrganizationId { get; set; }
    public int PaymentId { get; set; }
    public int? BillId { get; set; }
    public virtual Bill Bill { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletionDateTime { get; set; }
    public List<QrPayment> QrPayments { get; set; } = new List<QrPayment>();
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

    public void RegisterPayment(QrPayment payment)
    {
        var paymentIsAlreadyRegistered = QrPayments.Any(p => p.BankPayId == payment.BankPayId);
        if (paymentIsAlreadyRegistered)
        {
            return;
        }
        // Guard.Against.InvalidPaymentAmount(_payments, payment, Amount, nameof(payment.Amount));
        QrPayments.Add(payment);
        // TriggerEvent();
    }

    // private void TriggerEvent()
    // {
    //     var currentValueIsCompleted = IsCompleted;
    //     var paymentsSumTotalAmount = Payments.Select(p => p.Amount).Sum();
    //     var newValueIsCompleted = paymentsSumTotalAmount.IsNerlyEqualThan(TotalAmount);
    //     if (currentValueIsCompleted != newValueIsCompleted)
    //     {
    //         IsCompleted = newValueIsCompleted;
    //         RaiseDomainEvent(new BillPaidEvent(OrganizationId, Id, IsCompleted));
    //     }
    // }
}
