using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace GetStat.Domain.Services
{
    public class EmailService
    {
        public async Task<bool> SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress("Getknowledge", "ar037@yandex.kz"));
                emailMessage.To.Add(new MailboxAddress("", email));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart(TextFormat.Html)
                {
                    Text = message
                };

                using var client = new SmtpClient();
                {
                    await client.ConnectAsync("smtp.yandex.kz", 465, true);
                    await client.AuthenticateAsync("testUser", "testPass");
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}