using Microsoft.AspNetCore.Http;

namespace ToDoApp.Domain.Email;

public class Message
{
    public IEnumerable<string> To { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }

    public IFormFileCollection? Attachments { get; set; }

    public Message(IEnumerable<string> to, string subject, string content, IFormFileCollection? attachments)
    {
        To = to;
        Subject = subject;
        Content = content;
        Attachments = attachments;
    }
}
