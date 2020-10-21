using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Services;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace GetStat.Domain.Services
{
    public class EmailService
    {
        private readonly ModalService _modalService;

        public EmailService(ModalService modalService)
        {
            _modalService = modalService;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
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
                    await client.AuthenticateAsync("ar037@yandex.kz", "admin123");
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception e)
            {
                _modalService.ShowModalWindow("Ошибка при отправке email", e.Message);
            }
        }
    }
}
