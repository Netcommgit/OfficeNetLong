using OfficeNet.Domain.Contracts;
using System.Net.Mail;
using System.Net;
using OfficeNet.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace OfficeNet.Service.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public async Task SendEmailAsync(EmailRequest request)
        {
           var result =  _context.SmtpDetails.FirstOrDefaultAsync(s => s.SmtpId == 1);
            var smtpHost = _config["Email:SmtpHost"];
            var smtpPort = int.Parse(_config["Email:SmtpPort"]);
            var smtpUser = _config["Email:SmtpUser"];
            var smtpPass = _config["Email:SmtpPass"];
            var fromEmail = _config["Email:From"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = request.Subject,
                Body = request.Body,
                IsBodyHtml = request.IsHtml
            };

            request.To?.ForEach(email => message.To.Add(email));
            request.Cc?.ForEach(email => message.CC.Add(email));
            request.Bcc?.ForEach(email => message.Bcc.Add(email));

            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    message.Headers.Add(header.Key, header.Value);
                }
            }

            await client.SendMailAsync(message);
        }
    }
}
