using WebApi.Model;

namespace WebApi.Interface;
public interface IUsuarioService
{
    Task<UsuarioEntities> Autenticar(string username, string password);
    Task<UsuarioEntities> Registrar(UsuarioEntities usuario, string password);
    IEnumerable<UsuarioEntities> GetAll();
    UsuarioEntities GetById(int id);
    string GenerateJwtToken(UsuarioEntities usuario);
}