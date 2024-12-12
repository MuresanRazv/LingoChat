
using System.Text.Json;
using TranslationModelLibrary.Models;

namespace TranslatorGRPCService.Services
{
    public class LocalLLM : ILocalLLM
    {
        async Task<TranslationResponseDTO> ILocalLLM.TranslateAsync(TranslationRequestDTO requestData)
        {
            using (HttpClient client = new HttpClient())
            {
                string requestUrl = $"http://localhost:5000/translate";
                var body = new
                {
                    text = requestData.Text,
                    source_language = requestData.SourceLanguage,
                    target_language = requestData.TargetLanguage
                };

                HttpResponseMessage response = await client.PostAsJsonAsync(requestUrl, body);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var translationResponse = JsonSerializer.Deserialize<TranslationResponseDTO>(responseBody);

                return translationResponse;
            }
        }
    }
}
