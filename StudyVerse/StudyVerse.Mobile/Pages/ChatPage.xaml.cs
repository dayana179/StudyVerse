using StudyVerse.Mobile.Models;
using StudyVerse.Mobile.Services;

namespace StudyVerse.Mobile.Pages;

public partial class ChatPage : ContentPage
{
    private readonly ChatService _chatService = new ChatService();

    private int _cooldownSecondsLeft = 0;
    private IDispatcherTimer? _cooldownTimer;

    public ChatPage()
    {
        InitializeComponent();
        SetupCooldownTimer();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!MobileUserSession.IsLoggedIn)
        {
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        UserLabel.Text = $"Logged in as {MobileUserSession.Email}";

        await LoadMessagesAsync();
        RestoreCooldown();
    }

    private void SetupCooldownTimer()
    {
        _cooldownTimer = Dispatcher.CreateTimer();
        _cooldownTimer.Interval = TimeSpan.FromSeconds(1);
        _cooldownTimer.Tick += (sender, e) =>
        {
            if (_cooldownSecondsLeft > 0)
            {
                _cooldownSecondsLeft--;
                Preferences.Set("ChatCooldownEndTime", DateTime.Now.AddSeconds(_cooldownSecondsLeft).ToString("O"));
                UpdateCooldownDisplay();
            }
            else
            {
                _cooldownTimer.Stop();
                Preferences.Remove("ChatCooldownEndTime");
                UpdateCooldownDisplay();
            }
        };
    }

    private void StartCooldown()
    {
        _cooldownSecondsLeft = 10;
        Preferences.Set("ChatCooldownEndTime", DateTime.Now.AddSeconds(10).ToString("O"));
        UpdateCooldownDisplay();
        _cooldownTimer?.Start();
    }

    private void RestoreCooldown()
    {
        string savedEndTime = Preferences.Get("ChatCooldownEndTime", string.Empty);

        if (DateTime.TryParse(savedEndTime, out DateTime endTime))
        {
            int remaining = (int)Math.Ceiling((endTime - DateTime.Now).TotalSeconds);

            if (remaining > 0)
            {
                _cooldownSecondsLeft = remaining;
                UpdateCooldownDisplay();
                _cooldownTimer?.Start();
                return;
            }
        }

        _cooldownSecondsLeft = 0;
        Preferences.Remove("ChatCooldownEndTime");
        UpdateCooldownDisplay();
    }

    private void UpdateCooldownDisplay()
    {
        if (_cooldownSecondsLeft > 0)
        {
            SendButton.IsEnabled = false;
            SendButton.Text = $"{_cooldownSecondsLeft}s";
            CooldownLabel.Text = $"Please wait {_cooldownSecondsLeft} seconds before sending another message.";
        }
        else
        {
            SendButton.IsEnabled = true;
            SendButton.Text = "Send";
            CooldownLabel.Text = "";
        }
    }

    private async Task LoadMessagesAsync()
    {
        var messages = await _chatService.GetMessagesAsync();

        MessagesCollection.ItemsSource = messages;

        if (messages.Count > 0)
        {
            MessagesCollection.ScrollTo(messages.Last(), position: ScrollToPosition.End, animate: true);
        }
    }

    private async void RefreshClicked(object sender, EventArgs e)
    {
        await LoadMessagesAsync();
    }

    private async void SendClicked(object sender, EventArgs e)
    {
        if (_cooldownSecondsLeft > 0)
        {
            await DisplayAlert("Slow down", $"Please wait {_cooldownSecondsLeft} seconds before sending another message.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(MessageEntry.Text))
        {
            return;
        }

        string message = MessageEntry.Text.Trim();

        bool success = await _chatService.SendMessageAsync(message);

        if (!success)
        {
            await DisplayAlert("Error", "Message could not be sent. You may need to wait 10 seconds before sending another message.", "OK");
            StartCooldown();
            return;
        }

        MessageEntry.Text = string.Empty;

        StartCooldown();

        await LoadMessagesAsync();
    }

    private async void DeleteMessageClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not int id)
        {
            return;
        }

        bool confirm = await DisplayAlert(
            "Delete Message",
            "Are you sure you want to delete this message?",
            "Delete",
            "Cancel"
        );

        if (!confirm)
        {
            return;
        }

        bool success = await _chatService.DeleteMessageAsync(id);

        if (!success)
        {
            await DisplayAlert("Not allowed", "You can only delete your own messages.", "OK");
            return;
        }

        await LoadMessagesAsync();
    }
}