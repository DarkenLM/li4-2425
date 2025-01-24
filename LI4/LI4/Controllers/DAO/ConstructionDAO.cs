using Dapper;
using LI4.Common.Dados;
using LI4.Common.Exceptions.ConstructionExceptions;
using LI4.Dados;
using Microsoft.Data.SqlClient;
using System.Collections;

namespace LI4.Controllers.DAO;

public class ConstructionDAO {
    private readonly string connectionString;

    public ConstructionDAO(string conStr) {
        this.connectionString = conStr;
    }

    private SqlConnection getConnection() => new SqlConnection(this.connectionString);

    #region//---- CRUD OPERATIONS ----//
    public async Task<int?> addConstructionInstanceAsync(ConstructionState state, int constructionPropertiesID, int userID) {
        using var connection = getConnection();
        const string query = @"
            INSERT INTO Constructions (state, idConstructionProperties, idUser)
            VALUES (@State, @ConstructionPropertiesID, @UserID);
            SELECT CAST(SCOPE_IDENTITY() as int);";
        return await connection.ExecuteScalarAsync<int?>(query, new { State = state, ConstructionPropertiesID = constructionPropertiesID.ToString(), UserID = userID });
    }

    public async Task<Construction?> getByIDAsync(int constructionID) {
        try {
            using var connection = getConnection();
            const string query = @"
            SELECT c.id, state, name, dificulty, nStages
            FROM Constructions c
            INNER JOIN ConstructionProperties cp ON cp.id = c.idConstructionProperties;
            WHERE c.id = @Id";
            Construction? construction = await connection.QueryFirstOrDefaultAsync<Construction>(query, new { Id = constructionID });

            return construction ?? throw new ConstructionNotFoundException($"Construction with id: {constructionID} not found");

        } catch (SqlException ex) {
            throw new ConstructionNotFoundException("An error occurred while retrieving the construction.", ex);
        }
    }
    
    public async Task<IEnumerable<Construction>> getAllConstructionInstancesAsync() {
        using var connection = getConnection();
        const string query = @"
            SELECT c.id, name, state, dificulty, nStages 
            FROM Constructions c
            INNER JOIN ConstructionProperties cp ON c.idConstructionProperties = cp.id;";
        return await connection.QueryAsync<Construction>(query);
    }

    public async Task<Dictionary<int, ConstructionProperties>> getAllConstructionPropertiesAsync() {
        using var connection = getConnection();
        const string query = @"
            SELECT * FROM ConstructionProperties;";
        var result = await connection.QueryAsync<ConstructionProperties>(query);
        return result.ToDictionary(r => r.id, r => r);
    }

    public async Task<List<(int, int, int, int)>> getAllBlocksToConstructionAsync() {
        using var connection = getConnection();
        const string query = @"
            SELECT idConstructionProperties AS constructionPropertiesID, idBlockProperty AS blockPropertiesID, stage, quantity as blockQuantity
            FROM BlocksToConstruction;";
        var result = await connection.QueryAsync<(int idConstructionProperties, int idBlockProperty, int stage, int quantity)>(query);
        return result.ToList();
    }

    public async Task<List<(int, int, int)>> getAllConstructionStagesPropertiesAsync() {
        using var connection = getConnection();
        const string query = @"
            SELECT * FROM ConstructionStages;
        ";
        var result = await connection.QueryAsync<(int idConstructionProperties, int stage, int time)>(query);
        return result.ToList();
    }

    public async Task<bool> updateConstructionInstanceAsync(int constructionID, ConstructionState state, int constructionPropertiesID, int userID) {
        using var connection = getConnection();
        const string query = @"
            UPDATE Constructions
            SET sate = @State, idConstructionProperties = @ConstructionPropertiesID, userID = @UserID
            WHERE id = @Id";
        int rowsAffected = await connection.ExecuteAsync(query, new { State = state, ConstructionPropertiesID = constructionPropertiesID, UserID = userID, Id = constructionID });
        return rowsAffected > 0;
    }

    public async Task<bool> deleteConstructionInstanceAsync(int id) {
        using var connection = getConnection();
        const string query = "DELETE FROM Constructions WHERE id = @Id";
        int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
        return rowsAffected > 0;
    }
    #endregion

