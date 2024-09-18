using AutoMapper;
using Payments.Api.Endpoints.Bills;
using Payments.Api.Endpoints.Vouchers;
using Payments.Domain.Entities;
using SharedKernel.Contracts.Payments;

namespace Payments.Api
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            // CreateMap<Payment, CreatePaymentResult>();
            // CreateMap<Payment, ListPaymentResult>();
            // CreateMap<Payment, GetPaymentResult>();
            // CreateMap<Payment, PayQrResult>();
            // CreateMap<Bill, GetBillResult>();
            CreateMap<Bill, CreateBillResult>();
            CreateMap<Bill, GetBillResult>();
            CreateMap<Bill, ListBillsResult>();
            CreateMap<Bill, RegisterBillPaymentResult>();
            CreateMap<Payment, GetBillPaymentResult>();
            CreateMap<BankQr, CreateBillBankQrResult>();
            CreateMap<BankQr, GetBillBankQrResult>();
            CreateMap<Voucher, CreateVoucherResult>();
            // CreateMap<BankQr, ListBankQrResult>();
            // CreateMap<BankQr, PayQrResultDetail>();
        }
    }
}