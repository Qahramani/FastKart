using FastKartProject.Services.Interfaces;
using System.Net;
using System.Net.Mail;


namespace FastKartProject.Services.Implementations;

public class EmailSender : ISenderEmail
{

    private readonly IConfiguration _configuration;
    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public void SendEmailAsync(string ToEmail, string Subject, string Body, bool IsBodyHtml = false)
    {
       

        // Set up SMTP client
        SmtpClient client = new SmtpClient(_configuration["EmailSettings:MailServer"], int.Parse(_configuration["EmailSettings:MailPort"]));
        client.EnableSsl = true;
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(_configuration["EmailSettings:FromEmail"], _configuration["EmailSettings:Password"]);

        // Create email message
        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(_configuration["EmailSettings:FromEmail"]);
        mailMessage.To.Add(ToEmail);
        mailMessage.Subject = Subject;
        mailMessage.IsBodyHtml = true;
        
        mailMessage.Body = Body;

        // Send email
        client.Send(mailMessage);


    }
}
