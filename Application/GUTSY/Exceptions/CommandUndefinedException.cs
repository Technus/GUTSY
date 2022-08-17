using System.Runtime.Serialization;

namespace GeneralUnifiedTestSystemYard.Exceptions;

public class CommandUndefinedException : Exception
{
    public CommandUndefinedException()
    {
    }

    public CommandUndefinedException(string? message) : base(message)
    {
    }

    public CommandUndefinedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected CommandUndefinedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}