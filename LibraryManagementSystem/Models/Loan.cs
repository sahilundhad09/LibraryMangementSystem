using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class Loan
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int BookId { get; set; }
        public Book Book { get; set; }

        [Required]
        public DateTime LoanDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        [NotMapped]
        public bool IsOverdue => ReturnDate == null && DateTime.Now > DueDate;

        [NotMapped]
        public int MinutesOverdue => IsOverdue ? (int)(DateTime.Now - DueDate).TotalMinutes : 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal FineAmount => MinutesOverdue * 0.50m;  // Fine is 0.50 per overdue minute

        public bool IsFinePaid { get; set; }

        [NotMapped]
        public string Status => ReturnDate.HasValue ? "Returned" : (IsOverdue ? "Overdue" : "On Loan");
    }
}
