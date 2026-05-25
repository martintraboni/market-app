# Market App

Sistema de gestión para minimarket desarrollado en C# (.NET 8, WinForms).


## Requisitos

- **Windows 10/11**
- **.NET 8 SDK** ([descargar](https://dotnet.microsoft.com/download))
- **SQL Server** (Express, LocalDB o instancia propia)
- **SQL Server Management Studio (SSMS)** o **Azure Data Studio** (opcional, para administrar la base de datos)
- **Visual Studio 2022** (opcional, recomendado para desarrollo)

## Instalación y primer uso

1. **Clona el repositorio:**
   ```
   git clone https://github.com/martintraboni/market-app.git
   ```

2. **Prepara la base de datos:**
   - La base de datos se crea y actualiza automáticamente usando **Entity Framework Core** y **migrations**.
   - No es necesario ejecutar scripts SQL manualmente.
   - Para aplicar las migraciones y crear la base de datos, ejecuta:
     ```
     dotnet ef database update
     ```
   - Se crea un usuario de prueba: **admin / admin123** (si la migración lo incluye).

3. **Configura la cadena de conexión:**
   - Edita `src/Config.cs` y ajusta la propiedad `ConnectionString` según tu entorno:
     - LocalDB: `Server=(localdb)\\MSSQLLocalDB;Database=MinimarketDB;Trusted_Connection=True;`
     - SQL Express: `Server=.\\SQLEXPRESS;Database=MinimarketDB;Trusted_Connection=True;TrustServerCertificate=True;`
     - Servidor propio: `Server=localhost;Database=MinimarketDB;Trusted_Connection=True;TrustServerCertificate=True;`

4. **Compila y ejecuta el proyecto:**
   - Desde la terminal, navega a la carpeta `src` y ejecuta:
     ```
     dotnet build
     dotnet run
     ```
   - O abre el proyecto en Visual Studio y presiona F5.

## Uso

- Al iniciar la aplicación, aparecerá el formulario de login.
- Ingresa el usuario y contraseña (por defecto: **admin / admin123**).
- Si el login es exitoso, accederás al sistema principal.
- El sistema permite gestionar productos, ventas y reportes.

## Estructura del proyecto

- `src/Models/` — Clases de entidades (Producto, Venta, Usuario, etc.)
- `src/Data/` — Acceso a datos y repositorios
- `src/UI/` — Formularios de la interfaz gráfica (WinForms)
- `src/Exceptions/` — Excepciones personalizadas
- `database.sql` — Script para crear la base de datos y datos iniciales
- `.gitignore` — Exclusiones para control de versiones

## Notas y recomendaciones

- La gestión de la base de datos ahora se realiza con **Entity Framework Core** y migraciones.
- Puedes crear más usuarios desde la aplicación o mediante migraciones personalizadas.
- Para producción, se recomienda encriptar las contraseñas y mejorar la seguridad.
- Si tienes problemas de conexión, revisa la cadena de conexión y que la base de datos esté creada y accesible.

## Licencia

Este proyecto es de uso académico y personal. Puedes modificarlo y adaptarlo según tus necesidades.
