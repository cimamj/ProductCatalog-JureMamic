namespace ProductCatalog.Domain.Exceptions
{
    public class ExternalServiceUnavailableException : Exception
    {
        public ExternalServiceUnavailableException(string message, Exception innerException)
       : base(message, innerException)
        {
        }
    }
}
