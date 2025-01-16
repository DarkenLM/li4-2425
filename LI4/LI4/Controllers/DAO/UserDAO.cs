using Dapper;
using LI4.Common.Exceptions.UserExceptions;
using LI4.Dados;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LI4.Controllers.DAO;

/// <summary>
/// Provides an access point to the table LI4.Utilizador on the database and provides CRUD operations within that table.
/// 
/// All methods need to be run in an asynchronous environment.
/// 
/// <example>
/// Example:
/// <code>
/// Utilizador user = await UtilizadorDAO.GetByIdAsync(1); // Utilizador{id=1, username=abc, email=a@b.c, userPassword=123}
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

    #region//---- CRUD OPERATIONS ----//
    public async Task<int> addAsync(User user) {
        using var connection = getConnection();
        const string query = @"
            INSERT INTO Users (username, email, userPassword)
            VALUES (@username, @email, @userPassword);
            SELECT CAST(SCOPE_IDENTITY() as int)";
        return await connection.ExecuteScalarAsync<int>(query, user);
    }

    public async Task<User?> getByIdAsync(int id) {
        using var connection = getConnection();
        const string query = "SELECT * FROM Users WHERE id = @id";
        return await connection.QueryFirstOrDefaultAsync<User>(query, new { id = id });
    }

    public async Task<User> getByEmailAsync(string email) {
        try {
            using var connection = getConnection();
            const string query = "SELECT * FROM Users WHERE email = @email";
            User? user = await connection.QueryFirstOrDefaultAsync<User>(query, new { email });

            return user ?? throw new UserNotFoundException($"User with email: {email} not found");

        } catch (SqlException ex) {
            throw new UserNotFoundException("An error occurred while retrieving the user.", ex);
        }
    }

    public async Task<IEnumerable<User>> getAllAsync() {
        using var connection = getConnection();
        const string query = "SELECT * FROM Users";
        return await connection.QueryAsync<User>(query);
    }

    public async Task<bool> updateAsync(User user) {
        using var connection = getConnection();
        const string query = @"
            UPDATE Users
            SET username = @username, email = @email, userPassword = @userPassword
            WHERE id = @id";
        int rowsAffected = await connection.ExecuteAsync(query, user);
        return rowsAffected > 0;
    }

    public async Task<bool> deleteAsync(int id) {
        using var connection = getConnection();
        const string query = "DELETE FROM Users WHERE id = @id";
        int rowsAffected = await connection.ExecuteAsync(query, new { id = id });
        return rowsAffected > 0;
    }
    #endregion//---- END ----/

    public async Task<bool> emailExistsAsync(string email) {
        using var connection = getConnection();
        const string query = "SELECT COUNT(1) FROM Users WHERE email = @Email";
        bool count = await connection.ExecuteScalarAsync<int>(query, new { Email = email }) > 0;
        if (count) {
            return true;
        } else {
            throw new UserNotFoundException($"User with email: {email} not found");
        }
    }

    public async Task<bool> usernameExistsAsync(string username) {
        using var connection = getConnection();
        const string query = "SELECT COUNT(1) FROM Users WHERE username = @Username";
        bool count = await connection.ExecuteScalarAsync<int>(query, new { Username = username }) > 0;
        if (count) {
            return true;
        } else {
            throw new UserNotFoundException($"User with username: {username} not found");
        }
    }

    public async Task<bool> usernameNoOtherExistsAsync(int id, string username) {
        using var connection = getConnection();
        const string query = @"
            SELECT COUNT(1) 
            FROM Users 
            WHERE username = @Username 
            AND id != @id";
        bool existsForOther = await connection.ExecuteScalarAsync<int>(query, new { Username = username, id = id }) > 0;

        if (!existsForOther) {
            return true;
        } else {
            throw new UserAlreadyExistsException($"User with username: {username} already exists for another account.");
        }
    }

    public async Task<bool> emailNoOtherExistsAsync(int id, string email) {
        using var connection = getConnection();
        const string query = @"
            SELECT COUNT(1) 
            FROM Users 
            WHERE email = @Email 
            AND id != @Id";
        bool existsForOther = await connection.ExecuteScalarAsync<int>(query, new { Email = email, id = id }) > 0;

        if (!existsForOther) {
            return true;
        } else {
            throw new UserAlreadyExistsException($"User with email: {email} already exists for another account.");
        }
    }

    public async Task<bool> authenticateAsync(string email, string password) {
        using var connection = getConnection();
        const string query = "SELECT COUNT(1) FROM Users WHERE email = @Email AND userPassword = @Password";
        int count = await connection.ExecuteScalarAsync<int>(query, new { Email = email, Password = password });
        if (count > 0) {
            return true;
        } else {
            throw new UserNotAuthorizedException("Authentication failed: Invalid userPassword.");
        }
    }
}
