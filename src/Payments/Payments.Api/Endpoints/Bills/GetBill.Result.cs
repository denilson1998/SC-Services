using System.Collections.Generic;
using System;
using SharedKernel.Constants;

namespace Payments.Api.Endpoints.Bills
{
    public class GetBillResult
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsCompleted { get; set; }
        public List<GetBillPaymentResult> Payments { get; set; }
        public List<GetBillBankQrResult> BankQrs { get; set; }
    }

    public class GetBillPaymentResult
    {
        public int OrganizationId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public int PaymentMethod { get; set; }
        public Guid BankPayId { get; set; } = new();
    }

    public class GetBillBankQrResult
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
    }
}
