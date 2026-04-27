using Domain.Models;

namespace Domain.Builders
{
    /// <summary>
    /// Patrón Builder para construir la entidad asociativa SupplierProduct.
    /// </summary>
    public class SupplierProductBuilder
    {
        private Guid _id;
        private Guid _supplierId;
        private Guid _externalProductId;
        private string _productName = string.Empty;
        private decimal _unitPrice;
        private bool _isAvailable;
        private DateTime _createdAt;
        private DateTime _updateAt;

        public SupplierProductBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public SupplierProductBuilder WithSupplierId(Guid supplierId)
        {
            _supplierId = supplierId;
            return this;
        }

        public SupplierProductBuilder WithExternalProductId(Guid externalProductId)
        {
            _externalProductId = externalProductId;
            return this;
        }

        public SupplierProductBuilder WithProductName(string productName)
        {
            _productName = productName;
            return this;
        }

        public SupplierProductBuilder WithUnitPrice(decimal unitPrice)
        {
            _unitPrice = unitPrice;
            return this;
        }

        public SupplierProductBuilder WithIsAvailable(bool isAvailable)
        {
            _isAvailable = isAvailable;
            return this;
        }

        public SupplierProductBuilder WithCreatedAt(DateTime createdAt)
        {
            _createdAt = createdAt;
            return this;
        }

        public SupplierProductBuilder WithUpdateAt(DateTime updateAt)
        {
            _updateAt = updateAt;
            return this;
        }

        public SupplierProduct Build()
        {
            return new()
            {
                Id = _id,
                SupplierId = _supplierId,
                ExternalProductId = _externalProductId,
                ProductName = _productName,
                UnitPrice = _unitPrice,
                IsAvailable = _isAvailable,
                CreatedAt = _createdAt,
                UpdateAt = _updateAt
            };
        }
    }
}
