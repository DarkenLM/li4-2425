using LI4.Dados;

namespace LI4.Controllers;

public class MineBuildsLN : Common.IMineBuildsLN {
    private UserFacade userFacade;
    private StockFacade stockFacade;

    public MineBuildsLN(ConfigurationManager config) {
        this.userFacade = new UserFacade(config);
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
}