    public async Task<Dictionary<string, int>> getBlocksNeededAsync(int constructionPropertiesID) {
        using var connection = getConnection();
        const string query = @"
            SELECT bp.name, SUM(bc.quantity)
            FROM BlocksToConstruction bc
            INNER JOIN ConstructionProperties cp ON cp.id = bc.idConstructionProperties
            INNER JOIN BlockProperties bp ON bp.id = bc.idBlockProperty
            WHERE cp.id = @Id
            GROUP BY bp.name;";
        var result = await connection.QueryAsync<(string Name, int Quantity)>(query, new { id = constructionPropertiesID });
        return result.ToDictionary(r => r.Name, r => r.Quantity);
    }

    public async Task<List<int>> getConstructionsOfStateIdsAsync(int userID, int state) {
        using var connection = getConnection();
        string? dbState = Enum.GetName(typeof(ConstructionState), state);
        const string query = @"
            SELECT id
            FROM Constructions
            WHERE state = @dbState AND idUser = @userID;
        ";
        var res = await connection.QueryAsync<int>(query, new {dbState, userID});
        return res.ToList();
    }

    public async Task<List<(int, int, int)>> getUserIdAndConstructionsIdOfStateAsync(int state) {
        using var connection = getConnection();
        string? dbState = Enum.GetName(typeof(ConstructionState), state);
        const string query = @"
            SELECT id, idConstructionProperties, idUser
            FROM Constructions
            WHERE state = @dbState;
        ";
        var res = await connection.QueryAsync<(int idConstruction, int idConstructionProperties, int idUser)>(query, new { dbState });
        return res.ToList();
    }

    public async Task<Dictionary<int, int>> getConstructionsOfStateAsync(int userID, int state) {
        using var connection = getConnection();
        string? dbState = Enum.GetName(typeof(ConstructionState), state);
        const string query = @"
            SELECT cp.id, COUNT(c.idConstructionProperties) AS quantity
            FROM Constructions c 
            INNER JOIN ConstructionProperties cp ON c.idConstructionProperties = cp.id 
            WHERE c.state = @dbState AND c.idUser = @userID
            GROUP BY (cp.id);
        ";
        var res = await connection.QueryAsync<(int id, int quantity)>(query, new { dbState, userID});
        return res.ToDictionary(r => r.id, r => r.quantity);
    }

    public async Task<List<int>> getBuildingIdsConstructionsAsync(int idUser, int idConstructionProperties) {
        using var connection = getConnection();
        const string query = @"
            SELECT c.id
            FROM Constructions c 
            INNER JOIN ConstructionProperties cp ON c.idConstructionProperties = cp.id 
            WHERE c.state = 'BUILDING' AND c.idUser = @IdUser AND c.idConstructionProperties = @IdConstructionProperties;
        ";
        var res = await connection.QueryAsync<int>(query, new { idUser, idConstructionProperties });
        return res.ToList();
    }

    public async Task<Dictionary<string, int>> getCompletedConstructionBlocksAsync(int userId, int constructionId) {
        using var connection = getConnection();
        const string query = @"
            SELECT bp.name, SUM(btc.quantity)
            FROM Constructions c
            INNER JOIN BlocksToConstruction btc ON c.idConstructionProperties = btc.idConstructionProperties
            INNER JOIN BlockProperties bp ON btc.idBlockProperty = bp.id
            WHERE c.id = @constructionId AND c.idUser = @userId
            GROUP BY bp.name;
            ";

        var res = await connection.QueryAsync<(string name, int quantity)>(query, new { userId, constructionId });
        return res.ToDictionary(r => r.name, r => r.quantity);
    }

    public async Task<bool> updateConstructionStateAsync(int idConstruction, int state) {
        using var connection = getConnection();
        string? dbState = Enum.GetName(typeof(ConstructionState), state);
        const string query = @"
            UPDATE Constructions
            SET state = @state
            WHERE id = @idConstruction;
        ";

        int rowsAffected = await connection.ExecuteAsync(query, new { state = dbState, idConstruction });
        return rowsAffected > 0;
    }

