using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Extensions
{
    public static class CurrencyEnumExtensions
    {
        public static string ToFriendlyString(this Currency currency) {

            switch (currency) {

                case Currency.Bolivianos:
                    return "Bs";
                case Currency.Dollars:
                    return "USD";
                default:
                    return currency.ToString();
            }
        }
    }
}
