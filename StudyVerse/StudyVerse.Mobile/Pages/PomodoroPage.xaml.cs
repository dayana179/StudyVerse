namespace StudyVerse.Mobile.Pages;

public partial class PomodoroPage : ContentPage
{
    private const int FocusSeconds = 25 * 60;
    private const int BreakSeconds = 5 * 60;

    private readonly IDispatcherTimer _timer;

    private int _timeLeft;
    private bool _isRunning;
    private bool _isFocusMode;
    private int _completedFocusSessions;

    public PomodoroPage()
    {
        InitializeComponent();

        _timeLeft = Preferences.Get("PomodoroTimeLeft", FocusSeconds);
        _isRunning = Preferences.Get("PomodoroIsRunning", false);
        _isFocusMode = Preferences.Get("PomodoroIsFocusMode", true);
        _completedFocusSessions = Preferences.Get("PomodoroCompletedSessions", 0);

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += TimerTick;

        UpdateDisplay();

        if (_isRunning)
        {
            _timer.Start();
        }
    }

    private async void TimerTick(object? sender, EventArgs e)
    {
        if (_timeLeft > 0)
        {
            _timeLeft--;
            SaveState();
            UpdateDisplay();
            return;
        }

        _timer.Stop();
        _isRunning = false;

        if (_isFocusMode)
        {
            _completedFocusSessions++;
            _isFocusMode = false;
            _timeLeft = BreakSeconds;
            SaveState();
            UpdateDisplay();

            await DisplayAlert("Focus complete", "Good job. Time for a 5-minute break.", "OK");
        }
        else
        {
            _isFocusMode = true;
            _timeLeft = FocusSeconds;
            SaveState();
            UpdateDisplay();

            await DisplayAlert("Break complete", "Break is over. Ready for another focus session.", "OK");
        }
    }

    private void StartClicked(object sender, EventArgs e)
    {
        if (_isRunning)
        {
            return;
        }

        _isRunning = true;
        _timer.Start();
        SaveState();
        UpdateDisplay();
    }

    private void PauseClicked(object sender, EventArgs e)
    {
        _isRunning = false;
        _timer.Stop();
        SaveState();
        UpdateDisplay();
    }

    private void ResetClicked(object sender, EventArgs e)
    {
        _isRunning = false;
        _timer.Stop();

        _isFocusMode = true;
        _timeLeft = FocusSeconds;

        SaveState();
        UpdateDisplay();
    }

    private void SetFocusClicked(object sender, EventArgs e)
    {
        _isRunning = false;
        _timer.Stop();

        _isFocusMode = true;
        _timeLeft = FocusSeconds;

        SaveState();
        UpdateDisplay();
    }

    private void SetBreakClicked(object sender, EventArgs e)
    {
        _isRunning = false;
        _timer.Stop();

        _isFocusMode = false;
        _timeLeft = BreakSeconds;

        SaveState();
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        int minutes = _timeLeft / 60;
        int seconds = _timeLeft % 60;

        TimerLabel.Text = $"{minutes:00} : {seconds:00}";
        ModeLabel.Text = _isFocusMode ? "Focus Session" : "Short Break";
    }

    private void SaveState()
    {
        Preferences.Set("PomodoroTimeLeft", _timeLeft);
        Preferences.Set("PomodoroIsRunning", _isRunning);
        Preferences.Set("PomodoroIsFocusMode", _isFocusMode);
        Preferences.Set("PomodoroCompletedSessions", _completedFocusSessions);
    }
}