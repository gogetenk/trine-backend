using System;
using System.Net;
using Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.WebApi.Handlers
{
    /// <summary>
    /// This handler is an exception filter that handles explicit json response for the client.
    /// </summary>
    public class ExceptionHandler : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;
        private const string _DalError = "Une erreur est survenue avec un service externe.";
        private const string _BusinessError = "Une erreur métier est survenue.";
        private const string _FatalError = "Une erreur non prévue est survenue.";

        /// <summary>
        /// Build a new instance of <see cref="ExceptionHandler"/>.
        /// </summary>
        /// <param name="logger"></param>
        public ExceptionHandler(ILogger<ExceptionHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get access to exception context to log and enrich the response.
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case DalException dalException:
                    _logger.LogError(dalException, _DalError);
                    context.Result = CreateErrorResponse(_DalError);
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;

                case BusinessException businessException:
                    _logger.LogError(businessException, _BusinessError);
                    context.Result = CreateErrorResponse($"{_BusinessError}{Environment.NewLine} {businessException.Message}");
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                default:
                    _logger.LogCritical(context.Exception, _FatalError);
                    context.Result = CreateErrorResponse(_FatalError);
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            base.OnException(context);
        }

        private JsonResult CreateErrorResponse(string errorMessage)
        {
            var responseDto = new ErrorResponseDto
            {
                ErrorMessage = errorMessage
            };
            return new JsonResult(responseDto);
        }
    }
}
