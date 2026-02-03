namespace LibraryApi.Models.DTOs
{
    public class BookCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string Status { get; set; } = "Available";
    }
}