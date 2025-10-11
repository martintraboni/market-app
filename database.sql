-- Script de creación de base de datos y tablas
CREATE DATABASE MinimarketDB;
GO

USE MinimarketDB;
GO

IF OBJECT_ID('dbo.DetalleVentas', 'U') IS NOT NULL DROP TABLE dbo.DetalleVentas;
IF OBJECT_ID('dbo.Ventas', 'U') IS NOT NULL DROP TABLE dbo.Ventas;
IF OBJECT_ID('dbo.Productos', 'U') IS NOT NULL DROP TABLE dbo.Productos;
IF OBJECT_ID('dbo.Caja', 'U') IS NOT NULL DROP TABLE dbo.Caja;
IF OBJECT_ID('dbo.Usuarios', 'U') IS NOT NULL DROP TABLE dbo.Usuarios;

CREATE TABLE Productos (
    IDProducto INT IDENTITY PRIMARY KEY,
    Codigo NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion NVARCHAR(150) NOT NULL,
    Categoria NVARCHAR(80) NOT NULL DEFAULT('General'),
    Precio DECIMAL(12,2) NOT NULL CHECK (Precio >= 0),
    Stock INT NOT NULL DEFAULT 0 CHECK (Stock >= 0),
    StockMin INT NOT NULL DEFAULT 0 CHECK (StockMin >= 0)
);

CREATE TABLE Ventas (
    IDVenta INT IDENTITY PRIMARY KEY,
    Fecha DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Total DECIMAL(12,2) NOT NULL CHECK (Total >= 0),
    MedioPago NVARCHAR(50) NOT NULL
);

CREATE TABLE DetalleVentas (
    IDDetalle INT IDENTITY PRIMARY KEY,
    IDVenta INT NOT NULL FOREIGN KEY REFERENCES Ventas(IDVenta),
    IDProducto INT NOT NULL FOREIGN KEY REFERENCES Productos(IDProducto),
    Cantidad INT NOT NULL CHECK (Cantidad > 0),
    PrecioUnitario DECIMAL(12,2) NOT NULL CHECK (PrecioUnitario >= 0)
);

-- Tabla de Caja (opcional, no utilizada directamente por el ejemplo)
CREATE TABLE Caja (
    IDCaja INT IDENTITY PRIMARY KEY,
    Fecha DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Ingreso DECIMAL(12,2) NULL,
    Egreso DECIMAL(12,2) NULL,
    Descripcion NVARCHAR(150)
);


CREATE TABLE Usuarios (
    IDUsuario INT IDENTITY PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Rol NVARCHAR(50) NOT NULL DEFAULT('Usuario')
);

-- Datos de ejemplo
INSERT INTO Productos (Codigo, Descripcion, Categoria, Precio, Stock, StockMin) VALUES
('0001','Gaseosa Cola 2L','Bebidas',2500,100,10),
('0002','Yerba Mate 1Kg','Alimentos',4200,40,5),
('0003','Lavandina 1L','Limpieza',1800,60,8);

-- Usuario de prueba: admin / admin123
INSERT INTO Usuarios (Username, Password, Nombre, Rol) VALUES ('admin', 'admin123', 'Administrador', 'Admin');