using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebSocketClientLib
{
    public class WebSocketClientHandler : IDisposable
    {
        private ClientWebSocket _clientWebSocket;

        // Events for external subscribers
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<string> OnMessageReceived;
        public event Action<string> OnError;
        private string _id;
        public bool isConnected = false;
        public string Id()
        {
            return _id;
        }

        /// <summary>
        /// Connects to the specified WebSocket URI.
        /// </summary>
        public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            _clientWebSocket = new ClientWebSocket();
            try
            {
                await _clientWebSocket.ConnectAsync(uri, cancellationToken);
                isConnected = true;
                OnConnected?.Invoke();
                // Start the receive loop
                _ = Task.Run(() => ReceiveLoopAsync(cancellationToken));
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Connect error: {ex.Message}");
            }
        }

        /// <summary>
        /// Continuously listens for incoming messages.
        /// </summary>
        private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[1024];
            try
            {
                while (_clientWebSocket.State == WebSocketState.Open)
                {
                    var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
                        OnDisconnected?.Invoke();
                    }
                    else
                    {
                        // Convert the raw bytes into a UTF-8 string (expected to be JSON)
                        string jsonMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                        ServerMessage serverMsg = null;
                        try
                        {
                            // Deserialize the JSON message into our ServerMessage object.
                            serverMsg = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerMessage>(jsonMessage);
                        }
                        catch (Exception ex)
                        {
                            // Notify error if the JSON parsing fails.
                            OnError?.Invoke($"JSON parse error: {ex.Message}");
                            continue;
                        }

                        // Process the message based on the 'Type' property.
                        switch (serverMsg.Type)
                        {
                            case "connection":
                                // Example: { "type": "connection", "clientId": "abc123" }
                                _id = serverMsg.ClientId;
                                break;
                            case "newConnection":
                                // Example: { "type": "newConnection", "clientId": "xyz789" }
                                OnMessageReceived?.Invoke($"New client connected: {serverMsg.ClientId}");
                                break;
                            case "message":
                                // Example: { "type": "message", "clientId": "abc123", "data": "Hello World" }
                                OnMessageReceived?.Invoke($"{serverMsg.ClientId}: {serverMsg.Data}");
                                break;
                            case "disconnection":
                                OnMessageReceived?.Invoke($"{serverMsg.ClientId}: Disconnected");
                                break;
                            case "error":
                                // Example: { "type": "error", "message": "Invalid event" }
                                OnError?.Invoke($"Server error: {serverMsg.Message}");
                                break;
                            default:
                                OnError?.Invoke($"Unknown message type: {serverMsg.Type}");
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Receive error: {ex.Message}");
            }
        }


        /// <summary>
        /// Sends a text message over the WebSocket.
        /// </summary>
        public async Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            if (_clientWebSocket?.State == WebSocketState.Open)
            {
                var payload = new
                {
                    type = "message",
                    data = message,
                    clientId = _id
                };
                string jsonPayload = JsonConvert.SerializeObject(payload);
                var bytes = Encoding.UTF8.GetBytes(jsonPayload);
                await _clientWebSocket.SendAsync(new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text, true, cancellationToken);
            }
        }

        public void Dispose()
        {
            _clientWebSocket?.Dispose();
        }
    }
}
