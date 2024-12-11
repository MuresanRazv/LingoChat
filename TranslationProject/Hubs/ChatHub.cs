using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using TranslationModelLibrary.Context;
using TranslationModelLibrary.Models;
using TranslationProject.Models;

namespace TranslationProject.Hubs
{
    [Authorize(Policy = "RequireBasicRole")]
    public class ChatHub : Hub
    {
        private string _translateBaseUrl = "http://localhost:5034/api/Translation/translate";
        private UserManager<TranslationUser> _userManager;
        private readonly TranslationContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatHub(UserManager<TranslationUser> userManager, TranslationContext context, IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);

            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string receiverId, string messageContent)
        {
            var message = new Message
            {
                SenderId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                ReceiverId = receiverId,
                Content = messageContent,
                TimeStamp = DateTime.Now
            };

            _context.Messages.Add(message);
            var createdMessage = await _context.SaveChangesAsync();

            var senderUser = await _userManager.FindByIdAsync(message.SenderId);
            var receiverUser = await _userManager.FindByIdAsync(message.ReceiverId);

            var httpClient = _httpClientFactory.CreateClient();
            var translatedResponse = await httpClient.PostAsJsonAsync(
                _translateBaseUrl, 
                new TranslationRequestDTO { 
                    SourceLanguage = senderUser.PreferredLanguage, 
                    TargetLanguage = receiverUser.PreferredLanguage, 
                    Text = messageContent
                }
            );
            var translatedMessageContent = await translatedResponse.Content.ReadFromJsonAsync<TranslationResponseDTO>();
            if (translatedMessageContent == null || string.IsNullOrEmpty(translatedMessageContent.Translation))
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Translation result is invalid.");
                return;
            }

            var translatedMessage = new TranslatedMessage
            {
                SourceLanguage = senderUser.PreferredLanguage,
                TargetLanguage = receiverUser.PreferredLanguage,
                MessageId = message.Id,
                TranslatedContent = translatedMessageContent.Translation,
            };

            _context.TranslatedMessages.Add(translatedMessage);
            await _context.SaveChangesAsync();

            await Clients.Group(receiverId).SendAsync("ReceiveMessage", translatedMessageContent.Translation, senderUser.UserName);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
