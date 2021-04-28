using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BanorteXXISecurity.Helpers
{
    public class Correo
    {
        public static void EnviarMensaje(string para, string asunto, string cuerpo) {
            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress("Banorte XXI Security", "recdem@xxi-banorte.com");
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress("User", para);
            message.To.Add(to);

            message.Subject = asunto;

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = cuerpo;
            //bodyBuilder.TextBody = "Hello World!";

            message.Body = bodyBuilder.ToMessageBody();

            SmtpClient client = new SmtpClient();
            client.Connect("172.20.145.14", 25, SecureSocketOptions.None);
            //client.Authenticate("user_name_here", "pwd_here");

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
        }
    }
}
