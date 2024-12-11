using Microsoft.AspNetCore.Mvc;
using TranslatorGRPCService;
using TranslationModelLibrary.Models;
using Grpc.Net.Client;
using System.Threading.Channels;

namespace TranslationWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranslationController : Controller
    {
        private readonly TranslationService.TranslationServiceClient _client;

        public TranslationController()
        {
            _client = new TranslationService.TranslationServiceClient(GrpcChannel.ForAddress("http://localhost:5068"));
        }

        [HttpPost("translate")]
        public async Task<IActionResult> TranslateAsync([FromBody] TranslationRequestDTO request)
        {
            var translationGRPCRequest = new TranslationRequest
            {
                SourceText = request.Text,
                SourceLanguage = request.SourceLanguage,
                TargetLanguage = request.TargetLanguage
            };

            var response = await _client.TranslateAsync(translationGRPCRequest);

            return Ok(new TranslationResponseDTO
            {
                Translation = response.TranslatedText
            });
        }
    }
}
