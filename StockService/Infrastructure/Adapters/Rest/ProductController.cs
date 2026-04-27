using Aplication.Ports.In;
using Domain.Builders;
using Domain.Exceptions;
using Infrastructure.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Adapters.Rest
{
    /// <summary>
    ///     This class represents the endpoint for the product.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductUseCasePort _useCase;

        public ProductController(IProductUseCasePort useCase)
        {
            _useCase = useCase;
        }

        /// <summary>
        ///     This method creates a new product.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            var product = new ProductBuilder()
                .WithName(request.Name)
                .WithDescripcion(request.Descripcion)
                .WithStock(request.Stock)
                .WithStockminimum(request.Stockminimum)
                .WithPrice(request.Price)
                .Build();

            var result = await _useCase.CreateAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }

        /// <summary>
        ///     This method gets a product by id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _useCase.GetByIdAsync(id);
            if (result is null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        ///     This method gets all products.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _useCase.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        ///     This method updates a product by id.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
        {
            var product = new ProductBuilder()
                .WithName(request.Name)
                .WithDescripcion(request.Descripcion)
                .WithStock(request.Stock)
                .WithStockminimum(request.Stockminimum)
                .WithPrice(request.Price)
                .Build();

            var result = await _useCase.UpdateAsync(id, product);
            if (result is null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        ///     This method deletes a product by id.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _useCase.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        /// <summary>
        ///     This method registers an exit of a product.
        /// </summary>
        [HttpPatch("{id}/exit/{quantity}")]
        public async Task<IActionResult> RegisterExit(Guid id, int quantity)
        {
            try
            {
                var result = await _useCase.RegisterExitAsync(id, quantity);
                if (result is null) return NotFound();
                return Ok(result);
            }
            catch (DomainException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
