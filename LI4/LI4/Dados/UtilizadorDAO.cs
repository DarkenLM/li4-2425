using Dapper;
using Microsoft.Data.SqlClient;

namespace LI4.Dados;

public class UtilizadorDAO {
    private readonly string connectionString;

    public UtilizadorDAO(string conStr) {
        this.connectionString = conStr;
    }

    private SqlConnection GetConnection() => new SqlConnection(this.connectionString);

    public async Task<IEnumerable<Utilizador>> GetAllAsync() {
        using var connection = GetConnection();
        const string query = "SELECT * FROM Utilizador";
        return await connection.QueryAsync<Utilizador>(query);
    }

    public async Task<Utilizador?> GetByIdAsync(int id) {
        using var connection = GetConnection();
        const string query = "SELECT * FROM Utilizador WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Utilizador>(query, new { id = id });
    }

    public async Task<int> AddAsync(Utilizador user) {
        using var connection = GetConnection();
        const string query = @"
            INSERT INTO Utilizador (username, email, password)
            VALUES (@username, @email, @password);
            SELECT CAST(SCOPE_IDENTITY() as int)";
        return await connection.ExecuteScalarAsync<int>(query, user);
    }

    public async Task<bool> UpdateAsync(Utilizador user) {
        using var connection = GetConnection();
        const string query = @"
            UPDATE Utilizador
            SET username = @username, email = @email, password = @password
            WHERE id = @id";
        int rowsAffected = await connection.ExecuteAsync(query, user);
        return rowsAffected > 0;
    }

    // Delete a user
    public async Task<bool> DeleteAsync(int id) {
        using var connection = GetConnection();
        const string query = "DELETE FROM Utilizador WHERE id = @id";
        int rowsAffected = await connection.ExecuteAsync(query, new { id = id });
        return rowsAffected > 0;
    }
}
