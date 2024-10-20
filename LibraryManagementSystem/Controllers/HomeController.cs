using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly LibraryContext _context;

        public HomeController(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var featuredBooks = await _context.Books
                .Where(b => b.IsAvailable)
                .OrderByDescending(b => b.Id)
                .Take(4)
                .ToListAsync();
            return View(featuredBooks);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction("Index");
            }

            var books = await _context.Books
                .Where(b => b.Title.Contains(query) || b.Author.Contains(query) || b.ISBN.Contains(query))
                .ToListAsync();

            ViewData["SearchQuery"] = query;
            return View(books);
        }
    }
}