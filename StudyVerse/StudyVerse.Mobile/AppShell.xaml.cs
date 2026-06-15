using StudyVerse.Mobile.Models;

namespace StudyVerse.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Loaded += async (sender, e) =>
        {
            if (!MobileUserSession.IsLoggedIn)
            {
                await GoToAsync("//LoginPage");
            }
            else
            {
                await GoToAsync("//DashboardPage");
            }
        };
    }
}