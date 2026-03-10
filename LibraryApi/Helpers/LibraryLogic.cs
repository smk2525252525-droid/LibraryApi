namespace LibraryApi.Helpers
{
    public class LibraryLogic
    {
        // Rule 1: Passwords must be at least 6 characters long
        public bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrEmpty(password)) return false;
            return password.Length >= 6;
        }

        // Rule 2: Calculate if a book is overdue (Assuming loan is 14 days)
        public bool IsOverdue(DateTime issuedAt, DateTime current)
        {
            TimeSpan duration = current - issuedAt;
            return duration.Days > 14;
        }
    }
}