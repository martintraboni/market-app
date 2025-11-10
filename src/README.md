# 🛒 Minimarket PPV 2025

Sistema de Punto de Venta (POS) completo para minimercados, desarrollado en C# con Windows Forms y Entity Framework Core.

## 📋 Descripción

**Minimarket PPV** es un sistema integral de gestión para pequeños comercios que incluye:
- Punto de venta con generación automática de tickets PDF
- Gestión completa de inventario y productos
- Control de compras y proveedores
- Administración de caja con cierres diarios
- Sistema de reportes avanzados con exportación
- Control de usuarios y auditoría

## 🚀 Características Principales

### 🏪 Punto de Venta
- Interfaz intuitiva para registro rápido de ventas
- Búsqueda de productos por código o nombre
- Múltiples métodos de pago (Efectivo, Tarjeta Débito, Tarjeta Crédito, Transferencia)
- **Generación automática de tickets PDF** (formato A7)
- Historial completo de ventas con filtros

### 📦 Gestión de Inventario
- CRUD completo de productos
- Organización por categorías
- Control de stock con alertas de bajo stock
- Registro de movimientos de inventario (Entrada/Salida/Ajuste)
- Reporte de productos con bajo stock exportable a PDF

### 🛒 Gestión de Compras
- Registro de compras a proveedores
- Gestión de proveedores (CUIT, contacto, saldo)
- Vinculación automática con inventario
- Historial de compras por proveedor

### 💰 Control de Caja
- Registro de movimientos de caja (Ingresos/Egresos)
- **Cierre de caja diario con generación de acta en PDF** (formato A4)
- Comparación de valores teóricos vs reales
- Historial de cierres con notas

### 📊 Reportes y Exportación
- **Reporte de ventas** (rango de fechas) → Exportación CSV
- **Ventas por empleado** → Exportación CSV
- **Top productos más vendidos** → Exportación CSV
- **Compras por proveedor** → Exportación CSV
- **Resumen de caja** → Exportación CSV
- **Productos con bajo stock** → Exportación PDF

### 👥 Gestión de Usuarios
- Sistema de roles: **Admin**, **Supervisor**, **Usuario**
- Control de acceso por funcionalidad
- Auditoría completa de operaciones
- Gestión de usuarios (solo Admin)

## 🛠️ Tecnologías Utilizadas

| Tecnología | Versión | Propósito |
|-----------|---------|-----------|
| **.NET** | 8.0 | Framework principal |
| **Windows Forms** | - | Interfaz gráfica |
| **Entity Framework Core** | 9.0.9 | ORM para base de datos |
| **SQL Server** | 2019+ | Base de datos |
| **iText7** | 8.0.5 | Generación de PDFs |
| **BouncyCastle** | 8.0.5 | Soporte criptográfico para PDFs |

## 📁 Estructura del Proyecto

