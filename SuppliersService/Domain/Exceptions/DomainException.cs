using System;

namespace Domain.Exceptions
{
    /// <summary>
    /// Excepción personalizada para validaciones y reglas de negocio rotas en la capa de Dominio.
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException()
        {
        }

        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
