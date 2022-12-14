using Abraham.Mail;

namespace SmtpDemo
{
    class Program
    {
		static void Main()
		{
			Console.WriteLine("SMTP client - Sending an email to an SMTP postbox");

			var _client = new Abraham.Mail.SmtpClient()
				.UseHostname("ENTER YOUR SMTP SERVER NAME HERE")
				.UseSecurityProtocol(Security.Ssl)
				.UseAuthentication("ENTER YOUR USERNAME HERE", "ENTER YOUR PASSWORD HERE")
				.Open();

			var from		    = "ENTER YOUR EMAIL ADDRESS HERE";
			var to			    = "ENTER YOUR EMAIL ADDRESS HERE";
			var subject         = "Test-Email nuget package";
			var body            = "Test-Email body";

			_client.SendEmail(from, to, subject, body);
			Console.WriteLine("done");
		}
	}
}
