using LI4.Dados;

namespace LI4.Controllers;

public class MineBuildsLN : Common.IMineBuildsLN {
    private UserFacade userFacade;

    public MineBuildsLN(ConfigurationManager config) {
        this.userFacade = new UserFacade(config);
    }

    public async Task<bool> authenticate(string username, string password) {
        return await userFacade.validarUser(username, password);
    }

    public async Task<Utilizador> getUser(string email) {
        return await userFacade.getUser(email);
    }

    public bool updateUtilizador(string email, string username, string password) {
        return userFacade.updateUser(email, username, password);
    }
}
