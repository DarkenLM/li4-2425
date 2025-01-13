using Dapper;
using Microsoft.Data.SqlClient;

namespace LI4.Dados;

/// <summary>
/// Provides an access point to the table LI4.Utilizador on the database and provides CRUD operations within that table.
/// 
/// All methods need to be run in an asynchronous environment.
/// 
/// <example>
/// Example:
/// <code>
/// Utilizador user = await UtilizadorDAO.GetByIdAsync(1); // Utilizador{id=1, username=abc, email=a@b.c, palavraPasse=123}
/// </code>
/// </example>
/// 
/// See LI4.Client#Pages.Users for more examples.
/// </summary>
public class UserDAO {
    private readonly string connectionString;

    public UserDAO(string conStr) {
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
            INSERT INTO Utilizador (username, email, palavraPasse)
            VALUES (@username, @email, @palavraPasse);
            SELECT CAST(SCOPE_IDENTITY() as int)";
        return await connection.ExecuteScalarAsync<int>(query, user);
    }

    public async Task<bool> UpdateAsync(Utilizador user) {
        using var connection = GetConnection();
        const string query = @"
            UPDATE Utilizador
            SET username = @username, email = @email, palavraPasse = @palavraPasse
            WHERE id = @id";
        int rowsAffected = await connection.ExecuteAsync(query, user);
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateUserEmailAsync(int id, string newEmail) {
        using var connection = GetConnection();
        const string query = @"
        UPDATE Utilizador
        SET email = @newEmail
        WHERE id = @id";
        int rowsAffected = await connection.ExecuteAsync(query, new { id, newEmail });
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateUserUsernameAsync(int id, string newUsername) {
        using var connection = GetConnection();
        const string query = @"
        UPDATE Utilizador
        SET username = @newUsername
        WHERE id = @id";
        int rowsAffected = await connection.ExecuteAsync(query, new { id, newUsername });
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateUserPasswordAsync(int id, string newPassword) {
        using var connection = GetConnection();
        const string query = @"
        UPDATE Utilizador
        SET palavraPasse = @newPassword
        WHERE id = @id";
        int rowsAffected = await connection.ExecuteAsync(query, new { id, newPassword });
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id) {
        using var connection = GetConnection();
        const string query = "DELETE FROM Utilizador WHERE id = @id";
        int rowsAffected = await connection.ExecuteAsync(query, new { id = id });
        return rowsAffected > 0;
    }
}
