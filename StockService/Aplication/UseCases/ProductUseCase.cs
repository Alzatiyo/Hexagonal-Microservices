using Aplication.Ports.In;
using Aplication.Ports.Out;
using Domain.Models;
using Domain.Services;
using Domain.Events;

namespace Aplication.UseCases
{
    /// <summary>
    ///     This class implements the use case for managing products.
    /// </summary>
    public class ProductUseCase : IProductUseCasePort
    {
        private readonly IProductRepositoryPort _repository;
        private readonly ProductService _productService;
        private readonly IEventPublisherPort _eventPublisher;
        private readonly IRestockPublisherPort _restockPublisher;

        public ProductUseCase(
            IProductRepositoryPort repository,
            ProductService productService,
            IEventPublisherPort eventPublisher,
            IRestockPublisherPort restockPublisher)
        {
            _repository = repository;
            _productService = productService;
            _eventPublisher = eventPublisher;
            _restockPublisher = restockPublisher;
        }

        /// <summary>
        ///     This method creates a new product and saves it to the database.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task<Product?> CreateAsync(Product product)
        {
            product.Id = Guid.NewGuid();
            product.CreatedAt = DateTime.UtcNow;
            product.UpdateAt = DateTime.UtcNow; 

            _productService.EvaluateStockStatus(product);

            var saved = await _repository.SaveAsync(product);

            if (saved is not null)
                await _eventPublisher.PublishAsync(ProductEvent.Create(ProductAction.Created, saved));

            return saved;
        }

        /// <summary>
        ///     This method gets a product by id from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Product?> GetByIdAsync(Guid id) =>
            await _repository.GetByIdAsync(id);

        /// <summary>
        ///     This method gets all products from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _repository.GetAllAsync();

        /// <summary>
        ///     This method updates a product by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task<Product?> UpdateAsync(Guid id, Product product)
        {
            var existing = await _repository.GetByIdAsync(id);

            if (existing is null)
                return null;

            product.Id = id;
            product.UpdateAt = DateTime.UtcNow;

            _productService.EvaluateStockStatus(product);

            var updated = await _repository.UpdateAsync(product);

            if (updated is not null)
                await _eventPublisher.PublishAsync(ProductEvent.Create(ProductAction.Updated, updated));

            return updated;
        }

        /// <summary>
        ///     This method deletes a product by id from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _repository.GetByIdAsync(id);
            var result = await _repository.DeleteAsync(id);
 
            if (result && existing is not null)
                await _eventPublisher.PublishAsync(ProductEvent.Create(ProductAction.Deleted, existing));
 
            return result;
        }
 

        /// <summary>
        ///     This method registers an exit of a product.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<Product?> RegisterExitAsync(Guid id, int quantity)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product is null) return null;

            var updated = _productService.RegisterExit(quantity, product);
            var saved = await _repository.UpdateAsync(updated);

            // When stock hits the minimum threshold, notify SuppliersMS via RabbitMQ.
            if (saved is not null && saved.Stock <= saved.Stockminimum)
                await _restockPublisher.PublishRestockRequestAsync(
                    RestockRequestEvent.Create(saved.Id, saved.Name, saved.Stock, saved.Stockminimum));

            if (saved is not null)
                await _eventPublisher.PublishAsync(ProductEvent.Create(ProductAction.ExitRegistered, saved));

            return saved;
        }

    }
}
