using OpenAI.Chat;
namespace SISC.Integrations
{
    public class OpenAiClient : IOpenAiClient
    {
        private readonly ChatClient _chat;

        public OpenAiClient(string apiKey, string model = "gpt-4o")
        {
            _chat = new ChatClient(model, apiKey);
        }

        public async Task<string> GerarAnaliseAsync(string prompt)
        {
            var messages = new ChatMessage[]
            {
                ChatMessage.CreateSystemMessage("Você é um analista que gera insights estratégicos sobre simulações de produtos."),
                ChatMessage.CreateUserMessage(prompt)
            };

            var completion = await _chat.CompleteChatAsync(messages);

            return completion.Value.Content[0].Text;
        }
    }
}
