using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranslationModelLibrary.Context;
using TranslationProject.Models;

namespace TranslationProject.Controllers
{
    [Authorize(Policy = "RequireBasicRole")]
    public class ChatController : Controller
    {
        private string _baseUrl = "http://localhost:5034/api/Messaging";
        private readonly UserManager<TranslationUser> _userManager;
        private readonly TranslationContext _context;

        public ChatController(UserManager<TranslationUser> userManager, TranslationContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Message(string receiverId)
        {
            ViewData["ReceiverId"] = receiverId;
            return View();
        }

        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> FriendRequests()
        {
            var user = await _userManager.GetUserAsync(User);
            var friendRequests = await _context.FriendRequests
                .Where(fr => fr.ReceiverId == user.Id && fr.Status != "Accepted")
                .ToListAsync();
            return View(friendRequests);
        }

        public async Task<IActionResult> Friends()
        {
            var user = await _userManager.GetUserAsync(User);
            var friendRelationships = await _context.FriendRelationships
                .Where(fr => fr.UserId1 == user.Id || fr.UserId2 == user.Id)
                .ToListAsync();
            return View(friendRelationships);
        }
    }
}
