﻿using System.Runtime.Serialization;

namespace GeneralUnifiedTestSystemYard.Exceptions;

public class CommandFailedException : Exception
{
    public CommandFailedException()
    {
    }

    public CommandFailedException(string? message) : base(message)
    {
    }

    public CommandFailedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected CommandFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}