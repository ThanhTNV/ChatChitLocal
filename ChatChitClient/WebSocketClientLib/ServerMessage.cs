using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketClientLib
{
    public class ServerMessage
    {
        // For messages like connection confirmation or new connection notifications.
        public string Type { get; set; }

        // The client identifier, for example, from the server.
        public string ClientId { get; set; }

        // For chat messages, this holds the actual text.
        public string Data { get; set; }

        // For error messages, the error detail.
        public string Message { get; set; }
    }
}
