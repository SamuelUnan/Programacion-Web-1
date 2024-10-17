using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WebApi.Interface;
using WebApi.Model;

namespace WebApi.Implementation;

public class MateriaService : IMateriaService
{
    private readonly IConfiguration _configuration;
    private string connectionString;
    public MateriaService(IConfiguration configuration)
    {
        _configuration = configuration;
        connectionString = _configuration.GetConnectionString("DatabaseConnection");
    }

    public async Task<MateriaEntities> Add(MateriaEntities materia)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("INSERT INTO CatMateria (Materia_Nombre) VALUES (@Name)", connection);
            command.Parameters.AddWithValue("@Name", materia.Name);
            await connection.OpenAsync();
            await command.ExecuteReaderAsync();
        }
        return materia;
    }

    public async Task<int> Delete(int id)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("DELETE FROM CatMateria WHERE Materia_Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }
    }

    public List<MateriaEntities> GetAll()
    {
        List<MateriaEntities> Materias = new List<MateriaEntities>();
        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("Select * From CatMateria", connection);
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Materias.Add(new MateriaEntities
                    {
                        Id = (int)reader["Materia_Id"],
                        Name = reader["Materia_Nombre"].ToString(),
                        State = (bool)reader["Materia_State"]
                    });
                }
            }
        }
        return Materias;
    }

    public async Task<MateriaEntities> GetById(int id)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("SELECT * FROM CatMateria WHERE Materia_Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            MateriaEntities materia = null;
            await connection.OpenAsync();

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    materia = new MateriaEntities
                    {
                        Id = (int)reader["Materia_Id"],
                        Name = reader["Materia_Nombre"].ToString(),
                        State = (bool)reader["Materia_State"]
                    };
                }
            }
            return materia;
        }
    }

    public async Task<MateriaEntities> Update(MateriaEntities materia)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("UPDATE CatMateria SET Materia_Nombre = @Name, Materia_State = @State WHERE Materia_Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", materia.Id);
            command.Parameters.AddWithValue("@Name", materia.Name);
            command.Parameters.AddWithValue("@State", materia.State);

            await connection.OpenAsync();

            var response = await command.ExecuteReaderAsync();

            if (response != null)
            {
                return materia;
            }
            else
            {
                return null;
            }
        }
    }

}
