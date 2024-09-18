using System;

namespace Payments.Domain.Exceptions;

public class InvalidPaymentAmountException : ArgumentException
{
    public InvalidPaymentAmountException(string message, string paramName) : base(message, paramName)
    {
    }
}
