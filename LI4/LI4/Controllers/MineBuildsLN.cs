namespace LI4.Controllers;

public class MineBuildsLN : Common.IMineBuildsLN {
    private UserFacade userFacade;

    public MineBuildsLN(ConfigurationManager config) {
        this.userFacade = new UserFacade(config);
    }

    public bool authenticate(string username, string password) {
        return userFacade.validarUser(username, password);
    }
}
