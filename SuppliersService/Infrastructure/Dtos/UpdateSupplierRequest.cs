namespace Infrastructure.Dtos
{
    /// <summary>
    /// DTO para la actualización de un proveedor existente.
    /// </summary>
    public class UpdateSupplierRequest
    {
        public string Name { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int DeliveryDays { get; set; }
    }
}
