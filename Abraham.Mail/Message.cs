using MailKit;
using MimeKit;

namespace Abraham.Mail
{
    public class Message
	{
		public UniqueId UID { get; set; }
		public MimeMessage Msg { get; set; }

		public Message(UniqueId uniqueId, MimeMessage mimeMessage)
		{
			UID = uniqueId;
			Msg = mimeMessage;
		}

		public override string ToString()
		{
			return $"{Msg.Date,-35} {Msg.From,-60} {Msg.Subject}";
		}
	}
}
