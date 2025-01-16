using Dapper;
using LI4.Common.Dados;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LI4.Controllers.DAO;

/// <summary>
/// Provides an access point to the table LI4.Bloco on the database and provides CRUD operations within that table.
/// 
/// All methods need to be run in an asynchronous environment.
/// 
/// <example>
/// Example:
/// <code>
/// Block block = await BlockDAO.GetByIdAsync(1); // Block{id=1, username=abc, email=a@b.c, timeToAcquire=123, orderID=1}
/// </code>
/// </example>
/// 
/// See LI4.Client#Pages.Blocks for more examples.
/// </summary>
public class BlockDAO {
    private readonly string connectionString;

    public BlockDAO(string conStr) {
        this.connectionString = conStr;
    }

    private SqlConnection getConnection() => new SqlConnection(this.connectionString);

    public async Task<Block?> GetByIdAsync(int id) {
        using var connection = getConnection();
        const string query = @"
            SELECT b.id, b.name, b.rarity, b.timeToAcquire
            FROM Blocks b
            INNER JOIN BlockProperties bp ON b.idBlockProperty = bp.id
            WHERE b.id = @id;";
        return await connection.QueryFirstOrDefaultAsync<Block>(query, new { id });
    }
}
