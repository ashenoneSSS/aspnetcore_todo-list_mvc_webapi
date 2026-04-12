namespace TodoListApp.WebApp.Exceptions;

public class ApiUnavailableException : Exception
{
    public ApiUnavailableException()
        : base("The Todo List API is not running or not reachable.")
    {
    }

    public ApiUnavailableException(string message)
        : base(message)
    {
    }

    public ApiUnavailableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
