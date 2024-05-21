using ToDoApp.Domain.Email;

namespace ToDoApp.Domain.Interfaces;

public interface IEmailSender
{
    void SendEmail(Message message);
    Task SendEmailAsync(Message message);
}
