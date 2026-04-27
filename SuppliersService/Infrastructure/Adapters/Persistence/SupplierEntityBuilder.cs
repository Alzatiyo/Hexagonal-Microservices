using System;
using System.Collections.Generic;

namespace Infrastructure.Adapters.Persistence
{
    public class SupplierEntityBuilder
    {
        private Guid _id;
        private string _name = string.Empty;
        private string _contactName = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private int _deliveryDays;
        private bool _status;
        private DateTime _createdAt;
        private DateTime _updateAt;
        private ICollection<SupplierProductEntity> _products = new List<SupplierProductEntity>();

        public SupplierEntityBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public SupplierEntityBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public SupplierEntityBuilder WithContactName(string contactName)
        {
            _contactName = contactName;
            return this;
        }

        public SupplierEntityBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public SupplierEntityBuilder WithPhone(string phone)
        {
            _phone = phone;
            return this;
        }

        public SupplierEntityBuilder WithDeliveryDays(int deliveryDays)
        {
            _deliveryDays = deliveryDays;
            return this;
        }

        public SupplierEntityBuilder WithStatus(bool status)
        {
            _status = status;
            return this;
        }

        public SupplierEntityBuilder WithProducts(ICollection<SupplierProductEntity> products)
        {
            _products = products;
            return this;
        }

        public SupplierEntityBuilder WithCreatedAt(DateTime createdAt)
        {
            _createdAt = createdAt;
            return this;
        }

        public SupplierEntityBuilder WithUpdateAt(DateTime updateAt)
        {
            _updateAt = updateAt;
            return this;
        }

        public SupplierEntity Build() => new SupplierEntity
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
