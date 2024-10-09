namespace FastKartProject.Services.Interfaces;

public interface ISenderEmail
{
    void SendEmailAsync(string ToEmail, string Subject, string Body, bool IsBodyHtml = false);
}
