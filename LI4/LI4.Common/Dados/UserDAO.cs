using Dapper;
using LI4.Common.Exceptions.UserExceptions;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    private SqlConnection getConnection() => new SqlConnection(this.connectionString);

    public async Task<IEnumerable<Utilizador>> getAllAsync() {
        using var connection = getConnection();
        const string query = "SELECT * FROM Utilizador";
        return await connection.QueryAsync<Utilizador>(query);
    }

    public async Task<Utilizador?> getByIdAsync(int id) {
        using var connection = getConnection();
        const string query = "SELECT * FROM Utilizador WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Utilizador>(query, new { id = id });
    }

    public async Task<int> addAsync(Utilizador user) {
        using var connection = getConnection();
        const string query = @"
            INSERT INTO Utilizador (username, email, palavraPasse)
            VALUES (@username, @email, @palavraPasse);
            SELECT CAST(SCOPE_IDENTITY() as int)";
        return await connection.ExecuteScalarAsync<int>(query, user);
    }

    public async Task<bool> updateAsync(Utilizador user) {
        using var connection = getConnection();
        const string query = @"
            UPDATE Utilizador
            SET username = @username, email = @email, palavraPasse = @palavraPasse
            WHERE id = @id";
        int rowsAffected = await connection.ExecuteAsync(query, user);
        return rowsAffected > 0;
    }

    public async Task<bool> emailExistsAsync(string email) {
        using var connection = getConnection();
        const string query = "SELECT COUNT(1) FROM Utilizador WHERE email = @Email";
        return await connection.ExecuteScalarAsync<int>(query, new { Email = email }) > 0;
    }

    public async Task<bool> usernameExistsAsync(string username) {
        using var connection = getConnection();
        const string query = "SELECT COUNT(1) FROM Utilizador WHERE username = @Username";
        return await connection.ExecuteScalarAsync<int>(query, new { Username = username }) > 0;
    }



    public async Task<bool> updateUserEmailAsync(int id, string newEmail) {
        using var connection = getConnection();
        const string query = @"
        UPDATE Utilizador
        SET email = @newEmail
        WHERE id = @id";
        int rowsAffected = await connection.ExecuteAsync(query, new { id, newEmail });
        return rowsAffected > 0;
    }

    public async Task<bool> updateUserUsernameAsync(int id, string newUsername) {
        using var connection = getConnection();
        const string query = @"
        UPDATE Utilizador
        SET username = @newUsername
        WHERE id = @id";
        int rowsAffected = await connection.ExecuteAsync(query, new { id, newUsername });
        return rowsAffected > 0;
    }

    public async Task<bool> updateUserPasswordAsync(int id, string newPassword) {
        using var connection = getConnection();
        const string query = @"
        UPDATE Utilizador
        SET palavraPasse = @newPassword
        WHERE id = @id";
        int rowsAffected = await connection.ExecuteAsync(query, new { id, newPassword });
        return rowsAffected > 0;
    }

    public async Task<bool> deleteAsync(int id) {
        using var connection = getConnection();
        const string query = "DELETE FROM Utilizador WHERE id = @id";
        int rowsAffected = await connection.ExecuteAsync(query, new { id = id });
        return rowsAffected > 0;
    }

    public async Task<Utilizador> getByEmailAsync(string email) {
        try {
            using var connection = getConnection();
            const string query = "SELECT * FROM Utilizador WHERE email = @email";
            Utilizador? user = await connection.QueryFirstOrDefaultAsync<Utilizador>(query, new { email });

            return user ?? throw new UserNotFoundException($"User with email: {email} not found");

        } catch (SqlException ex) {
            throw new UserNotFoundException("An error occurred while retrieving the user.", ex);
        }
    }

    public async Task<bool> authenticateAsync(string email, string password) {
        using var connection = getConnection();
        const string query = "SELECT COUNT(1) FROM Utilizador WHERE email = @Email AND palavraPasse = @Password";
        int count = await connection.ExecuteScalarAsync<int>(query, new { Email = email, Password = password });
        if (count > 0) {
            return true;
        } else {
            throw new UserNotAuthorizedException("Authentication failed: Invalid password.");
        }
    }
}
