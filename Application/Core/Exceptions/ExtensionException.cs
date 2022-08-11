using System.Runtime.Serialization;

namespace GeneralUnifiedTestSystemYard.Core.Exceptions;

public class ExtensionException : Exception
{
    public ExtensionException()
    {
    }

    public ExtensionException(string? message) : base(message)
    {
    }

    public ExtensionException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ExtensionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}