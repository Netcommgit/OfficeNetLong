using OfficeNet.Domain.Contracts;

namespace OfficeNet.Service.EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailRequest request);
    }
}
