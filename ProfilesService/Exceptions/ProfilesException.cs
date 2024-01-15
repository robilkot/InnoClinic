namespace ProfilesService.Exceptions
{
    public class ProfilesException : Exception
    {
        public int? StatusCode = null;

        public ProfilesException(string? message, int? statusCode = default) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
