namespace SISC.Exceptions
{
    public class NegocioException : Exception
    {
        public int StatusCode { get; }

        public string? RedirectUrl { get; }

        public NegocioException(string message, int statusCode = 400, string? redirectUrl = null)
            : base(message)
        {
            StatusCode = statusCode;
            RedirectUrl = redirectUrl;
        }
    }
}
