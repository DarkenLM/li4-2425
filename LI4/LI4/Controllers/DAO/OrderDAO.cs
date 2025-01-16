using Dapper;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using LI4.Dados;

namespace LI4.Controllers.DAO;

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
        const string query = "SELECT * FROM Orders";
        return await connection.QueryAsync<Order>(query);
    }

    public async Task<Order?> getByIdAsync(int id) {
        using var connection = getConnection();
        const string query = "SELECT * FROM Orders WHERE id = @id";
        return await connection.QueryFirstOrDefaultAsync<Order>(query, new { id = id });
    }

    public async Task<int> addAsync(Order order) {
        using var connection = getConnection();
        const string query = @"
            INSERT INTO Encomenda (idUser, orderDate)
            VALUES (@IdUser, @OrderDate);
            SELECT CAST(SCOPE_IDENTITY() as int)";
        return await connection.ExecuteScalarAsync<int>(query, order);
    }

    public async Task<bool> updateAsync(Order order) {
        using var connection = getConnection();
        const string query = @"
            UPDATE Orders
            SET idUtilizador = @IdUser, orderDate = @OrderDate
            WHERE id = @Id";
        int rowsAffected = await connection.ExecuteAsync(query, order);
        return rowsAffected > 0;
    }

    public async Task<bool> deleteAsync(int id) {
        using var connection = getConnection();
        const string query = "DELETE FROM Orders WHERE id = @Id";
        int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<Dictionary<string, int>> getOrderContentAsync(int id) {
        using var connection = getConnection();
        const string query = @"
            SELECT bp.name, bo.quantity
            FROM Orders o
            INNER JOIN BlocksInOrder bo ON o.id = bo.idOrder
            INNER JOIN BlockProperties bp ON bo.idBlockProperty = bp.id
            WHERE o.id = @id;";
        var result = await connection.QueryAsync<(string Name, int Quantity)>(query, new { id });
        return result.ToDictionary(r => r.Name, r => r.Quantity);
    }

    public async Task<List<Order>> getOrdersAsync(string email) {
        using var connection = getConnection();
        const string query = @"
        SELECT 
            o.id AS id,
            o.idUser AS idUser,
            o.orderDate AS orderDate
        FROM 
            Orders o
        INNER JOIN 
            Users u
        ON 
            o.idUser = u.id
        WHERE 
            u.email = @Email;
        ";

        var orders = await connection.QueryAsync<Order>(query, new { Email = email });
        return orders.ToList();
    }
}
