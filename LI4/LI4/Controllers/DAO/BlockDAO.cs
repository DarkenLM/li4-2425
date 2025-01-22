using Dapper;
using LI4.Common.Dados;
using LI4.Dados;
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
    /// <summary>
    ///  The connection string used to connect to the database.
    /// </summary>
    private readonly string connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlockDAO"/> class with the specified connection string.
    /// </summary>
    /// <param name="conStr">The connnection string to the database</param>
    public BlockDAO(string conStr) {
        this.connectionString = conStr;
    }

    /// <summary>
    /// Creates and returns a new SQL connection using the configured connection string.
    /// </summary>
    /// <returns>A new <see cref="SqlConnection"/> instance.</returns>
    private SqlConnection getConnection() => new SqlConnection(this.connectionString);

    #region//---- CRUD OPERATIONS ----//
    /**
     * This function adds a block instance to a User.
     * It needs the userID who has ownership off the block, and it need the idBlockProperty which represents the
     * properties off the block, being the name, etc.
     */
    public async Task<int?> addBlockTypeAsync(string name, BlockRarity rarity, int timeToAcquire) {
        using var connection = getConnection();
        const string query = @"
            INSERT INTO BlockProperties (name, rarity, timeToAcquire)
            Values (@Name, @Rarity, @TimeToAcquire);
            SELECT CAST(SCOPE_IDENTITY() as int);";
        return await connection.ExecuteScalarAsync<int>(query, new { Name = name, Rarity = rarity.ToString(), TimeToAcquire = timeToAcquire });
    }

    public async Task<int?> addBlockInstanceAsync(int idUser, int idBlockProperty, int quantity) {
        using var connection = getConnection();
        const string query = @"
            MERGE INTO Blocks AS target
                USING (VALUES (@idBlockProperty, @idUser, @quantity)) AS source (idBlockProperty, idUser, quantity)
                ON target.idBlockProperty = source.idBlockProperty AND target.idUser = source.idUser
            WHEN MATCHED THEN 
                UPDATE SET quantity = target.quantity + source.quantity
            WHEN NOT MATCHED THEN
                INSERT (idBlockProperty, idUser, quantity) 
                VALUES (source.idBlockProperty, source.idUser, source.quantity);
        ";

        return await connection.ExecuteScalarAsync<int>(query, new { quantity, idUser, idBlockProperty });
    }

    public async Task<Block?> getBlockByIdAsync(int id) {
        using var connection = getConnection();
        const string query = @"
            SELECT b.id, b.quantity, bp.name, bp.rarity, bp.timeToAcquire
            FROM Blocks b
            INNER JOIN BlockProperties bp ON b.idBlockProperty = bp.id
            WHERE b.id = @id;";
        return await connection.QueryFirstOrDefaultAsync<Block>(query, new { id });
    }

    public async Task<IEnumerable<Block>> getAllBlockInstancesAsync() {
        using var connection = getConnection();
        const string query = @"
            SELECT b.idUser, b.quantity, name, rarity, timeToAcquire 
            FROM Blocks b
            INNER JOIN BlockProperties bp ON b.idBlockProperty = bp.id;";
        return await connection.QueryAsync<Block>(query);
    }

    public async Task<Dictionary<int, BlockProperties>> getAllBlockPropertiesAsync() {
        using var connection = getConnection();
        const string query = @"
            SELECT id, name, rarity, timeToAcquire 
            FROM BlockProperties;";
        var result = await connection.QueryAsync<BlockProperties>(query);
        return result.ToDictionary(r => r.id, r => r);
    }

    public async Task<bool> updateBlockPropertyAsync(int id, int quantity, string name, BlockRarity rarity, int timeToAcquire) {
        using var connection = getConnection();
        const string query = @"
            UPDATE BlockProperties
            SET quantity = @Quantity, name = @Name, rarity = @Rarity, timeToAcquire = @TimeToAcquire
            WHERE id = @Id";
        int rowsAffected = await connection.ExecuteAsync(query, new { Id = id, Quantity = quantity, Name = name, Rarity = rarity.ToString(), TimeToAcquire = timeToAcquire});
        return rowsAffected > 0;
    }

    public async Task<bool> deleteAsync(int id) {
        using var connection = getConnection();
        const string query = "DELETE FROM Blocks WHERE id = @Id";
        int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
        return rowsAffected > 0;
    }
    #endregion

    public async Task<Dictionary<string, int>> getAllUserBlocksAsync(int idUser) {
        using var connection = getConnection();
        const string query = @"
            SELECT bp.name, COALESCE(SUM(b.quantity), 0)
            FROM BlockProperties bp
            LEFT JOIN Blocks b ON bp.id = b.idBlockProperty AND (b.idUser = @idUser OR b.idUser IS NULL)
            GROUP BY bp.name;
            ";

        var result = await connection.QueryAsync<(string Name, int Quantity)>(query, new { idUser });
        return result.ToDictionary(r => r.Name, r => r.Quantity);
    }

    public async Task<int> getTimeToAcquireByIdAsync(int idBlockProperties) {
        using var connection = getConnection();
        const string query = @"
            SELECT timeToAcquire
            FROM BlockProperties
            WHERE id = @Id
        ";

        return await connection.QueryFirstAsync<int>(query, new { Id = idBlockProperties });
    }

    public async Task removeBlocksFromUserAsync(int idUser, int idBlockProperty, int quantity) {
        using var connection = getConnection();
        const string query = @"
            UPDATE Blocks
            SET quantity = CASE 
                               WHEN quantity >= @quantity THEN quantity - @quantity
                               ELSE 0
                           END
            WHERE idUser = @idUser AND idBlockProperty = @idBlockProperty;";
        await connection.ExecuteAsync(query, new { quantity, idUser, idBlockProperty });
    }
}
