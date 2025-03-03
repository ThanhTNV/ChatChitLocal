using System.Net.WebSockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatChitClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
        string serverUri = ServerUriTextBox.Text.Trim();

        // We'll attempt a quick connection here to validate the URI and server availability.
        using var clientWebSocket = new ClientWebSocket();
        // Set a 5-second timeout (adjust as needed).
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        try
        {
            if (!serverUri.StartsWith("ws://"))
            {
                serverUri = "ws://" + serverUri;
            }
            // Try to connect within the timeout.
            await clientWebSocket.ConnectAsync(new Uri(serverUri), cts.Token);

            // If successful, open the ChatWindow and pass the server URI.
            var chatWindow = new ChatWindow(serverUri);
            chatWindow.Show();

            // Close the HomeWindow.
            this.Close();
        }
        catch (OperationCanceledException)
        {
            // This indicates our 5-second timeout expired.
            MessageBox.Show("Connection to the server timed out. Please try again later.",
                            "Connection Timeout",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            // Any other connection error (e.g., wrong URI, server offline, etc.)
            MessageBox.Show($"Connection error: {ex.Message}",
                            "Connection Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}