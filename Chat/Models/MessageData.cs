using System;
namespace Chat.Models
{
    public class MessageData
    {
        public MessageData(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
        
    }
}
