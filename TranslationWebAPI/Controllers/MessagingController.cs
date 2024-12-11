using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TranslationModelLibrary.Context;
using TranslationModelLibrary.Models;

namespace TranslationWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagingController : Controller
    {
        private readonly TranslationContext _context;

        public MessagingController(TranslationContext context)
        {
            _context = context;
        }

        [HttpGet("GetMessages")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages(string userId, string receiverId)
        {
            var senderMessages = await _context.Messages
                .Where(m => m.SenderId == userId && m.ReceiverId == receiverId)
                .ToListAsync();

            var receivedMessages = await _context.Messages
                .Where(m => m.SenderId == receiverId && m.ReceiverId == userId)
                .ToListAsync();

            var translatedReceivedMessages = new List<Message>();

            foreach (var message in receivedMessages) 
            {
                var translatedMessage = await _context.TranslatedMessages
                    .Where(tm => tm.MessageId == message.Id)
                    .FirstOrDefaultAsync();

                if (translatedMessage != null)
                {
                    message.Content = translatedMessage.TranslatedContent;
                }
            }
            
            var allMessages = senderMessages.Concat(receivedMessages).ToList().OrderBy(m => m.TimeStamp).ToList();

            return allMessages;
        }

        [HttpGet("GetFriendRelationships")]
        public async Task<ActionResult<IEnumerable<FriendRelationship>>> GetFriendRelationships(string userId)
        {
            var friendRelationships = await _context.FriendRelationships
                .Where(fr => fr.UserId1 == userId || fr.UserId2 == userId)
                .ToListAsync();
            return friendRelationships;
        }

        [HttpGet("GetFriendRequests")]
        public async Task<ActionResult<IEnumerable<FriendRequests>>> GetFriendRequests(string userId)
        {
            var friendRequests = await _context.FriendRequests
                .Where(fr => fr.ReceiverId == userId && fr.Status != "Accepted")
                .ToListAsync();
            return friendRequests;
        }

        [HttpPost("SendFriendRequest")]
        public async Task<ActionResult<FriendRequests>> SendFriendRequest(string senderId, string receiverId)
        {
            var friendRequest = new FriendRequests
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Status = "Pending"
            };
            _context.FriendRequests.Add(friendRequest);
            await _context.SaveChangesAsync();
            return friendRequest;
        }

        [HttpPost("AcceptFriendRequest")]
        public async Task<ActionResult<FriendRelationship>> AcceptFriendRequest(string senderId, string receiverId)
        {
            var friendRequest = await _context.FriendRequests
                .Where(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId)
                .FirstOrDefaultAsync();
            if (friendRequest == null)
            {
                return NotFound();
            }
            friendRequest.Status = "Accepted";
            _context.FriendRequests.Update(friendRequest);
            var friendRelationship = new FriendRelationship
            {
                UserId1 = senderId,
                UserId2 = receiverId
            };
            _context.FriendRelationships.Add(friendRelationship);
            await _context.SaveChangesAsync();
            return friendRelationship;
        }

        [HttpPost("DeclineFriendRequest")]
        public async Task<ActionResult<FriendRequests>> DeclineFriendRequest(string senderId, string receiverId)
        {
            var friendRequest = await _context.FriendRequests
                .Where(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId)
                .FirstOrDefaultAsync();
            if (friendRequest == null)
            {
                return NotFound();
            }
            friendRequest.Status = "Declined";
            _context.FriendRequests.Update(friendRequest);
            await _context.SaveChangesAsync();
            return friendRequest;
        }
    }
}
