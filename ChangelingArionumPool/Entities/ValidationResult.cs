using System;
namespace ChangelingArionumPool.Entities
{
    public class ValidationResult
    {
        public ValidationStatus Status;
        public string Message;
    }

    public enum ValidationStatus
    {
        Success = 0,
        Error = 1,
        Abuser = 2,
        SyntaxError = 3,
        UnexpectedError = 4
    }
}
