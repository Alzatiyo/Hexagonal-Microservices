using Domain.Events;

namespace Aplication.Ports.Out
{
    /// <summary>
    ///     Puerto de salida para publicar solicitudes de reabastecimiento
    ///     hacia SuppliersMS a través del message broker.
    /// </summary>
    public interface IRestockPublisherPort
    {
        Task PublishRestockRequestAsync(RestockRequestEvent restockEvent);
    }
}