namespace Settlement_Service.Models.Common
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorCode { get; set; }

        public ValidationResult(bool isValid, string? errorCode = null)
        {
            IsValid = isValid;
            ErrorCode = isValid ? null : errorCode;
        }
    }
}
