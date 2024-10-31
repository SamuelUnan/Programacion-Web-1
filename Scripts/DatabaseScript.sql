Create DataBase DbExample

Use DbExample

create table CatMateria(
	Materia_Id int Primary Key Identity(1,1),
	Materia_Nombre varchar(50) Not Null,
	Materia_State bit Default 1
)

CREATE TABLE TblFactura (
    Factura_Id INT PRIMARY KEY IDENTITY(1,1),
    Factura_Cliente NVARCHAR(100),
    Factura_Fecha DATETIME
);

CREATE TYPE TDetalleFactura AS TABLE (
    Detalle_Nombre NVARCHAR(100),
    Detalle_Cantidad INT,
    Detalle_Precio  DECIMAL(18, 2)
);

CREATE TABLE TblDetalleFactura (
    Detalle_Id INT PRIMARY KEY IDENTITY(1,1),
    Factura_Id INT FOREIGN KEY REFERENCES TblFactura(Factura_Id),
    Detalle_Nombre NVARCHAR(100),
    Detalle_Cantidad INT,
    Detalle_Precio DECIMAL(18, 2)
);

CREATE PROCEDURE sp_CrearFactura
    @Factura_Cliente NVARCHAR(30),
    @Factura_Fecha DATETIME,
    @Detalles TDetalleFactura READONLY 
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO TblFactura (Factura_Fecha, Factura_Cliente)
        VALUES (@Factura_Fecha, @Factura_Cliente);

        SET @FacturaId = SCOPE_IDENTITY();

         INSERT INTO TblDetalleFactura (Factura_Id, Detalle_Nombre, Detalle_Cantidad, Detalle_Precio)
        SELECT @FacturaId, Detalle_Nombre, Detalle_Cantidad, Detalle_Precio
        FROM @Detalles;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

Select * From CatMateria
Select * From TblFactura
Select * From TblDetalleFactura