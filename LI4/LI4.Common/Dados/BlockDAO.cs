using Dapper;
using LI4.Common.Exceptions.UserExceptions;
using LI4.Dados;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LI4.Common.Dados;

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
/// See LI4.Client#Pages.Users for more examples.
/// </summary>
public class BlockDAO {
    private readonly string connectionString;

    public BlockDAO(string conStr) {
        this.connectionString = conStr;
    }

    private SqlConnection getConnection() => new SqlConnection(this.connectionString);

    public async Task<Block> getByIdAsync(int id) {
        
        // Problema, o bloco tem mais informação do que a retirada por aqui (tenho que aceder a propriedadeBloco)
        using var connection = getConnection();
        const string query = "SELECT * FROM Utilizador WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Block>(query, new { id = id });
    }
}
