using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApi.Interface;
using WebApi.Model;

namespace WebApi.Implementation;

public class UsuarioService : IUsuarioService
{
    private readonly string _ConnectionString;
    private readonly IConfiguration _configuration;
    public UsuarioService(IConfiguration configuration)
    {

        _configuration = configuration;

        _ConnectionString = _configuration.GetConnectionString("DatabaseConnection");

    }
    public async Task<UsuarioEntities> Autenticar(string username, string password)
    {
        UsuarioEntities usuario = null;
        byte[] storedSalt = null;
        using (var connection = new SqlConnection(_ConnectionString))
        {
            var command = new SqlCommand("SELECT * FROM CatUsuario WHERE UsuarioUsername = @Username", connection);
            command.Parameters.AddWithValue("@Username", username);

            await connection.OpenAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (reader.Read())
                {
                    var passwordHash = reader["UsuarioPassword"].ToString();
                    storedSalt = (byte[])reader["UsuarioSalt"];
                    if (VerifyPasswordHash(password, passwordHash, storedSalt))
                    {
                        usuario = new UsuarioEntities
                        {
                            Id = (int)reader["UsuarioId"],
                            Username = reader["UsuarioUsername"].ToString(),
                            Password = passwordHash,
                            Rol = reader["UsuarioRol"].ToString()
                        };
                    }
                }
            }
        }
        return usuario;
    }

    public string GenerateJwtToken(UsuarioEntities usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        Console.WriteLine(key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public IEnumerable<UsuarioEntities> GetAll()
    {
        var usuarios = new List<UsuarioEntities>();

        using (var connection = new SqlConnection(_ConnectionString))
        {
            var command = new SqlCommand("SELECT * FROM CatUsuario", connection);
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var usuario = new UsuarioEntities
                    {
                        Id = (int)reader["UsuarioId"],
                        Username = reader["UsuarioUsername"].ToString(),
                        Rol = reader["UsuarioRol"].ToString()
                    };
                    usuarios.Add(usuario);
                }
            }
        }
        return usuarios;
    }

    public UsuarioEntities GetById(int id)
    {
        UsuarioEntities usuario = null;

        using (var connection = new SqlConnection(_ConnectionString))
        {
            var command = new SqlCommand("SELECT * FROM CatUsuario WHERE UsuarioId = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    usuario = new UsuarioEntities
                    {
                        Id = (int)reader["UsuarioId"],
                        Username = reader["UsuarioUsername"].ToString(),
                        Rol = reader["UsuarioRol"].ToString()
                    };
                }
            }
        }
        return usuario;
    }

    public async Task<UsuarioEntities> Registrar(UsuarioEntities usuario, string password)
    {
        byte[] salt;
        usuario.Password = CreatePasswordHash(password, out salt);

        using (var connection = new SqlConnection(_ConnectionString))
        {
            var command = new SqlCommand("INSERT INTO CatUsuario (UsuarioUsername, UsuarioPassword, UsuarioRol, UsuarioSalt) OUTPUT INSERTED.UsuarioId VALUES (@Username, @PasswordHash, @Role, @Salt)", connection);
            command.Parameters.AddWithValue("@Username", usuario.Username);
            command.Parameters.AddWithValue("@PasswordHash", usuario.Password);
            command.Parameters.AddWithValue("@Salt", salt);
            command.Parameters.AddWithValue("@Role", usuario.Rol);

            await connection.OpenAsync();
            usuario.Id = (int)await command.ExecuteScalarAsync();
        }
        return usuario;
    }

    private string CreatePasswordHash(string password, out byte[] salt)
    {
        using var hmac = new HMACSHA256();
        salt = hmac.Key;
        var combinedBytes = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
        var hash = hmac.ComputeHash(combinedBytes);
        return Convert.ToBase64String(hash);
    }

    private bool VerifyPasswordHash(string password, string storedHash, byte[] salt)
    {
        using var hmac = new HMACSHA256(salt);
        var combinedBytes = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
        var computedHash = hmac.ComputeHash(combinedBytes);
        return storedHash == Convert.ToBase64String(computedHash);
    }


}