using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;

namespace Abraham.Mail
{
    public interface IImapClient
    {
		public IImapClient UseHostname(string hostname);
		public IImapClient UsePort(int port);
		public IImapClient UseSecurityProtocol(Security securityProtocol);
		public IImapClient UseAuthentication(string username, string password);
		public IImapClient UseLogger(Action<string> logger);
		public IImapClient RegisterCodepageProvider();
		public IImapClient Open();
		public IImapClient Close();
		public List<Message> ReadAllEmailsFromInbox();
		public List<Message> ReadUnreadEmailsFromInbox();
		public List<Message> ReadEmailsFromFolder(string folderName, bool unreadOnly = false, bool closeConnectionAfterwards = true);
		public IMailFolder GetFolderByName(string name, bool caseInsensitive = true);
		public IMailFolder GetFolderByName(IEnumerable<IMailFolder> folders, string name, bool caseInsensitive = true);
		public IEnumerable<IMailFolder> GetAllFolders();
		public IEnumerable<Message> GetUnreadMessagesFromFolder(IMailFolder folder);
		public IEnumerable<Message> GetAllMessagesFromFolder(IMailFolder folder);
		public IEnumerable<Message> GetMessagesFromFolder(IMailFolder folder, SearchQuery searchQuery);
		public void MoveEmailToFolder(Message message, IMailFolder source, IMailFolder destination);
		public void CopyEmailToFolder(Message message, IMailFolder source, IMailFolder destination);
		public void MarkAsRead(Message message, IMailFolder folder);
		public void MarkAsUnread(Message message, IMailFolder folder);
		public IMailFolder CreateFolder(string displayName);
		public void DeleteFolder(string folderName);
		public void DeleteFolder(ImapFolder folder);
	}
}
