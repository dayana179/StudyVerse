using StudyVerse.Mobile.Models;
using StudyVerse.Mobile.Services;

namespace StudyVerse.Mobile.Pages;

public partial class FlashcardsPage : ContentPage
{
    private readonly FlashcardService _flashcardService = new FlashcardService();

    private List<FlashcardDto> _cards = new();
    private int _currentIndex = 0;
    private bool _showingAnswer = false;

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

        UserLabel.Text = $"Logged in as {MobileUserSession.Email}";

        await LoadCardsAsync();
    }

    private async Task LoadCardsAsync()
    {
        _cards = await _flashcardService.GetFlashcardsAsync();

        if (_currentIndex >= _cards.Count)
        {
            _currentIndex = 0;
        }

        _showingAnswer = false;
        ShowCurrentCard();
    }

    private void ShowCurrentCard()
    {
        if (_cards.Count == 0)
        {
            CardLabel.Text = "No flashcards yet";
            CountLabel.Text = "0 cards";
            return;
        }

        if (_currentIndex < 0)
        {
            _currentIndex = 0;
        }

        if (_currentIndex >= _cards.Count)
        {
            _currentIndex = _cards.Count - 1;
        }

        var card = _cards[_currentIndex];

        CardLabel.Text = _showingAnswer ? card.Answer : card.Question;
        CountLabel.Text = $"Card {_currentIndex + 1} of {_cards.Count}";
    }

    private async void AddFlashcardClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(QuestionEntry.Text) || string.IsNullOrWhiteSpace(AnswerEntry.Text))
        {
            await DisplayAlert("Missing info", "Please enter both question and answer.", "OK");
            return;
        }

        var flashcard = new FlashcardDto
        {
            Question = QuestionEntry.Text.Trim(),
            Answer = AnswerEntry.Text.Trim()
        };

        bool success = await _flashcardService.CreateFlashcardAsync(flashcard);

        if (!success)
        {
            await DisplayAlert("Error", "Flashcard could not be added. Make sure the MVC project is running and you are logged in.", "OK");
            return;
        }

        QuestionEntry.Text = string.Empty;
        AnswerEntry.Text = string.Empty;

        await LoadCardsAsync();

        if (_cards.Count > 0)
        {
            _currentIndex = 0;
            ShowCurrentCard();
        }
    }

    private async void RefreshClicked(object sender, EventArgs e)
    {
        await LoadCardsAsync();
    }

    private void FlipCardClicked(object sender, EventArgs e)
    {
        if (_cards.Count == 0) return;

        _showingAnswer = !_showingAnswer;
        ShowCurrentCard();
    }

    private void PreviousCardClicked(object sender, EventArgs e)
    {
        if (_cards.Count == 0) return;

        _currentIndex--;

        if (_currentIndex < 0)
        {
            _currentIndex = _cards.Count - 1;
        }

        _showingAnswer = false;
        ShowCurrentCard();
    }

    private void NextCardClicked(object sender, EventArgs e)
    {
        if (_cards.Count == 0) return;

        _currentIndex++;

        if (_currentIndex >= _cards.Count)
        {
            _currentIndex = 0;
        }

        _showingAnswer = false;
        ShowCurrentCard();
    }

    private async void DeleteCardClicked(object sender, EventArgs e)
    {
        if (_cards.Count == 0) return;

        var card = _cards[_currentIndex];

        bool confirm = await DisplayAlert(
            "Delete Flashcard",
            "Are you sure you want to delete this flashcard?",
            "Delete",
            "Cancel"
        );

        if (!confirm)
        {
            return;
        }

        bool success = await _flashcardService.DeleteFlashcardAsync(card.Id);

        if (!success)
        {
            await DisplayAlert("Error", "Flashcard could not be deleted.", "OK");
            return;
        }

        await LoadCardsAsync();
    }
}