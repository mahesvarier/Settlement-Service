namespace Settlement_Service.Models.Exceptions
{
    public class BookingConflictException : Exception
    {
        public string ErrorCode { get; }

        public BookingConflictException(string errorCode) : base(errorCode)
        {
            ErrorCode = errorCode;
        }
    }
}
