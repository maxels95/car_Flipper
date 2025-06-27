// Services/MailService.cs
using CarFlipper.API.Models;
using MailKit.Net.Smtp;
using MimeKit;

public interface IMailService
{
    Task SendUndervaluedAdAlertAsync(Ad ad);
}

public class MailService : IMailService
{
    private readonly IConfiguration _config;

    public MailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendUndervaluedAdAlertAsync(Ad ad)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config["Mail:From"]));
        email.To.Add(MailboxAddress.Parse(_config["Mail:To"]));
        email.Subject = $"🔔 Undervärderad bil upptäckt: {ad.Title}";
        email.Body = new TextPart("plain")
        {
            Text = $"🚗 {ad.Title}\nPris: {ad.Price} kr\nURL: {ad.Url}"
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_config["Mail:Smtp"], 587, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_config["Mail:Username"], _config["Mail:Password"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
