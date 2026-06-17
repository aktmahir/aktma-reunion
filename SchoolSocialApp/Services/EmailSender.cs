using Microsoft.AspNetCore.Identity.UI.Services;

namespace SchoolSocialApp.Services;

public sealed class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return Task.CompletedTask;
    }
}