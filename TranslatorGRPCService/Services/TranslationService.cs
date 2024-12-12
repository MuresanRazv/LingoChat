using Grpc.Core;
using TranslationModelLibrary.Models;
using TranslatorGRPCService;

namespace TranslatorGRPCService.Services
{
    public class TranslationService : TranslatorGRPCService.TranslationService.TranslationServiceBase
    {
        private readonly ILocalLLM _llm;

        public TranslationService(ILocalLLM llm)
        {
            _llm = llm;
        }

        public override async Task<TranslationResponse> Translate(TranslationRequest requestData, ServerCallContext context)
        {
            var translatedData = await _llm.TranslateAsync(new TranslationRequestDTO { Text = requestData.SourceText, SourceLanguage = requestData.SourceLanguage, TargetLanguage = requestData.TargetLanguage});

            return new TranslationResponse
            {
                TranslatedText = translatedData.Translation,
            };
        }
    }
}
