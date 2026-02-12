namespace LibraryApi.Models.DTOs//DTOs, To separate the Database Schema from the API Interface.,Used in Controller method parameters 
{
    public class BookCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string Status { get; set; } = "Available";
    }
}