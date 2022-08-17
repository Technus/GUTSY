using System.Runtime.Serialization;

namespace GeneralUnifiedTestSystemYard.Exceptions;

public class RequestUndefinedException : Exception
{
    public RequestUndefinedException()
    {
    }

    public RequestUndefinedException(string? message) : base(message)
    {
    }

    public RequestUndefinedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected RequestUndefinedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}