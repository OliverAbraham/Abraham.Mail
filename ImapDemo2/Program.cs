using Abraham.Mail;

namespace ImapDemo2
{
    class Program
    {
		static void Main()
		{
			Console.WriteLine("IMAP client - move an email from your inbox to a different folder");

			var _client = new Abraham.Mail.ImapClient()
				.UseHostname("ENTER YOUR IMAP SERVER NAME HERE")
				.UseSecurityProtocol(Security.Ssl)
				.UseAuthentication("ENTER YOUR USERNAME HERE", "ENTER YOUR PASSWORD HERE")
				.Open();


			Console.WriteLine("\n\n\nThese are your mailbox folders:");
			var folders = _client.GetAllFolders().ToList();
			folders.ForEach(x => Console.WriteLine($"    - {x.Name}"));


			Console.WriteLine("\n\n\nSelecting the inbox...");
			var inbox = _client.GetFolderByName(folders, "inbox");


			Console.WriteLine("\n\n\nReading the inbox...");
			var emails = _client.GetAllMessagesFromFolder(inbox);


			Console.WriteLine("\n\n\nThese are the last 5 emails:");
			var lastFiveEmails = emails.OrderByDescending(x => x.Msg.Date).Take(5).ToList();
			folders.ForEach(x => Console.WriteLine($"    - {x}"));


			Console.WriteLine("\n\n\nReading only the unread messages...");
			emails = _client.GetUnreadMessagesFromFolder(inbox);


			Console.WriteLine("\n\n\nThese are the last 5 unread emails:");
			lastFiveEmails = emails.OrderByDescending(x => x.Msg.Date).Take(5).ToList();
			lastFiveEmails.ForEach(x => Console.WriteLine($"    - {x}"));
 			_client.Close();
		}
	}
}
