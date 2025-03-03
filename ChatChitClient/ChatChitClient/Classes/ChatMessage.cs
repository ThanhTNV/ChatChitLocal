using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ChatChitClient
{
    public class ChatMessage
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public string Time { get; set; }
        public bool IsFromMe { get; set; }

        public ChatMessage(string sender, string content, bool isFromMe)
        {
            Sender = sender;
            Content = content;
            Time = DateTime.Now.ToString("h:mm tt");
            IsFromMe = isFromMe;
        }
    }
}
