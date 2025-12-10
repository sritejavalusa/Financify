using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

public static class EmailService
{
    public static async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var fromEmail = "your_email@example.com";  // your SMTP email
        var fromPassword = "your_password";        // SMTP password

        var smtp = new SmtpClient("smtp.example.com")  // replace with your SMTP server
        {
            Port = 587,
            Credentials = new NetworkCredential(fromEmail, fromPassword),
            EnableSsl = true
        };

        var message = new MailMessage(fromEmail, toEmail, subject, body);
        message.IsBodyHtml = true;

        await smtp.SendMailAsync(message);
    }
}
