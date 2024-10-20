using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.ViewModels
{
    public class SendMessageViewModel
    {
        public List<User> UsersWithOverdueLoans { get; set; }

        [Required(ErrorMessage = "Please select a user.")]
        [Display(Name = "Select User")]
        public int SelectedUserId { get; set; }

        [Required(ErrorMessage = "Message content is required.")]
        [Display(Name = "Message")]
        public string MessageContent { get; set; }
    }
}