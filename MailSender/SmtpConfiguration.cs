namespace MailSender;

/// <summary>
/// SmtpConfiguration for the <see cref="MailSender"/>
/// </summary>
[Serializable]
public class SmtpConfiguration
{
    public string UserName { get; init; }
    public string Password { get; init; }
    public string Host { get; init; }
    public int Port { get; init; }
    public string  Sender { get; init; }

    /// <summary>
    /// Creates a new instance of <see cref="SmtpConfiguration"/>.
    /// </summary>
    /// <param name="userName">The client's user name.</param>
    /// <param name="password">The client's password.</param>
    /// <param name="host">The SMTP host's address.</param>
    /// <param name="port">The SMTP host's port.</param>
    /// <param name="sender">The sender used in mime messages.</param>
    public SmtpConfiguration(string userName, string password, string host, int port, string sender) 
    {
        this.UserName = userName;
        this.Password = password;
        this.Host = host;
        this.Port = port;
        this.Sender = sender;
    }
}
