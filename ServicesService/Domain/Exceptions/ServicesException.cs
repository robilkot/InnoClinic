namespace ServicesService.Domain.Exceptions
{
    public class ServicesException : Exception
    {
        public int? StatusCode = null;

        public ServicesException(string? message, int? statusCode = default) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
