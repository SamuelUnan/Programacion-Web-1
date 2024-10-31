namespace WebApi.Model
{
    public class Factura
    {
        public int FacturaId { get; set; }
        public string FacturaCliente { get; set; }
        public DateTime FacturaFecha { get; set; }
        public List<DetalleFactura> FacturaDetalle { get; set; } = new List<DetalleFactura>();
    }

    public class DetalleFactura
    {
        public int DetalleId { get; set; }
        public int DetalleFacturaId { get; set; }
        public string DetalleProducto { get; set; }
        public int DetalleCantidad { get; set; }
        public decimal DetallePrecio { get; set; }
    } 
}