```
Minimarket-PPV-2025/
└── src/
    ├── Config.cs                    # Configuración de conexión
    ├── Program.cs                   # Punto de entrada
    ├── Minimarket.csproj            # Archivo del proyecto
    │
    ├── Constants/
    │   └── Constants.cs             # Códigos de roles y constantes
    │
    ├── Data/                        # Capa de acceso a datos
    │   ├── Db.cs                    # DbContext de EF Core
    │   ├── CategoryRepository.cs
    │   ├── ProductRepository.cs
    │   ├── SupplierRepository.cs
    │   ├── PurchaseRepository.cs
    │   ├── SaleRepository.cs
    │   ├── InventoryMovementRepository.cs
    │   └── UserRepository.cs
    │
    ├── DTOs/                        # Data Transfer Objects
    │   ├── ProductListDto.cs
    │   ├── SaleListDto.cs
    │   ├── PurchaseListDto.cs
    │   └── ...
    │
    ├── Exceptions/
    │   └── LoginExceptions.cs       # Excepciones personalizadas
    │
    ├── Helpers/
    │   └── PdfHelper.cs             # Generación de PDFs
    │
    ├── Migrations/                  # Migraciones de EF Core
    │   ├── 20251101161723_InitialStructure.cs
    │   ├── 20251101164127_Seed_Initial_User.cs
    │   └── ...
    │
    ├── Models/                      # Modelos de dominio
    │   ├── User.cs
    │   ├── Product.cs
    │   ├── Category.cs
    │   ├── Supplier.cs
    │   ├── Purchase.cs
    │   ├── Sale.cs
    │   ├── CashMovement.cs
    │   ├── CashClose.cs
    │   ├── InventoryMovement.cs
    │   └── AuditLog.cs
    │
    └── UI/                          # Interfaces de usuario
        ├── LoginForm.cs             # Pantalla de login
        ├── MainForm.cs              # Menú principal
        ├── ProductsForm.cs          # Gestión de productos
        ├── CategoriesForm.cs        # Gestión de categorías
        ├── SuppliersForm.cs         # Gestión de proveedores
        ├── PurchasesForm.cs         # Gestión de compras
        ├── SalesForm.cs             # Punto de venta
        ├── SalesListForm.cs         # Historial de ventas
        ├── CashMovementsForm.cs     # Movimientos de caja
        ├── CashCloseForm.cs         # Cierre de caja
        ├── InventoryMovementsForm.cs
        ├── ReportsForm.cs           # Reportes de ventas
        ├── AdvancedReportsForm.cs   # Reportes avanzados
        ├── LowStockReportForm.cs    # Reporte de bajo stock
        ├── UsersForm.cs             # Gestión de usuarios
        └── AuditLogsForm.cs         # Auditoría
```

## ⚙️ Instalación y Configuración

### Prerequisitos

