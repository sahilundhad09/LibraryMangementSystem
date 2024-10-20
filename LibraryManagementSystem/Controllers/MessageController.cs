using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly LibraryContext _context;

        public MessageController(LibraryContext context)
        {
            _context = context;
        }

        // Display list of messages for the logged-in user
        public async Task<IActionResult> MyMessages()
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User ID not found");
            }

            var messages = await _context.Messages
                .Where(m => m.UserId == userId.Value)
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();

            return View(messages);
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out int userId) ? userId : (int?)null;
        }
    }
}
