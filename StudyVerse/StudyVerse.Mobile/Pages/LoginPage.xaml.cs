using StudyVerse.Mobile.Models;
using StudyVerse.Mobile.Services;

namespace StudyVerse.Mobile.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService = new AuthService();

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void LoginClicked(object sender, EventArgs e)
    {
        StatusLabel.Text = string.Empty;

        if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            StatusLabel.Text = "Please enter email and password.";
            return;
        }

        var result = await _authService.LoginAsync(
            EmailEntry.Text.Trim(),
            PasswordEntry.Text
        );

        if (result == null || !result.Success)
        {
            StatusLabel.Text = result?.Message ?? "Login failed.";
            return;
        }

        MobileUserSession.Save(result.UserId, result.Email, result.UserName);

        await Shell.Current.GoToAsync("//DashboardPage");
    }
}