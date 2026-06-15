namespace StudyVerse.Mobile.Models
{
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? UserName { get; set; }
    }
}
