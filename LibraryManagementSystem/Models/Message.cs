using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime SentDate { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false; // Track if the user has read the message
    }
}
