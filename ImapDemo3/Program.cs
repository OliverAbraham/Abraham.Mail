using Abraham.Mail;

namespace ImapDemo3
{
    internal class Program
    {
        static void Main(string[] args)
        {
			Console.WriteLine("IMAP client - read all unread emails from your inbox");

			var _client = new Abraham.Mail.ImapClient()
				.UseHostname("ENTER YOUR IMAP SERVER NAME HERE")
				.UseSecurityProtocol(Security.Ssl)
				.UseAuthentication("ENTER YOUR USERNAME HERE", "ENTER YOUR PASSWORD HERE")
				.Open();

			Console.Write("Reading the inbox...");
			var emails = _client.ReadUnreadEmailsFromInbox();
			emails.ForEach(x => Console.WriteLine($"    - {x}"));
        }
    }
}
