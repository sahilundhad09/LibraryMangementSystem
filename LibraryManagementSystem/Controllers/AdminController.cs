using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly LibraryContext _context;

        public AdminController(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalBooks = await _context.Books.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                ActiveLoans = await _context.Loans.Where(l => l.ReturnDate == null).CountAsync(),
                RecentlyAddedBooks = await _context.Books.OrderByDescending(b => b.Id).Take(5).ToListAsync(),
                OverdueLoans = await _context.Loans
                    .Where(l => l.ReturnDate == null && l.DueDate < DateTime.Now)
                    .Include(l => l.Book)
                    .Include(l => l.User)
                    .ToListAsync() // Change here for full list
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> OverdueLoans()
        {
            var overdueLoans = await _context.Loans
                .Include(l => l.User)
                .Include(l => l.Book)
                .Where(l => l.DueDate < DateTime.Now && l.ReturnDate == null)
                .ToListAsync();

            return View(overdueLoans);
        }

        [HttpGet]
        public async Task<IActionResult> SendMessage()
        {
            var usersWithOverdueLoans = await _context.Loans
                .Where(l => l.DueDate < DateTime.Now && l.ReturnDate == null)
                .Select(l => l.User)
                .Distinct()
                .ToListAsync();

            var model = new SendMessageViewModel
            {
                UsersWithOverdueLoans = usersWithOverdueLoans
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate the list of users with overdue loans
                model.UsersWithOverdueLoans = await _context.Loans
                    .Where(l => l.DueDate < DateTime.Now && l.ReturnDate == null)
                    .Select(l => l.User)
                    .Distinct()
                    .ToListAsync();

                return View(model);
            }

            var message = new Message
            {
                UserId = model.SelectedUserId,
                Content = model.MessageContent,
                SentDate = DateTime.Now,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Message sent successfully!";
            return RedirectToAction("Index");
        }
    }
}