- **Windows 10/11**
- **.NET 8.0 SDK** o superior ([Descargar](https://dotnet.microsoft.com/download))
- **SQL Server 2019+** o **SQL Server Express** ([Descargar](https://www.microsoft.com/sql-server/sql-server-downloads))
- **Visual Studio 2022** (opcional, recomendado)

### Pasos de Instalación

1. **Clonar el repositorio**
```bash
git clone https://github.com/tu-usuario/minimarket-ppv-2025.git
cd minimarket-ppv-2025/src
```

2. **Configurar la cadena de conexión**

Editar `Config.cs` según tu entorno:

```csharp
// Opción 1: LocalDB (Visual Studio)
public static string ConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=MinimarketDB;Trusted_Connection=True;";

// Opción 2: SQL Server Express
public static string ConnectionString = @"Server=.\SQLEXPRESS;Database=MinimarketDB;Trusted_Connection=True;TrustServerCertificate=True;";

// Opción 3: Servidor local (por defecto)
public static string ConnectionString = "Server=localhost,1433;Database=MinimarketDB;Trusted_Connection=True;TrustServerCertificate=True;";
```

3. **Restaurar paquetes NuGet**
```bash
dotnet restore
```

4. **Aplicar migraciones a la base de datos**
```bash
dotnet ef database update
```

Esto creará:
- La base de datos `MinimarketDB`
- Todas las tablas necesarias
- **Usuario inicial**: `admin` / `admin123` (rol: Administrador)
- Datos de prueba (productos, categorías, proveedores, etc.)

5. **Compilar el proyecto**
```bash
dotnet build
```

6. **Ejecutar la aplicación**
```bash
dotnet run
```

## 🔑 Credenciales Iniciales

Después de aplicar las migraciones, se crea un usuario administrador:

- **Usuario**: `admin`
- **Contraseña**: `admin123`
- **Rol**: Administrador

## 📂 Carpetas de Exportación

El sistema crea automáticamente las siguientes carpetas en el directorio de ejecución:

- `Tickets/` - Tickets de venta en formato PDF (A7)
- `Cierres/` - Actas de cierre de caja en PDF (A4)
- `Reportes/` - Reportes exportados en PDF/CSV

## 👥 Sistema de Roles

### 🔴 Administrador
- **Acceso total** al sistema
- Gestión de usuarios
- Auditoría completa
- Todas las funciones de Supervisor y Usuario

### 🟡 Supervisor
- Gestión de productos, categorías y proveedores
- Gestión de compras
- Registro de ventas
- **Movimientos de caja y cierres**
- Acceso a todos los reportes

### 🟢 Usuario
- Gestión de productos y categorías
- Registro de ventas
- Acceso a reportes básicos
- **Sin acceso** a caja ni proveedores

## 🗺️ Navegación del Sistema

```
Menú Principal
├── Productos
│   ├── Gestión de Productos
│   ├── Categorías
│   └── Movimientos de Inventario
│
├── Compras
│   ├── Gestión de Compras
│   └── Proveedores (Supervisor/Admin)
│
├── Ventas
│   ├── Nueva Venta (Genera ticket PDF)
│   └── Listado de Ventas
│
├── Caja (Supervisor/Admin)
│   ├── Movimientos de Caja
│   └── Cierre de Caja (Genera acta PDF)
│
├── Reportes
│   ├── Reporte de Ventas (CSV)
│   ├── Bajo Stock (PDF)
│   └── Reportes Avanzados (CSV)
│       ├── Ventas por Empleado
│       ├── Top Productos
│       ├── Compras por Proveedor
│       └── Resumen de Caja
│
└── Gestión Admin (Solo Admin)
    ├── Usuarios
    └── Auditoría
```

## 📄 Características de los PDFs

### Tickets de Venta (A7 - 74x105mm)
- Generación automática después de cada venta
- Información del negocio (configurable)
- Detalle de productos
- Método de pago
- Total y cambio
- Formato compacto para impresora térmica

### Actas de Cierre de Caja (A4)
- Generación automática al cerrar caja
- Fecha y hora del cierre
- Usuario responsable
- Detalle de movimientos del día
- Comparación teórico vs real
- Diferencias destacadas
- Sección de observaciones
- Espacio para firmas

### Reportes de Bajo Stock (A4)
- Exportación manual desde el formulario
- Listado completo de productos
- Stock actual vs stock mínimo
- Identificación visual de productos críticos
- Agrupado por categoría

## 🔧 Comandos Útiles

### Crear una nueva migración
```bash
dotnet ef migrations add NombreDeLaMigracion
```

### Actualizar la base de datos
```bash
dotnet ef database update
```

### Revertir a una migración específica
```bash
dotnet ef database update NombreDeLaMigracion
```

### Eliminar la última migración (sin aplicar)
```bash
dotnet ef migrations remove
```

### Generar script SQL de las migraciones
```bash
dotnet ef migrations script
```

## 🐛 Solución de Problemas

### Error de conexión a SQL Server
- Verificar que SQL Server esté corriendo
- Comprobar la cadena de conexión en `Config.cs`
- Asegurar que el puerto 1433 esté abierto
- Verificar permisos de Windows Authentication

### Error "Font for Unicode characters required"
- Ya resuelto: el sistema usa caracteres ASCII
- Si aparece: verificar versiones de iText7 (8.0.5) y bouncy-castle-adapter (8.0.5)

### Base de datos no se crea
```bash
dotnet ef database drop  # Eliminar BD existente
dotnet ef database update  # Recrear desde cero
```

### Error al generar PDFs
- Verificar que las carpetas `Tickets/`, `Cierres/`, `Reportes/` tengan permisos de escritura
- Revisar que no haya procesos bloqueando archivos PDF

## 📝 Modelo de Datos

### Entidades Principales
- **User**: Usuarios del sistema con roles
- **Product**: Productos con stock y precios
- **Category**: Categorías de productos
- **Supplier**: Proveedores con información de contacto
- **Purchase / PurchaseItem**: Compras y sus detalles
- **Sale / SaleItem**: Ventas y sus detalles
- **CashMovement**: Movimientos de caja
- **CashClose**: Cierres de caja diarios
- **InventoryMovement**: Movimientos de inventario
- **AuditLog**: Registro de auditoría

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor:

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📜 Licencia

Este proyecto fue desarrollado como proyecto de tesis para fines educativos.

## 👨‍💻 Autor

Desarrollado por [Tu Nombre] - 2025

## 📞 Contacto

- Email: tu-email@ejemplo.com
- GitHub: [@tu-usuario](https://github.com/tu-usuario)

---

⭐ Si este proyecto te fue útil, no olvides darle una estrella en GitHub
