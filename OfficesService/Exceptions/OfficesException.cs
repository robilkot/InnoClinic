namespace OfficesService.Exceptions
{
    public class OfficesException : Exception
    {
        public int? StatusCode = null;

        public OfficesException(string? message, int? statusCode = default) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
