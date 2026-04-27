using Domain.Enums;
using Domain.Models;

namespace Domain.Builders
{
    /// <summary>
    /// Patrón Builder para construir instancias de Supplier de forma fluida y segura.
    /// </summary>
    public class SupplierBuilder
    {
        private Guid _id;
        private string _name = string.Empty;
        private string _contactName = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private int _deliveryDays;
        private SupplierStatus _status;
        private DateTime _createdAt;
        private DateTime _updateAt;
        private ICollection<SupplierProduct> _products = new List<SupplierProduct>();

        public SupplierBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public SupplierBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public SupplierBuilder WithContactName(string contactName)
        {
            _contactName = contactName;
            return this;
        }

        public SupplierBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public SupplierBuilder WithPhone(string phone)
        {
            _phone = phone;
            return this;
        }

        public SupplierBuilder WithDeliveryDays(int deliveryDays)
        {
            _deliveryDays = deliveryDays;
            return this;
        }

        public SupplierBuilder WithStatus(SupplierStatus status)
        {
            _status = status;
            return this;
        }

        public SupplierBuilder WithProducts(ICollection<SupplierProduct> products)
        {
            _products = products;
            return this;
        }

        public SupplierBuilder WithCreatedAt(DateTime createdAt)
        {
            _createdAt = createdAt;
            return this;
        }

        public SupplierBuilder WithUpdateAt(DateTime updateAt)
        {
            _updateAt = updateAt;
            return this;
        }

        /// <summary>
        /// Genera la instancia final de Supplier con los valores configurados.
        /// </summary>
        public Supplier Build()
        {
            return new()
            {
                Id = _id,
                Name = _name,
                ContactName = _contactName,
                Email = _email,
                Phone = _phone,
                DeliveryDays = _deliveryDays,
                Status = _status,
                Products = _products,
                CreatedAt = _createdAt,
                UpdateAt = _updateAt
            };
        }
    }
}
