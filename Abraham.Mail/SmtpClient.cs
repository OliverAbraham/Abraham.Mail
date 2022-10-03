using MailKit.Security;
using MimeKit;

namespace Abraham.Mail
{
	public class SmtpClient
    {
		#region ------------- Properties ----------------------------------------------------------
		public string	Hostname         { get; set; }
	    public int		Port 	         { get; set; } = 465;
	    public Security	SecurityProtocol { get; set; } = Security.Ssl;
	    public string	Username         { get; set; }
	    public string	Password         { get; set; }
		#endregion



		#region ------------- Fields --------------------------------------------------------------
		private MailKit.Net.Smtp.SmtpClient _client;
		#endregion



		#region ------------- Init ----------------------------------------------------------------
		public SmtpClient()
		{
		}
		#endregion



		#region ------------- Methods -------------------------------------------------------------
		public SmtpClient UseHostname(string hostname)
		{
			Hostname = hostname;
			return this;
		}

		public SmtpClient UsePort(int port)
		{
			Port = port;
			return this;
		}

		public SmtpClient UseSecurityProtocol(Security securityProtocol)
		{
			SecurityProtocol = securityProtocol;
			return this;
		}

		public SmtpClient UseAuthentication(string username, string password)
		{
			Username = username;
			Password = password;
			return this;
		}
		
		public SmtpClient RegisterCodepageProvider()
		{
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
			return this;
		}

		public SmtpClient Open()
		{
            _client = new MailKit.Net.Smtp.SmtpClient();

			var options = SecurityProtocol switch
			{
				Security.None                  => SecureSocketOptions.None,
				Security.Ssl                   => SecureSocketOptions.SslOnConnect,
				Security.StartTls              => SecureSocketOptions.StartTls,
				Security.StartTlsWhenAvailable => SecureSocketOptions.StartTlsWhenAvailable,
			};

			_client.Connect (Hostname, Port, options);
			_client.Authenticate (Username, Password);
			return this;
		}

		public SmtpClient Close()
		{
			_client.Disconnect (true);
			return this;
		}

		public void SendEmail(string from, string to, string subject, string body, List<MimeEntity> attachments = default(List<MimeEntity>))
        {
			var message         = new MimeKit.MimeMessage();
			message.From		.Add(new MimeKit.MailboxAddress(from, from));
			message.To			.Add(new MimeKit.MailboxAddress(to, to));
			message.Subject     = subject;
			
			var builder			= new MimeKit.BodyBuilder();
			builder.TextBody    = body;
			if (attachments is not null)
				attachments.ForEach(x => builder.Attachments.Add(x));
			
			message.Body        = builder.ToMessageBody();
			SendEmail(message);
		}

		public void SendEmail(MimeMessage message)
        {
			_client.Send(message);
        }
		#endregion
	}
}
