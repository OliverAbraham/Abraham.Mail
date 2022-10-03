using Abraham.Mail;

namespace ImapDemo1
{
	class Program
	{
		static void Main()
		{
			Console.WriteLine("IMAP client - read all unread emails from your inbox");

			var _client = new Abraham.Mail.ImapClient()
				.UseHostname("ENTER YOUR IMAP SERVER NAME HERE")
				.UseSecurityProtocol(Security.Ssl)
				.UseAuthentication("ENTER YOUR USERNAME HERE", "ENTER YOUR PASSWORD HERE")
				.Open();


			Console.WriteLine("\n\n\nSelecting the inbox...");
			var inbox = _client.GetFolderByName("inbox");


			Console.Write("Reading the inbox...");
			var emails = _client.GetUnreadMessagesFromFolder(inbox).ToList();
			emails.ForEach(x => Console.WriteLine($"    - {x}"));
		}
	}
}
