using Dapper;
using LI4.Common.Dados;
using LI4.Common.Exceptions.ConstructionExceptions;
using LI4.Dados;
using Microsoft.Data.SqlClient;

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

    public async Task<Dictionary<string, int>> getBlocksNeeded(int constructionPropertyID) {
        using var connection = getConnection();
        const string query = @"
            SELECT bp.name, bc.quantity
            FROM BlocksToConstruction bc
            INNER JOIN ConstructionProperties cp ON cp.id = bc.idConstructionProperties
            INNER JOIN BlockProperties bp ON bp.id = bc.idBlockProperty
            WHERE cp.id = @Id;";
        var result = await connection.QueryAsync<(string Name, int Quantity)>(query, new { id = constructionPropertyID });
        return result.ToDictionary(r => r.Name, r => r.Quantity);
    }

    public async Task<Dictionary<string, int>> getConstructionsOfState(int userID, int state) {
        using var connection = getConnection();
        string? dbState = Enum.GetName(typeof(ConstructionState), state);
        const string query = @"
            SELECT cp.name, COUNT(c.idConstructionProperties)
            FROM Constructions c 
            INNER JOIN ConstructionProperties cp ON c.idConstructionProperties = cp.id 
            WHERE c.state = @dbState AND c.idUser = @userID
            GROUP BY (cp.name);
        ";
        var res = await connection.QueryAsync<(string name, int quantity)>(query, new { dbState, userID});
        return res.ToDictionary(r => r.name, r => r.quantity);
    }

    public async Task<Dictionary<string, int>> getCompletedConstructionBlocks(int userId, int constructionId) {
        using var connection = getConnection();

        const string query = @"
        SELECT b.name, COUNT(b.id) 
        FROM Blocks b
        INNER JOIN Constructions c ON b.constructionId = c.id
        WHERE c.id = @constructionId AND c.idUser = @userId
        GROUP BY b.name;
    ";

        var res = await connection.QueryAsync<(string name, int quantity)>(query, new { userId, constructionId });
        return res.ToDictionary(r => r.name, r => r.quantity);
    }


}
