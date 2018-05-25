using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;


namespace QualityCapsIrina.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailConfirmationAsync(string email, string callbackUrl)
        {
            var message = $"<h3>Welcome to Quality Caps<h3>" +
                $"<p>To complete your registration, please open the following link</p>" +
                $"<a target='_blank' rel='noopener noreferrer' href='{callbackUrl}'>Confirm email</a>";
            return SendEmailAsync(email, "Welcome to Quality Caps", message);
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("support@qc.co.nz", "Quality Caps"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            return client.SendEmailAsync(msg);
        }
    }
}
