using System;

namespace Routes.Service.Exceptions;

public class BusinessRuleException : Exception
{
    public BusinessRuleException()
        : base("Ocurreu um erro de regra de negócio!")
    {
    }

    public BusinessRuleException(string message)
        : base(message)
    {
    }

    public BusinessRuleException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}