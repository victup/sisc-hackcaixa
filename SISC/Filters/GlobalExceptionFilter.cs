using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SISC.Exceptions;
using SISC.Models.Errors;
using System.Net;

namespace SISC.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var redirectUrl = (string?)null;

            if (context.Exception is ArgumentException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
            }
            else if (context.Exception is KeyNotFoundException)
            {
                statusCode = (int)HttpStatusCode.NotFound;
            }
            else if (context.Exception is NegocioException negocioEx)
            {
                statusCode = negocioEx.StatusCode;
                redirectUrl = negocioEx.RedirectUrl;
            }

            _logger.LogError(context.Exception, "Erro tratado pelo GlobalExceptionFilter");

            var errorResponse = new ErrorResponse
            {
                Timestamp = DateTime.UtcNow,
                StatusCode = statusCode,
                Message = context.Exception.Message,
                RedirectUrl = redirectUrl
            };

            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = statusCode
            };

            context.ExceptionHandled = true;
        }
    }
}
