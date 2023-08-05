namespace ProductListCrawler;

/// <summary>
/// Represents errors that occur during parsing.
/// </summary>
public class ParseException : Exception
{
    /// Initializes a new instance of <see cref="ParseException"/> class.
    public ParseException()
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ParseException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ParseException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ParseException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">The reference to the inner exception that is the cause of this exception.</param>
    public ParseException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
