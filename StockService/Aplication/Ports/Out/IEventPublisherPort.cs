using Domain.Events;

namespace Aplication.Ports.Out
{
    /// <summary>
    ///     Port interface for publishing product events to a message broker.
    /// </summary>
    public interface IEventPublisherPort
    {
        /// <summary>
        ///     This method publishes a product event to the message brokeer.
        /// </summary>
        /// <param name="productEvent"></param>
        /// <returns></returns>
        Task PublishAsync(ProductEvent productEvent);
    }
}