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
    #endregion//---- END ----/

}
