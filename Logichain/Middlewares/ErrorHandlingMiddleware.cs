using System.Net;
using Models;
using Newtonsoft.Json;

namespace Logichain.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.Log(LogLevel.Error, exception, exception.Message);
        context.Response.ContentType = "application/json";
        var statusCode = (int)HttpStatusCode.InternalServerError;
        var errorMessage = "An error has occurred. Please try again later.";

        if (exception is UserException userException)
        {
            if (MapErrorCodes.TryGetValue((ErrorCodes)userException.ErrorCode, out var returnStatusCode))
            {
                statusCode = (int)returnStatusCode;
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
            }

            errorMessage = userException.Message;
        }

        context.Response.StatusCode = statusCode;
        var result = JsonConvert.SerializeObject(new { error = errorMessage });
        return context.Response.WriteAsync(result);
    }

    private static readonly Dictionary<ErrorCodes, HttpStatusCode> MapErrorCodes = new()
    {
        // Generic
        { ErrorCodes.Unauthorized, HttpStatusCode.Unauthorized },
        { ErrorCodes.Forbidden, HttpStatusCode.Forbidden },
        { ErrorCodes.NotFound, HttpStatusCode.NotFound },
        { ErrorCodes.Conflict, HttpStatusCode.Conflict },


        // Bad Requests
        { ErrorCodes.BadRequest, HttpStatusCode.BadRequest },
        { ErrorCodes.ParentLocationNotFound, HttpStatusCode.BadRequest },

        // Internal Server Error
        { ErrorCodes.InternalServerError, HttpStatusCode.InternalServerError },
        { ErrorCodes.DatabaseConnectionError, HttpStatusCode.InternalServerError }
    };
}