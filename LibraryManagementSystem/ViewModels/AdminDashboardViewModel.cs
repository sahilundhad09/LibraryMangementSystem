using System.Collections.Generic;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveLoans { get; set; }
        public List<Book> RecentlyAddedBooks { get; set; }
        public List<Loan> OverdueLoans { get; set; }

        // Add any other properties you need for your admin dashboard
    }
}