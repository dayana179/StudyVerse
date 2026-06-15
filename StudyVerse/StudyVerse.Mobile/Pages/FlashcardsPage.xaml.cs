using StudyVerse.Mobile.Models;
using StudyVerse.Mobile.Services;

namespace StudyVerse.Mobile.Pages;

public partial class FlashcardsPage : ContentPage
{
    private readonly FlashcardService _flashcardService = new FlashcardService();

    private List<FlashcardDeckDto> _decks = new();

    public FlashcardsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!MobileUserSession.IsLoggedIn)
        {
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        await LoadDecksAsync();
    }

    private async Task LoadDecksAsync()
    {
        _decks = await _flashcardService.GetDecksAsync();
        DeckCollection.ItemsSource = _decks;
    }

    private async void RefreshClicked(object sender, EventArgs e)
    {
        await LoadDecksAsync();
    }

    private async void CreateDeckClicked(object sender, EventArgs e)
    {
        string name = await DisplayPromptAsync("Create Deck", "Enter deck name:");

        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        string description = await DisplayPromptAsync("Deck Description", "Enter deck description:");

        bool success = await _flashcardService.CreateDeckAsync(name.Trim(), description?.Trim());

        if (!success)
        {
            await DisplayAlert("Error", "Deck could not be created.", "OK");
            return;
        }

        await LoadDecksAsync();
    }

    private async void OpenDeckClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not int deckId)
        {
            return;
        }

        var deck = _decks.FirstOrDefault(d => d.Id == deckId);

        if (deck == null)
        {
            return;
        }

        await Navigation.PushAsync(new FlashcardDeckPage(deck));
    }

    private async void DeleteDeckClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not int deckId)
        {
            return;
        }

        bool confirm = await DisplayAlert(
            "Delete Deck",
            "Are you sure you want to delete this deck and all its cards?",
            "Delete",
            "Cancel"
        );

        if (!confirm)
        {
            return;
        }

        bool success = await _flashcardService.DeleteDeckAsync(deckId);

        if (!success)
        {
            await DisplayAlert("Error", "Deck could not be deleted.", "OK");
            return;
        }

        await LoadDecksAsync();
    }
}