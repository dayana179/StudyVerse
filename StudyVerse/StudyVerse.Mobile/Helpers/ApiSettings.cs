namespace StudyVerse.Mobile.Helpers
{
    public static class ApiSettings
    {
        public static string BaseUrl
        {
            get
            {
#if ANDROID
                return "https://10.0.2.2:44329";
#else
                return "https://localhost:44329";
#endif
            }
        }

        public static string GetFullUrl(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return string.Empty;
            }

            if (filePath.StartsWith("http"))
            {
                return filePath;
            }

            return $"{BaseUrl}{filePath}";
        }
    }
}