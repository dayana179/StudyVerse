namespace StudyVerse.Mobile.Pages;

public partial class PomodoroPage : ContentPage
{
    private IDispatcherTimer _timer;
    private int _timeLeft;
    private bool _isRunning;

    public PomodoroPage()
    {
        InitializeComponent();

        _timeLeft = Preferences.Get("pomodoroTimeLeft", 25 * 60);
        _isRunning = Preferences.Get("pomodoroIsRunning", false);

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += TimerTick;

        UpdateTimerLabel();

        if (_isRunning)
        {
            _timer.Start();
        }
    }

    private void TimerTick(object? sender, EventArgs e)
    {
        if (_timeLeft > 0)
        {
            _timeLeft--;
            SaveState();
            UpdateTimerLabel();
        }
        else
        {
            _timer.Stop();
            _isRunning = false;
            SaveState();
            DisplayAlert("Pomodoro", "Time is up. Take a short break.", "OK");
        }
    }

    private void StartClicked(object sender, EventArgs e)
    {
        if (_isRunning) return;

        _isRunning = true;
        _timer.Start();
        SaveState();
    }

    private void PauseClicked(object sender, EventArgs e)
    {
        _isRunning = false;
        _timer.Stop();
        SaveState();
    }

    private void ResetClicked(object sender, EventArgs e)
    {
        _isRunning = false;
        _timer.Stop();
        _timeLeft = 25 * 60;
        SaveState();
        UpdateTimerLabel();
    }

    private void UpdateTimerLabel()
    {
        int minutes = _timeLeft / 60;
        int seconds = _timeLeft % 60;

        TimerLabel.Text = $"{minutes:00}:{seconds:00}";
    }

    private void SaveState()
    {
        Preferences.Set("pomodoroTimeLeft", _timeLeft);
        Preferences.Set("pomodoroIsRunning", _isRunning);
    }
}