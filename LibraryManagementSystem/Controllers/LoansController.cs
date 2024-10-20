using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    public class LoansController : Controller
    {
        private readonly LibraryContext _context;

        public LoansController(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User ID not found");
            }

            var loans = await _context.Loans
                .Include(l => l.Book)
                .Where(l => l.UserId == userId.Value)
                .ToListAsync();
            return View(loans);
        }

        [HttpPost]
        public async Task<IActionResult> Borrow(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null || !book.IsAvailable)
            {
                return NotFound();
            }

            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User ID not found");
            }

            // Check if the user has already borrowed this book
            var existingLoan = await _context.Loans
                .FirstOrDefaultAsync(l => l.UserId == userId.Value && l.BookId == bookId && l.ReturnDate == null);
            if (existingLoan != null)
            {
                TempData["ErrorMessage"] = "You have already borrowed this book.";
                return RedirectToAction("Details", "Books", new { id = bookId });
            }

            var loan = new Loan
            {
                UserId = userId.Value,
                BookId = bookId,
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddMinutes(3) // 3-minute return time
            };

            book.Quantity--;
            if (book.Quantity == 0)
            {
                book.IsAvailable = false;
            }

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Book borrowed successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Return(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null || loan.ReturnDate != null)
            {
                return NotFound();
            }

            loan.ReturnDate = DateTime.Now;

            var book = await _context.Books.FindAsync(loan.BookId);
            book.Quantity++;
            if (book.Quantity > 0)
            {
                book.IsAvailable = true;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Book returned successfully.";
            return RedirectToAction(nameof(Index));
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out int userId) ? userId : (int?)null;
        }
    }
}
