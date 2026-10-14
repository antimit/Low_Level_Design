namespace NotificationSystem;

using System.Collections.Generic;


interface INotificationService
{
    void sendMessage(string message, string recipient);
    bool validateRecipient(string recipient);
    NotificationStatus getStatus();
};


enum NotificationStatus
{
    Success,
    Fail
};
class EmailService : INotificationService
{
    public void sendMessage(string message, string recipient)
    {
        string notificatin = "The message : " + message + " was sent to the recipient : " + recipient;
        System.Console.WriteLine(notificatin);
    }

    public bool validateRecipient(string recipient)
    {
        return true;
    }

    public NotificationStatus getStatus()
    {
        return NotificationStatus.Success;
    }
};


class SMSService : INotificationService
{
    public void sendMessage(string message, string recipient)
    {
        string notificatin = "The message : " + message + " was sent to the recipient : " + recipient;
        System.Console.WriteLine(notificatin);
    }

    public bool validateRecipient(string recipient)
    {
        return true;
    }

    public NotificationStatus getStatus()
    {
        return NotificationStatus.Success;
    }
};


class NotificationManager
{
    private List<INotificationService> services;

    public NotificationManager()
    {
        services = new List<INotificationService>();
        services.Add(new EmailService());
        services.Add(new SMSService());
    }


    public void notifyAll(string message, string recipient)
    {
        services.ForEach(service =>
        {
            if (service.validateRecipient(recipient))
            {
                service.sendMessage(message, recipient);
            }
        });
    }
};