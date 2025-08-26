namespace SISC.Integrations
{
    public interface IOpenAiClient
    {
        Task<string> GerarAnaliseAsync(string prompt);
    }
}
