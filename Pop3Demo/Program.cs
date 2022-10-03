using Abraham.Mail;

namespace Pop3Demo
{
    class Program
    {
		static void Main()
		{
			Console.WriteLine("POP3 client - read all emails from your inbox");

			var _client = new Abraham.Mail.Pop3Client()
				.UseHostname("ENTER YOUR POP3 SERVER NAME HERE")
				.UseSecurityProtocol(Security.Ssl)
				.UseAuthentication("ENTER YOUR USERNAME HERE", "ENTER YOUR PASSWORD HERE")
				.Open();

			Console.Write("Reading the inbox...");
			var emails = _client.GetAllMessages();
			emails.ForEach(x => Console.WriteLine($"    - {x}"));
		}
	}
}
