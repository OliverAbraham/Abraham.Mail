using MailKit;
using MailKit.Search;
using MailKit.Security;

namespace Abraham.Mail
{
	public class ImapClient
	{
		#region ------------- Properties ----------------------------------------------------------
		public string	Hostname         { get; set; }
	    public int		Port 	         { get; set; } = 993;
	    public Security	SecurityProtocol { get; set; } = Security.Ssl;
	    public string	Username         { get; set; }
	    public string	Password         { get; set; }
		#endregion



		#region ------------- Fields --------------------------------------------------------------
		private MailKit.Net.Imap.ImapClient _client;
		#endregion



		#region ------------- Init ----------------------------------------------------------------
		public ImapClient()
		{
		}
		#endregion



		#region ------------- Methods -------------------------------------------------------------
		public ImapClient UseHostname(string hostname)
		{
			Hostname = hostname;
			return this;
		}

		public ImapClient UsePort(int port)
		{
			Port = port;
			return this;
		}

		public ImapClient UseSecurityProtocol(Security securityProtocol)
		{
			SecurityProtocol = securityProtocol;
			return this;
		}

		public ImapClient UseAuthentication(string username, string password)
		{
			Username = username;
			Password = password;
			return this;
		}
		
		public ImapClient RegisterCodepageProvider()
		{
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
			return this;
		}

		public ImapClient Open()
		{
            _client = new MailKit.Net.Imap.ImapClient();

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

		public ImapClient Close()
		{
			_client.Disconnect (true);
			return this;
		}

		public IMailFolder GetFolderByName(string name, bool caseInsensitive = true)
		{
			var allFolders = GetAllFolders();
			return GetFolderByName(allFolders, name);
		}

		public IMailFolder GetFolderByName(IEnumerable<IMailFolder> folders, string name, bool caseInsensitive = true)
		{
			if (caseInsensitive)
				return folders.Where(x => x.Name.ToUpper() == name.ToUpper()).First();
			else
				return folders.Where(x => x.Name == name).First();
		}

		public IEnumerable<IMailFolder> GetAllFolders()
        {
			var personal = _client.GetFolder(_client.PersonalNamespaces[0]);
			foreach (var folder in personal.GetSubfolders (false))
				yield return folder;
        }

		public IEnumerable<Message> GetUnreadMessagesFromFolder(IMailFolder folder)
		{
			return GetMessagesFromFolder(folder, SearchQuery.NotSeen);
		}

		public IEnumerable<Message> GetAllMessagesFromFolder(IMailFolder folder)
        {
			return GetMessagesFromFolder(folder, SearchQuery.All);
        }

		public IEnumerable<Message> GetMessagesFromFolder(IMailFolder folder, SearchQuery searchQuery)
        {
			folder.Open(FolderAccess.ReadOnly);

			var uniqueIDs = folder.Search (searchQuery);
			foreach (var uniqueID in uniqueIDs) 
				yield return new Message(uniqueID, folder.GetMessage (uniqueID));

			folder.Close();
        }

		public void MoveEmailToFolder(Message message, IMailFolder source, IMailFolder destination)
		{
			source.Open(FolderAccess.ReadWrite);
			source.MoveTo(message.UID, destination);
			source.Close();
		}
		#endregion



		#region ------------- Implementation ------------------------------------------------------
		#endregion
	}
}
