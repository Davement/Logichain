namespace Models;

public enum ErrorCodes
{
    // Generic
    BadRequest = 400,
    NotAuthenticated = 401,
    NotAuthorized = 403,
    NotFound = 404,
    Conflict = 409,
    InternalServerError = 500,

    // Bad Requests
    ParentLocationNotFound = 1
}

public static class GetReturnError
{
    public static readonly Dictionary<ErrorCodes, ErrorCodes> MappedErrors = new()
    {
        { ErrorCodes.ParentLocationNotFound, ErrorCodes.BadRequest },
    };
}