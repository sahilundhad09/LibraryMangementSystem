namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int Quantity { get; set; }
        public bool IsAvailable { get; set; }

        // Navigation property for related loans
        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}