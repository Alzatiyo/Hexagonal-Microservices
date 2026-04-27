
namespace Domain.Exceptions
{
    /// <summary>
    ///     This class represents a domain exception.
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        ///     This constructor initializes a new instance of the DomainException class.
        /// </summary>
        /// <param name="message"></param>
        public DomainException(string message) : base(message){ }
    }
}