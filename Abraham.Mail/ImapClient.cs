using MailKit;
using MailKit.Search;
using MailKit.Security;

namespace Abraham.Mail;

/// <summary>
/// Receive and send emails easily using POP3, SMTP and IMAP
/// 
/// Author:
/// Oliver Abraham, mail@oliver-abraham.de, https://www.oliver-abraham.de
/// 
/// Source code hosted at: 
/// https://github.com/OliverAbraham/Abraham.Mail
/// 
/// Nuget Package hosted at: 
/// https://www.nuget.org/packages/Abraham.Mail/
/// 
/// </summary>
/// 
public class ImapClient
{
	#region ------------- Properties ----------------------------------------------------------
	public string	Hostname         { get; set; }
    public int		Port 	         { get; set; } = 993;
    public Security	SecurityProtocol { get; set; } = Security.Ssl;
    public string	Username         { get; set; }
    public string	Password         { get; set; }
	public Action<string> Logger     { get; set; }
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

	public ImapClient UseLogger(Action<string> logger)
	{
		Logger = logger;
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

    public List<Message> ReadAllEmailsFromInbox()
    {
        return ReadEmailsFromFolder("inbox", unreadOnly: false);
    }

    public List<Message> ReadUnreadEmailsFromInbox()
    {
        return ReadEmailsFromFolder("inbox", unreadOnly: true);
    }

    public List<Message> ReadEmailsFromFolder(string folderName, bool unreadOnly = false)
    {
        List<IMailFolder> folders = null;
        try
        {
            folders = GetAllFolders()?.ToList();
            if (folders is null)
                throw new Exception();
        }
        catch (Exception ex) 
        {
            Logger($"Error opening the connection to postbox {Hostname}. More Info: {ex}");
            throw;
        }

        IMailFolder folderToReadFrom = null;
        try
        {
            folderToReadFrom = GetFolderByName(folders, folderName);
            if (folderToReadFrom is null)
                throw new Exception();
        }
        catch (Exception)
        {
            var allImapFolders = string.Join(',', folders.Select(x => x.Name));
            Logger($"Error getting the folder named '{folderName}' from your imap server. Existing folders are: {allImapFolders}");
            throw;
        }

        Logger($"Checking email account {Hostname}");

        try
        {
            var emails = unreadOnly
                ? GetUnreadMessagesFromFolder(folderToReadFrom).ToList()
                : GetAllMessagesFromFolder(folderToReadFrom).ToList();
            return emails;
        }
        catch (Exception ex)
        {
            Logger($"Emails cannot be read for postbox {Hostname}. More info: {ex}");
            throw;
        }
        finally
        {
            if (_client is not null)
                Close();
        }
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

	public void CopyEmailToFolder(Message message, IMailFolder source, IMailFolder destination)
	{
		source.Open(FolderAccess.ReadWrite);
		source.CopyTo(message.UID, destination);
		source.Close();
	}

	public void MarkAsRead(Message message, IMailFolder folder)
	{
		folder.Open(FolderAccess.ReadWrite);
		folder.AddFlags(message.UID, MessageFlags.Seen, true);
		folder.Close();
	}

	public void MarkAsUnread(Message message, IMailFolder folder)
	{
		folder.Open(FolderAccess.ReadWrite);
		folder.RemoveFlags(message.UID, MessageFlags.Seen, true);
		folder.Close();
	}
	#endregion
}
