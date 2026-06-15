namespace StudyVerse.Mobile.Models
{
    public static class MobileUserSession
    {
        public static bool IsLoggedIn =>
            !string.IsNullOrWhiteSpace(Preferences.Get("MobileUserId", string.Empty));

        public static string UserId =>
            Preferences.Get("MobileUserId", string.Empty);

        public static string Email =>
            Preferences.Get("MobileUserEmail", string.Empty);

        public static string UserName =>
            Preferences.Get("MobileUserName", string.Empty);

        public static void Save(string userId, string? email, string? userName)
        {
            Preferences.Set("MobileUserId", userId);
            Preferences.Set("MobileUserEmail", email ?? string.Empty);
            Preferences.Set("MobileUserName", userName ?? string.Empty);
        }

        public static void Clear()
        {
            Preferences.Remove("MobileUserId");
            Preferences.Remove("MobileUserEmail");
            Preferences.Remove("MobileUserName");
        }
    }
}