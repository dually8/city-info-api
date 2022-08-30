namespace CityInfo.API.Services;

public class CloudMailService : IMailService
{
    private readonly string _mailTo;
    private readonly string _mailFrom;

    public CloudMailService(IConfiguration config)
    {
        _mailTo = config["MailSettings:MailToAddress"];
        _mailFrom = config["MailSettings:MailFromAddress"];
    }

    public void Send(string subject, string message)
    {
        // send mail - output to console
        Console.WriteLine($"Mail from {_mailFrom} to {_mailTo} with {nameof(CloudMailService)}.");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {message}");
    }
}