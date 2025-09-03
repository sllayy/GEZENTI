using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.Configuration;

namespace GeziRotasi.Services;

public class MailKitEmailSender : IEmailSender
{
    private readonly IConfiguration _cfg;
    public MailKitEmailSender(IConfiguration cfg) => _cfg = cfg;

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
    {
        // appsettings.json: "Email": { ... } bölümünden oku
        var host = _cfg["Email:SmtpHost"]!;                 // "smtp.gmail.com"
        var portStr = _cfg["Email:SmtpPort"];
        var user = _cfg["Email:Username"] ?? _cfg["Email:FromAddress"];
        var pass = _cfg["Email:Password"];                  // Gmail için App Password (boşluksuz)
        var from = _cfg["Email:FromEmail"] ?? _cfg["Email:FromAddress"] ?? user;
        var fromName = _cfg["Email:FromName"] ?? "loginpage";
        var useStartTls = bool.TryParse(_cfg["Email:UseStartTls"], out var b) ? b : true;

        var port = 587;
        if (!string.IsNullOrWhiteSpace(portStr) && int.TryParse(portStr, out var p)) port = p;

        var msg = new MimeMessage();
        msg.From.Add(new MailboxAddress(fromName, from));
        msg.To.Add(MailboxAddress.Parse(toEmail));
        msg.Subject = subject;
        msg.Body = new TextPart(TextFormat.Html) { Text = htmlBody };

        using var client = new SmtpClient();

        var secure = useStartTls && port != 465
            ? SecureSocketOptions.StartTls
            : SecureSocketOptions.SslOnConnect;

        await client.ConnectAsync(host, port, secure, ct);

        // Parola ile giriş yapacaksak XOAUTH2'yi devre dışı bırak
        client.AuthenticationMechanisms.Remove("XOAUTH2");

        if (!string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(pass))
            await client.AuthenticateAsync(user, pass, ct);

        await client.SendAsync(msg, ct);
        await client.DisconnectAsync(true, ct);
    }
}
