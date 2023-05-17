namespace Models;

public class UserException : ArgumentException
{
    public int ErrorCode { get; }
    public UserException(ErrorCodes errorCode, string message): base(message)
    {
        ErrorCode = (int)errorCode;
    }
}