using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WebSocketClientLib;
using System.Windows.Data;

namespace ChatChitClient
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        private WebSocketClientHandler _clientHandler = new WebSocketClientHandler();
        private readonly string _serverUri;
        private string username = "You";
        private bool isConnected = false;

        public ChatWindow(string serverUri)
        {
            InitializeComponent();
            _serverUri = serverUri;

            // Initialize the window with disconnected state
            UpdateConnectionStatus(false);

            // When the window finishes loading, attempt to connect
            this.Loaded += ChatWindow_Loaded;
        }

        private async void ChatWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await ConnectToServer(_serverUri);
        }

        private async Task ConnectToServer(string uri)
        {
            StatusTextBlock.Text = "Connecting...";
            StatusTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 165, 0)); // Orange

            var ellipse = ((Grid)StatusTextBlock.Parent).Children[1] as Ellipse;
            if (ellipse != null)
                ellipse.Fill = new SolidColorBrush(Color.FromRgb(255, 165, 0));

            try
            {
                var _uri = new Uri(uri);
                await _clientHandler.ConnectAsync(_uri, CancellationToken.None);

                // Update UI to show connected state
                UpdateConnectionStatus(true);
                AddReceivedMessage("System", "Connected to " + uri);

                // Start a background task to receive messages
                _ = Task.Run(ReceiveMessages);
            }
            catch (Exception ex)
            {
                UpdateConnectionStatus(false);
                AddReceivedMessage("System", "Connection error: " + ex.Message);
            }
        }

        // Update the connection status display
        private void UpdateConnectionStatus(bool connected)
        {
            isConnected = connected;

            if (connected)
            {
                StatusTextBlock.Text = "Connected";
                StatusTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Green

                // Update the status indicator ellipse color
                var ellipse = ((Grid)StatusTextBlock.Parent).Children[1] as Ellipse;
                if (ellipse != null)
                    ellipse.Fill = new SolidColorBrush(Color.FromRgb(76, 175, 80));

                MessageTextBox.IsEnabled = true;
            }
            else
            {
                StatusTextBlock.Text = "Disconnected";
                StatusTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Red

                // Update the status indicator ellipse color
                var ellipse = ((Grid)StatusTextBlock.Parent).Children[1] as Ellipse;
                if (ellipse != null)
                    ellipse.Fill = new SolidColorBrush(Color.FromRgb(244, 67, 54));

                MessageTextBox.IsEnabled = false;
            }
        }

        private async Task ReceiveMessages()
        {
            var buffer = new byte[4096]; // Increased buffer size
            _clientHandler.OnMessageReceived += (message) =>
            {    
                var parts = message.Split(new[] { ':' }, 2);
                var sender = parts[0].Trim();  
                message = parts[1].Trim();
                // Add the message to the UI
                Dispatcher.Invoke(() => AddReceivedMessage(sender, message));
            };
            
        }

        // Called when a message is received from the server
        public void AddReceivedMessage(string sender, string message)
        {
            var chatMessage = new ChatMessage(sender, message, false);
            MessagesPanel.Items.Add(chatMessage);
            ScrollView.ScrollToBottom();
        }

        // Add a message from the local user
        private void AddLocalMessage(string message)
        {
            var chatMessage = new ChatMessage(username, message, true);
            MessagesPanel.Items.Add(chatMessage);
            ScrollView.ScrollToBottom();
        }


        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (_clientHandler is null)
            {
                MessageBox.Show("Not connected to server.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string message = MessageTextBox.Text.Trim();
            if (string.IsNullOrEmpty(message))
                return;
            try
            {
                await _clientHandler.SendMessageAsync(message);

                // Add message to local display
                AddLocalMessage(message);

                // Clear the text box after sending
                MessageTextBox.Text = string.Empty;
                MessageTextBox.Focus();
            }
            catch (Exception ex)
            {
                UpdateConnectionStatus(false);
                AddReceivedMessage("System", "Send error: " + ex.Message);
            }
        }

        // Override window closing to ensure proper disconnection
        protected override async void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_clientHandler is not null)
            {
                try
                {
                    _clientHandler.OnDisconnected += () =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            StatusTextBlock.Text = "Disconnected";
                        });
                    };
                }
                catch
                {
                    // Ignore errors during close, just make sure we clean up
                }
            }

            base.OnClosing(e);
        }

        // Allow pressing Enter to send messages
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && MessageTextBox.IsFocused)
            {
                SendButton_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }

            base.OnKeyDown(e);
        }
    }

    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isFromMe = (bool)value;
            return new SolidColorBrush(isFromMe ? Color.FromRgb(235, 245, 255) : Color.FromRgb(245, 245, 245));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isFromMe = (bool)value;
            return isFromMe ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}