using MailKit;
using MailKit.Security;

namespace Abraham.Mail
{
	public class Pop3Client
    {
		#region ------------- Properties ----------------------------------------------------------
		public string	Hostname         { get; set; }
	    public int		Port 	         { get; set; } = 995;
	    public Security	SecurityProtocol { get; set; } = Security.Ssl;
	    public string	Username         { get; set; }
	    public string	Password         { get; set; }
		#endregion



		#region ------------- Fields --------------------------------------------------------------
		private MailKit.Net.Pop3.Pop3Client _client;
		#endregion



		#region ------------- Init ----------------------------------------------------------------
		public Pop3Client()
		{
		}
		#endregion



		#region ------------- Methods -------------------------------------------------------------
		public Pop3Client UseHostname(string hostname)
		{
			Hostname = hostname;
			return this;
		}

		public Pop3Client UsePort(int port)
		{
			Port = port;
			return this;
		}

		public Pop3Client UseSecurityProtocol(Security securityProtocol)
		{
			SecurityProtocol = securityProtocol;
			return this;
		}

		public Pop3Client UseAuthentication(string username, string password)
		{
			Username = username;
			Password = password;
			return this;
		}
		
		public Pop3Client RegisterCodepageProvider()
		{
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
			return this;
		}

		public Pop3Client Open()
		{
            _client = new MailKit.Net.Pop3.Pop3Client();

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

		public Pop3Client Close()
		{
			_client.Disconnect (true);
			return this;
		}

		public List<Message> GetAllMessages()
        {
			return GetMessages(0, _client.Count);
		}

		public List<Message> GetMessages(int startIndex, int count)
        {
			var results = new List<Message>();

			var messages = _client.GetMessages(startIndex, count);
			foreach(var message in messages)
				results.Add(new Message(new UniqueId(), message));

			return results;
        }
		#endregion
	}
}
