using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Downloader;

/// <summary>
/// Represents error that occur during the network communication
/// </summary>
public class NetworkException : Exception
{
    /// Initializes a new instance of <see cref="NetworkException"/> class.
    public NetworkException()
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NetworkException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NetworkException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NetworkException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">The reference to the inner exception that is the cause of this exception.</param>
    public NetworkException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
