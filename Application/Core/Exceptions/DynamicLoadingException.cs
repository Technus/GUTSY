using System.Runtime.Serialization;

namespace GeneralUnifiedTestSystemYard.Core.Exceptions;

public class DynamicLoadingException : Exception
{
    public DynamicLoadingException()
    {
    }

    protected DynamicLoadingException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public DynamicLoadingException(string? message) : base(message)
    {
    }

    public DynamicLoadingException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}