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

    #region//---- CRUD OPERATIONS ----//
    public async Task<int> addAsync(Order order) {
        using var connection = getConnection();
        const string query = @"
            INSERT INTO Orders (idUser, orderDate, delivered)
            VALUES (@idUser, @orderDate, @delivered);
            SELECT CAST(SCOPE_IDENTITY() as int)";
        return await connection.ExecuteScalarAsync<int>(query, order);
    }

    public async Task<IEnumerable<Order>> getAllAsync() {
        using var connection = getConnection();
        const string query = "SELECT * FROM Orders";
        return await connection.QueryAsync<Order>(query);
    }

    public async Task<Order?> getByIdAsync(int id) {
        using var connection = getConnection();
        const string query = "SELECT * FROM Orders WHERE id = @id";
        return await connection.QueryFirstOrDefaultAsync<Order>(query, new { id });
    }

    public async Task<bool> updateAsync(Order order) {
        using var connection = getConnection();
        const string query = @"
            UPDATE Orders
            SET idUser = @IdUser, orderDate = @OrderDate
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
    #endregion

    public async Task<List<(int, int, int, int)>> getRemainingOrders() {
        using var connection = getConnection();
        const string query = @"
            SELECT o.id, o.idUser, bo.idBlockProperty, bo.quantity
            FROM Orders o
            INNER JOIN BlocksInOrder bo ON bo.idOrder = o.id
            WHERE delivered = 0
        ";

        var remainingOrders = await connection.QueryAsync<(int, int, int, int)>(query);
        return remainingOrders.ToList();
    }

    public async Task<bool> updateOrderDeliveredAsync(int idOrder, bool delivered) {
        using var connection = getConnection();
        const string query = @"
            UPDATE Orders
            SET delivered = @delivered
            WHERE id = @idOrder
        ";

        int rowsAffected = await connection.ExecuteAsync(query, new { delivered, idOrder });
        return rowsAffected > 0;
    }

    public async Task<Dictionary<int, int>> getOrderContentAsync(int id) {
        using var connection = getConnection();
        const string query = @"
            SELECT bp.id, bo.quantity
            FROM Orders o
            INNER JOIN BlocksInOrder bo ON o.id = bo.idOrder
            INNER JOIN BlockProperties bp ON bo.idBlockProperty = bp.id
            WHERE o.id = @id;";
        var result = await connection.QueryAsync<(int id, int Quantity)>(query, new { id });
        return result.ToDictionary(r => r.id, r => r.Quantity);
    }

    public async Task<bool> addBlocksInOrderAsync(int idOrder, int idBlockProperty, int quantity) {
        using var connection = getConnection();
        const string query = @"
            INSERT INTO BlocksInOrder (idOrder, idBlockProperty, quantity)
            VALUES (@idOrder, @idBlockProperty, @quantity);
        ";

        int rowsAffected = await connection.ExecuteAsync(query, new { idOrder, idBlockProperty, quantity });
        return rowsAffected > 0;
    }

    public async Task<List<Order>> getUserOrdersAsync(int id) {
        using var connection = getConnection();
        const string query = @" 
            SELECT * FROM Orders WHERE idUser = @Id; 
        ";
        var orders = await connection.QueryAsync<Order>(query, new { Id = id });
        return orders.ToList();
    }

    public async Task<int> getEstimatedTimeOrderAsync(int idOrder) {
        using var connection = getConnection();
        const string query = @"
            SELECT 
                (bio.quantity * b.timeToAcquire) AS TotalTimeToAcquire
            FROM 
                BlocksInOrder bio
            INNER JOIN 
                BlockProperties b ON bio.idBlockProperty = b.id
            WHERE 
                bio.idOrder = @OrderId;
        ";

        var times = await connection.QueryAsync<int>(query, new { OrderId = idOrder });
        return times.Sum();
    }
}
