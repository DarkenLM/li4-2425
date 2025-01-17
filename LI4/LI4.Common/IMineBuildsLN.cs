using LI4.Common.Dados;
using LI4.Dados;

namespace LI4.Common;
public interface IMineBuildsLN {

    #region//---- USER METHODS ----//
    public Task<IEnumerable<User>> getAllUsersAsync();

    public Task<bool> authenticate(string email, string password);
    
    public Task<bool> updateUtilizador(int id, string email, string username,string password);

    public Task<User> getUserByEmail(string email);

    public Task<bool> registerUser(string email, string username, string password);
    #endregion

    #region//---- STOCK METHODS----//
    public Task<IEnumerable<Block>> getAllBlocksAsync();

    public Task<Dictionary<string, int>> getOrderContentAsync(int id);

    public Task<List<Order>> getOrders(int id);

    public Task<Dictionary<string, int>> getStock(int id);

    public Task<int> createOrderAsync(int id, Dictionary<int, int> blocks);
    #endregion

    #region//---- CONSTRUCTION METHODS ----//
    public Task<IEnumerable<Construction>> getAllConstructionInstancesAsync();

    public Task<bool> hasStock(int constructionPropertyID, int userID);

    public Task<bool> addConstructionToQueue(int userID, int constructionPropertyID);

    public Task<Dictionary<string, int>> calculateMissingBlocks(int userdID, int constructionPropertyID);

    public Task<Dictionary<string, int>> getAwaitingConstructions(int userID);

    public Task<Dictionary<string, int>> getCompletedConstructions(int userID);

    public Task<Dictionary<string, int>> getBuildingConstructions(int userID);

    public Task<Dictionary<string, int>> viewCompletedConstruction(int userId, int constructionId);

    public Task<bool> removeConstruction(int idUser, int idConstruction);

    public Task<Dictionary<int, string>> getCatalog();
    
    #endregion
}
