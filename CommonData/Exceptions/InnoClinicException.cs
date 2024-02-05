namespace CommonData.Exceptions
{
    public class InnoClinicException : Exception
    {
        public int? StatusCode = null;

        public InnoClinicException(string? message, int? statusCode = default) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
