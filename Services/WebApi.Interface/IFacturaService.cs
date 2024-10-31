using WebApi.Model;

namespace WebApi.Interface;

public interface IFacturaService
{
    public Factura Add(Factura factura);
    public IEnumerable<Factura> GetALL();
    public Factura GetByID(int id);
    void Update(Factura factura);
    void Delete(int id);
}
