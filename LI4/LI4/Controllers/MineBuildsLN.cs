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

    //---- User Related Methods ----//
    public async Task<IEnumerable<User>> getAllAsync() {
        return await userFacade.getAllAsync();
    }

    public async Task<bool> authenticate(string email, string password) {
        return await userFacade.validateUser(email, password);
    }

    public async Task<User> getUserByEmail(string email) {
        return await userFacade.getUserByEmail(email);
    }

    public async Task<bool> updateUtilizador(string emailId, string email, string username, string password) {
        return await userFacade.updateUser(emailId, email, username, password);
    }

    public async Task<bool> registerUser(string email, string username, string password) {
        return await userFacade.createUser(email, username, password);
    }

    //---- Stock Related Methods ----//
    public async Task<Dictionary<string, int>> getOrderContentAsync(int id) {
        return await stockFacade.getOrderContentAsync(id);
    }
    public async Task<List<Order>> getOrders(string email) {
        return await stockFacade.getOrdersUser(email);
    }

    public async Task<Dictionary<string, int>> getStock(string email) {
        var user = await userFacade.getUserByEmail(email);
        return await stockFacade.getStockUser(user.id);
    }

    //---- Construction Related Methods ----//
}
