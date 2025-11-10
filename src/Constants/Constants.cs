namespace Minimarket
{
    public static class Constants
    {
        public const string InventoryMovementTypeIngreso = "Compra";
        public const string InventoryMovementTypeEgreso = "Venta";
        public const string InventoryMovementTypeAjuste = "Mov. Manual";
        
        public const string CashMovementTypeIn = "Ingreso";
        public const string CashMovementTypeOut = "Egreso";

        public const string RoleCodeAdmin = "Admin";
        public const string RoleCodeSupervisor = "Supervisor";
        public const string RoleCodeUser = "User";

        // Eventos de auditoría
        public const string AuditEventCreateUser = "Crear Usuario";
        public const string AuditEventUpdateUser = "Actualizar Usuario";
        public const string AuditEventDeleteUser = "Eliminar Usuario";
        public const string AuditEventToggleUserStatus = "Cambiar Estado Usuario";
        public const string AuditEventCreatePurchase = "Crear Compra";
        public const string AuditEventCreateSale = "Crear Venta";
        public const string AuditEventCreateProduct = "Crear Producto";
        public const string AuditEventUpdateProduct = "Actualizar Producto";
        public const string AuditEventDeleteProduct = "Eliminar Producto";
        public const string AuditEventCashClose = "Cierre de Caja";
        
        // Eventos de auditoría avanzados (control de acceso y operaciones críticas)
        public const string AuditEventPriceChange = "Cambio de Precio";
        public const string AuditEventStockAdjustment = "Ajuste de Stock";
        public const string AuditEventSaleCancellation = "Anulación de Venta";
        public const string AuditEventAccessDenied = "Acceso Denegado";
    }
}
