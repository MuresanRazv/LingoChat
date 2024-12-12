using TranslationModelLibrary.Models;

namespace TranslatorGRPCService.Services
{
    public interface ILocalLLM
    {
        public Task<TranslationResponseDTO> TranslateAsync(TranslationRequestDTO requestData);
    }
}
