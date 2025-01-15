using Dapper;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LI4.Dados;

/// <summary>
/// Provides an access point to the table LI4.Order in the database and supports CRUD operations.
/// 
/// All methods need to be run in an asynchronous environment.
/// 
/// <example>
/// Example:
/// <code>
/// Order order = await OrderDAO.GetByIdAsync(1); // Order{id=1, idUtilizador="abc", orderDate="2025-01-01 12:00:00"}
/// </code>
/// </example>
/// 
/// See LI4.Client#Pages.Orders for more examples.
/// </summary>
public class OrderDAO {
    private readonly string connectionString;

    public OrderDAO(string conStr) {
        this.connectionString = conStr;
    }

    private SqlConnection getConnection() => new SqlConnection(this.connectionString);

    public async Task<IEnumerable<Order>> getAllAsync() {
        using var connection = getConnection();
        const string query = "SELECT * FROM Encomenda";
        return await connection.QueryAsync<Order>(query);
    }

    public async Task<Order?> getByIdAsync(int id) {
        using var connection = getConnection();
        const string query = "SELECT * FROM Utilizador WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Order>(query, new { id = id });
    }

    public async Task<int> addAsync(Order order) {
        using var connection = getConnection();
        const string query = @"
            INSERT INTO Encomenda (idUtilizador, orderDate)
            VALUES (@IdUtilizador, @OrderDate);
            SELECT CAST(SCOPE_IDENTITY() as int)";
        return await connection.ExecuteScalarAsync<int>(query, order);
    }

    public async Task<bool> UpdateAsync(Order order) {
        using var connection = getConnection();
        const string query = @"
            UPDATE Encomenda
            SET idUtilizador = @IdUtilizador, orderDate = @OrderDate
            WHERE id = @Id";
        int rowsAffected = await connection.ExecuteAsync(query, order);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id) {
        using var connection = getConnection();
        const string query = "DELETE FROM Encomenda WHERE id = @Id";
        int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
        return rowsAffected > 0;
    }
}
