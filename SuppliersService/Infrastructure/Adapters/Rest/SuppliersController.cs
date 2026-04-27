using System;
using System.Threading.Tasks;
using Aplication.Ports.In;
using Domain.Builders;
using Domain.Exceptions;
using Infrastructure.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Adapters.Rest
{
    /// <summary>
    /// Controlador REST para gestionar el ciclo de vida de los Proveedores.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierUseCasePort _useCase;

        public SuppliersController(ISupplierUseCasePort useCase)
        {
            _useCase = useCase;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupplierRequest request)
        {
            var supplier = new SupplierBuilder()
                .WithName(request.Name)
                .WithContactName(request.ContactName)
                .WithEmail(request.Email)
                .WithPhone(request.Phone)
                .WithDeliveryDays(request.DeliveryDays)
                .Build();

            var result = await _useCase.CreateAsync(supplier);
            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _useCase.GetByIdAsync(id);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _useCase.GetAllAsync();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSupplierRequest request)
        {
            var supplier = new SupplierBuilder()
                .WithName(request.Name)
                .WithContactName(request.ContactName)
                .WithEmail(request.Email)
                .WithPhone(request.Phone)
                .WithDeliveryDays(request.DeliveryDays)
                .Build();

            var result = await _useCase.UpdateAsync(id, supplier);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _useCase.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/products")]
        public async Task<IActionResult> AddProduct(Guid id, [FromBody] AddProductToSupplierRequest request)
        {
            var supplierProduct = new SupplierProductBuilder()
                .WithExternalProductId(request.ExternalProductId)
                .WithProductName(request.ProductName)
                .WithUnitPrice(request.UnitPrice)
                .WithIsAvailable(request.IsAvailable)
                .Build();

            var result = await _useCase.AddProductAsync(id, supplierProduct);
            if (result is null) return NotFound("Supplier not found");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene los proveedores disponibles para un producto externo. 
        /// Invocado por StockPro cuando un producto requiere reabastecimiento.
        /// </summary>
        [HttpGet("by-product/{productId}")]
        public async Task<IActionResult> GetAvailableByProduct(Guid productId)
        {
            var result = await _useCase.GetAvailableByProductAsync(productId);
            return Ok(new { ProductId = productId, SuppliersAvailable = result });
        }
    }
}
