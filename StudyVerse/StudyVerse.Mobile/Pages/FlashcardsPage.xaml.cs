using System.Text.Json;
using StudyVerse.Mobile.Models;

namespace StudyVerse.Mobile.Pages;

public partial class FlashcardsPage : ContentPage
{
    private List<MobileFlashcard> _cards = new();
    private int _currentIndex = 0;
    private bool _showingAnswer = false;

    public FlashcardsPage()
    {
        InitializeComponent();
        LoadCards();
        ShowCurrentCard();
    }

    private void LoadCards()
    {
        string json = Preferences.Get("mobileFlashcards", "[]");
        _cards = JsonSerializer.Deserialize<List<MobileFlashcard>>(json) ?? new List<MobileFlashcard>();
    }

    private void SaveCards()
    {
        string json = JsonSerializer.Serialize(_cards);
        Preferences.Set("mobileFlashcards", json);
    }

    private void ShowCurrentCard()
    {
        if (_cards.Count == 0)
        {
            CardLabel.Text = "No flashcards yet";
            return;
        }

        if (_currentIndex < 0) _currentIndex = 0;
        if (_currentIndex >= _cards.Count) _currentIndex = _cards.Count - 1;

        var card = _cards[_currentIndex];
        CardLabel.Text = _showingAnswer ? card.Answer : card.Question;
    }

    private void AddFlashcardClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(QuestionEntry.Text) || string.IsNullOrWhiteSpace(AnswerEntry.Text))
        {
            return;
        }

        _cards.Add(new MobileFlashcard
        {
            Question = QuestionEntry.Text.Trim(),
            Answer = AnswerEntry.Text.Trim()
        });

        QuestionEntry.Text = string.Empty;
        AnswerEntry.Text = string.Empty;

        _currentIndex = _cards.Count - 1;
        _showingAnswer = false;

        SaveCards();
        ShowCurrentCard();
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

    private void DeleteCardClicked(object sender, EventArgs e)
    {
        if (_cards.Count == 0) return;

        _cards.RemoveAt(_currentIndex);

        if (_currentIndex >= _cards.Count)
        {
            _currentIndex = _cards.Count - 1;
        }

        _showingAnswer = false;

        SaveCards();
        ShowCurrentCard();
    }
}