using StudyVerse.Mobile.Models;
using StudyVerse.Mobile.Services;

namespace StudyVerse.Mobile.Pages;

public partial class FlashcardDeckPage : ContentPage
{
    private readonly FlashcardService _flashcardService = new FlashcardService();

    private readonly FlashcardDeckDto _deck;
    private List<FlashcardDto> _cards = new();
    private int _currentIndex = 0;
    private bool _showingAnswer = false;

    public FlashcardDeckPage(FlashcardDeckDto deck)
    {
        InitializeComponent();

        _deck = deck;

        DeckTitleLabel.Text = deck.Name;
        DeckDescriptionLabel.Text = deck.Description ?? "Review this deck.";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCardsAsync();
    }

    private async Task LoadCardsAsync()
    {
        _cards = await _flashcardService.GetCardsAsync(_deck.Id);

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
            CardLabel.Text = "No cards yet";
            CardCountLabel.Text = "0 cards";
            return;
        }

        if (_currentIndex < 0)
        {
            _currentIndex = _cards.Count - 1;
        }

        if (_currentIndex >= _cards.Count)
        {
            _currentIndex = 0;
        }

        var card = _cards[_currentIndex];

        CardLabel.Text = _showingAnswer ? card.Answer : card.Question;
        CardCountLabel.Text = $"Card {_currentIndex + 1} of {_cards.Count}";
    }

    private async void RefreshClicked(object sender, EventArgs e)
    {
        await LoadCardsAsync();
    }

    private async void AddCardClicked(object sender, EventArgs e)
    {
        string question = await DisplayPromptAsync("Add Card", "Enter question:");

        if (string.IsNullOrWhiteSpace(question))
        {
            return;
        }

        string answer = await DisplayPromptAsync("Add Card", "Enter answer:");

        if (string.IsNullOrWhiteSpace(answer))
        {
            return;
        }

        bool success = await _flashcardService.CreateCardAsync(_deck.Id, question.Trim(), answer.Trim());

        if (!success)
        {
            await DisplayAlert("Error", "Card could not be added.", "OK");
            return;
        }

        await LoadCardsAsync();
    }

    private void FlipClicked(object sender, EventArgs e)
    {
        if (_cards.Count == 0)
        {
            return;
        }

        _showingAnswer = !_showingAnswer;
        ShowCurrentCard();
    }

    private void PreviousClicked(object sender, EventArgs e)
    {
        if (_cards.Count == 0)
        {
            return;
        }

        _currentIndex--;
        _showingAnswer = false;
        ShowCurrentCard();
    }

    private void NextClicked(object sender, EventArgs e)
    {
        if (_cards.Count == 0)
        {
            return;
        }

        _currentIndex++;
        _showingAnswer = false;
        ShowCurrentCard();
    }

    private async void DeleteCardClicked(object sender, EventArgs e)
    {
        if (_cards.Count == 0)
        {
            return;
        }

        var card = _cards[_currentIndex];

        bool confirm = await DisplayAlert(
            "Delete Card",
            "Are you sure you want to delete this card?",
            "Delete",
            "Cancel"
        );

        if (!confirm)
        {
            return;
        }

        bool success = await _flashcardService.DeleteCardAsync(card.Id);

        if (!success)
        {
            await DisplayAlert("Error", "Card could not be deleted.", "OK");
            return;
        }

        await LoadCardsAsync();
    }
}