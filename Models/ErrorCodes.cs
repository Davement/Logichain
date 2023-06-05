namespace Models;

public enum ErrorCodes
{
    // Generic
    Forbidden = 1001,
    Unauthorized = 1002,
    NotFound = 1003,
    Conflict = 1004,
    
    // Bad Requests
    BadRequest = 1000,
   
    ParentLocationNotFound = 1005,

    // Internal Server Error
    InternalServerError = 2000,
    DatabaseConnectionError = 2001
}