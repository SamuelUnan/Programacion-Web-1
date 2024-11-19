using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WebApi.Interface;
using WebApi.Model;

namespace WebApi.Implementation;

public class FacturaService : IFacturaService
{
    private readonly IConfiguration _configuration;
    private string connectionString;
    public FacturaService(IConfiguration configuration)
    {
        _configuration = configuration;
        connectionString = _configuration.GetConnectionString("DatabaseConnection");
    }
    public Factura Add(Factura factura)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            try
            {
                using (SqlCommand command = new SqlCommand("sp_CrearFactura", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Factura_Cliente", factura.FacturaCliente);
                    command.Parameters.AddWithValue("@Factura_Fecha", factura.FacturaFecha);

                    DataTable detalleTable = new DataTable();
                    detalleTable.Columns.Add("Detalle_Nombre", typeof(string));
                    detalleTable.Columns.Add("Detalle_Cantidad", typeof(int));
                    detalleTable.Columns.Add("Detalle_Precio", typeof(decimal));

                    foreach (var detalle in factura.FacturaDetalle)
                    {
                        detalleTable.Rows.Add(detalle.DetalleProducto, detalle.DetalleCantidad, detalle.DetallePrecio);
                    }

                    SqlParameter detalleParameter = new SqlParameter("@Detalles", SqlDbType.Structured)
                    {
                        TypeName = "dbo.TDetalleFactura",
                        Value = detalleTable
                    };
                    command.Parameters.Add(detalleParameter);

                    command.ExecuteNonQuery();

                    return factura;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
    }

    public void Delete(int id)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var detalle = new SqlCommand(
                        "DELETE FROM TblDetalleFactura WHERE Factura_Id = @Id",
                        connection,
                        transaction);
                    detalle.Parameters.AddWithValue("@Id", id);
                    detalle.ExecuteNonQuery();

                    // Eliminar la orden
                    var factura = new SqlCommand(
                        "DELETE FROM Orders WHERE OrderId = @OrderId",
                        connection,
                        transaction);
                    factura.Parameters.AddWithValue("@Id", id);
                    factura.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    public IEnumerable<Factura> GetALL()
    {
        var facturas = new List<Factura>();
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqlCommand("Select * From TblFactura", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        facturas.Add(new Factura
                        {
                            FacturaId = (int)reader["Factura_Id"],
                            FacturaCliente = reader["Factura_Cliente"].ToString(),
                            FacturaFecha = (DateTime)reader["Factura_Fecha"],
                            FacturaDetalle = new List<DetalleFactura>()
                        });
                    }
                }
            }
            using (var command = new SqlCommand("Select * From TblDetalleFactura", connection))
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var detalle = new DetalleFactura
                    {
                        DetalleId = (int)reader["Detalle_Id"],
                        DetalleFacturaId = (int)reader["Factura_Id"],
                        DetalleProducto = reader["Detalle_Nombre"].ToString(),
                        DetalleCantidad = (int)reader["Detalle_Cantidad"],
                        DetallePrecio = (decimal)reader["Detalle_Precio"],
                    };

                    var factura = facturas.FirstOrDefault(find => find.FacturaId == detalle.DetalleFacturaId);
                    if (factura != null)
                    {
                        factura.FacturaDetalle.Add(detalle);
                    }
                }
            }
        }
        return facturas;
    }

    public Factura GetByID(int id)
    {
        var factura = new Factura();
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqlCommand("Select * From TblFactura Where Factura_Id=@Id", connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        factura.FacturaId = reader.GetInt32(0);
                        factura.FacturaCliente = reader.GetString(1);
                        factura.FacturaFecha = reader.GetDateTime(2);
                    }
                }
            }

            using (var command = new SqlCommand("Select * From TblDetalleFactura where Factura_Id=@Id", connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    factura.FacturaDetalle = new List<DetalleFactura>();
                    while (reader.Read())
                    {
                        factura.FacturaDetalle.Add(new DetalleFactura
                        {
                            DetalleId = reader.GetInt32(0),
                            DetalleFacturaId = reader.GetInt32(1),
                            DetalleProducto = reader.GetString(2),
                            DetalleCantidad = reader.GetInt32(3),
                            DetallePrecio = reader.GetDecimal(4)
                        });
                    }
                }
            }
        }
        return factura;
    }

    public void Update(Factura factura)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var facturaCommand = new SqlCommand(
                        "UPDATE TblFactura SET Factura_Cliente = @Cliente, Factura_Fecha = @Fecha WHERE Factura_Id = @Factura",
                        connection,
                        transaction);
                    facturaCommand.Parameters.AddWithValue("@Cliente", factura.FacturaCliente);
                    facturaCommand.Parameters.AddWithValue("@Fecha", factura.FacturaFecha);
                    facturaCommand.Parameters.AddWithValue("@Factura", factura.FacturaId);

                    facturaCommand.ExecuteNonQuery();

                    foreach (var detail in factura.FacturaDetalle)
                    {
                        var detalleCommand = new SqlCommand(
                            "UPDATE TblDetalleFactura SET Detalle_Nombre = @Producto, Detalle_Cantidad = @Cantidad, Detalle_Precio = @Precio WHERE Detalle_Id = @DetalleId",
                            connection,
                            transaction);
                        detalleCommand.Parameters.AddWithValue("@Producto", detail.DetalleProducto);
                        detalleCommand.Parameters.AddWithValue("@Cantidad", detail.DetalleCantidad);
                        detalleCommand.Parameters.AddWithValue("@Precio", detail.DetallePrecio);
                        detalleCommand.Parameters.AddWithValue("@DetalleId", detail.DetalleId);

                        detalleCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();

                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}

