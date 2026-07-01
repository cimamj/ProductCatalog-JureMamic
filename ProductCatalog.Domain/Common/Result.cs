using ProductCatalog.Domain.Enums;

namespace ProductCatalog.Domain.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public ErrorCode? ErrorCode { get; }
        public string? ErrorMessage { get; }

        private Result(T value)
        {
            IsSuccess = true;
            Value = value;
        }

        private Result(ErrorCode errorCode, string errorMessage)
        {
            IsSuccess = false;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(ErrorCode errorCode, string errorMessage) => new(errorCode, errorMessage);
    }
}
