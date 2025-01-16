using LI4.Common.Dados;
using LI4.Controllers.DAO;
using LI4.Dados;

namespace LI4.Controllers;

public class MineBuildsLN : Common.IMineBuildsLN {
    private UserFacade userFacade;
    private StockFacade stockFacade;

    public MineBuildsLN(ConfigurationManager config) {
        this.userFacade = new UserFacade(config);
        this.stockFacade = new StockFacade(config);
    }

    #region//---- USER METHODS ----//
    public async Task<IEnumerable<User>> getAllUsersAsync() {
        return await userFacade.getAllAsync();
    }

    public async Task<bool> authenticate(string email, string password) {
        return await userFacade.validateUser(email, password);
    }

    public async Task<User> getUserByEmail(string email) {
        return await userFacade.getUserByEmail(email);
    }

    public async Task<bool> updateUtilizador(int id, string email, string username, string password) {
        return await userFacade.updateUser(id, email, username, password);
    }

    public async Task<bool> registerUser(string email, string username, string password) {
        return await userFacade.createUser(email, username, password);
    }
    #endregion

    #region//---- STOCK METHODS ----//
    public async Task<IEnumerable<Block>> getAllBlocksAsync() {
        return await stockFacade.getAllBlocksAsync();
    }

    public async Task<Dictionary<string, int>> getOrderContentAsync(int id) {
        return await stockFacade.getOrderContentAsync(id);
    }
    public async Task<List<Order>> getOrders(int id) {
        return await stockFacade.getOrdersUser(id);
    }

    public async Task<Dictionary<string, int>> getStock(int id) {
        return await stockFacade.getStockUser(id);
    }

    public async Task<int> createOrderAsync(int id, Dictionary<int, int> blocks) {
        return await stockFacade.makeOrderAsync(id, blocks);
    } 
    #endregion

    #region//---- CONSTRUCTION METHODS ----//
    #endregion
}
