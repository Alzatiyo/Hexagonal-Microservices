namespace Domain.Enums
{
    /// <summary>
    /// Estados posibles en el ciclo de vida de un proveedor.
    /// </summary>
    public enum SupplierStatus
    {
        /// <summary>
        /// El proveedor está habilitado para recibir pedidos.
        /// </summary>
        Activo = 1,

        /// <summary>
        /// El proveedor ha sido deshabilitado y no aparecerá en las recomendaciones de reabastecimiento.
        /// </summary>
        Inactivo = 2
    }
}