    public async Task<bool> removeConstructionInWaitingAsync(int idUser, int idConstructionProperties) {
        using var connection = getConnection();
        await connection.OpenAsync(); // Ensure connection is explicitly opened
        using var transaction = await connection.BeginTransactionAsync();

        try {
            // see if waiting
            const string checkQuery = @"
                SELECT state, idConstructionProperties 
                FROM Constructions 
                WHERE idConstructionProperties  = @IdConstruction AND idUser = @IdUser AND state = 'WAITING'
            ";
            var construction = await connection.QueryFirstOrDefaultAsync<(string, int)>(
                checkQuery,
                new { IdConstruction = idConstructionProperties, IdUser = idUser },
                transaction
            );

            if (construction == (null, 0)) {
                throw new InvalidOperationException("The user does not have the construction.");
            }
            if (construction.Item1 != "WAITING") {
                throw new InvalidOperationException("Construction is not in 'waiting' state or does not exist.");
            }

            // get blocks of the construction
            const string blocksQuery = @"
                SELECT idBlockProperty, SUM(quantity) 
                FROM BlocksToConstruction 
                WHERE idConstructionProperties = @IdConstructionProperties
                GROUP BY idBlockProperty
            ";
            var blocks = await connection.QueryAsync<(int idBlockProperty, int quantity)>(
                blocksQuery,
                new { IdConstructionProperties = construction.Item2 },
                transaction
            );

            // add blocks to user
            foreach (var block in blocks) {
                const string updateStockQuery = @"
                UPDATE Blocks 
                SET quantity = quantity + @Quantity 
                WHERE idUser = @IdUser AND idBlockProperty = @IdBlockProperty";

                int rowsAffected = await connection.ExecuteAsync(
                    updateStockQuery,
                    new { IdUser = idUser, Quantity = block.quantity, IdBlockProperty = block.idBlockProperty },
                    transaction
                );

                // If no rows were updated, it means the block doesn't exist; insert it
                if (rowsAffected == 0) {
                    const string insertStockQuery = @"
                    INSERT INTO Blocks (idUser, idBlockProperty, quantity)
                    VALUES (@IdUser, @IdBlockProperty, @Quantity)";

                    await connection.ExecuteAsync(
                        insertStockQuery,
                        new { IdUser = idUser, IdBlockProperty = block.idBlockProperty, Quantity = block.quantity },
                        transaction
                    );
                }
            }

            // remove the construction
            const string deleteQuery = @"
                DELETE TOP(1) 
                FROM Constructions 
                WHERE idConstructionProperties = @IdConstruction AND state = 'WAITING' AND idUser = @IdUser";
            await connection.ExecuteAsync(deleteQuery, new { IdConstruction = idConstructionProperties, IdUser = idUser }, transaction);

            // Commit the transaction
            await transaction.CommitAsync();

            return true;
        } catch (Exception) {
            // Rollback the transaction in case of error
            await transaction.RollbackAsync();
            throw; /// future exception
        }
    }

    public async Task<int> addConstructionToQueueAsync(Dictionary<int, int> blocksNeeded, int userID, int constructionPropertiesID) {
        using var connection = getConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try {
            const string removeBlocksQuery = @"
                UPDATE Blocks
                SET quantity = quantity - @quantity
                WHERE idUser = @userID AND idBlockProperty = @blockPropertyID
                AND quantity >= @quantity;";

            foreach (var block in blocksNeeded) {
                var affectedRows = await connection.ExecuteAsync(
                    removeBlocksQuery,
                    new { quantity = block.Value, userID, blockPropertyID = block.Key },
                    transaction
                );

                if (affectedRows == 0) {
                    throw new InvalidOperationException($"Not enough stock for block {block.Key}.");
                }
            }

            const string addConstructionQuery = @"
                INSERT INTO Constructions (state, idConstructionProperties, idUser)
                VALUES (@state, @constructionPropertiesID, @userID);
                SELECT SCOPE_IDENTITY();";

            var instanceID = await connection.ExecuteScalarAsync<int?>(
                addConstructionQuery,
                new { state = ConstructionState.WAITING.ToString(), constructionPropertiesID, userID },
                transaction
            );

            await transaction.CommitAsync();
            return (int) instanceID;
        } catch (Exception ex) {
            await transaction.RollbackAsync();
            Console.WriteLine($"Transaction failed: {ex.Message}");
            throw;
        }
    }

    public async Task<int> getConstructionPropertyIdAsync(int idConstruction) {
        using var connection = getConnection();
        const string query = @"
            SELECT idConstructionProperties 
            FROM Constructions 
            WHERE id = @ConstructionId;
        ";
        return await connection.QuerySingleAsync<int>(query, new { ConstructionId = idConstruction });
    }


    //public async Task<Dictionary<int, string>> getConstructionsAsync() {
    //    using var connection = getConnection();
    //    const string query = @"
    //    SELECT id, name
    //    FROM ConstructionProperties
    //    ";
    //    var res = await connection.QueryAsync<(int id, string name)>(query);
    //    return res.ToDictionary(r => r.id, r => r.name);
    //}
}
