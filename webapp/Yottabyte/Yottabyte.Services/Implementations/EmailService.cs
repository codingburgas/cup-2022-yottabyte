using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using Yottabyte.Services.Contracts;
using static Yottabyte.Services.Contracts.IEmailService;

namespace Yottabyte.Services.Implementations;

/// <summary>
/// Email service.
/// </summary>
internal class EmailService : IEmailService
{
    private readonly IConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailService"/> class.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    public EmailService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    /// <inheritdoc/>
    public async Task SendEmailAsync(SendEmailRequest emailRequest)
    {
        var client = new SendGridClient(this.configuration["SendGrid:APIKey"]);
        var sendGridMessage = new SendGridMessage()
        {
            From = new EmailAddress(this.configuration["SendGrid:Email"], this.configuration["SendGrid:Name"]),
            Subject = emailRequest.Subject,
            PlainTextContent = emailRequest.Message,
            HtmlContent = emailRequest.Message,
        };
        sendGridMessage.AddTo(new EmailAddress(emailRequest.Email));
        await client.SendEmailAsync(sendGridMessage);
    }
}