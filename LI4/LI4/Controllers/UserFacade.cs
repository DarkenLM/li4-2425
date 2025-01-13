using LI4.Dados;

namespace LI4.Controllers;

public class UserFacade {
    private UserDAO userDAO;

    public UserFacade(ConfigurationManager config) {
        this.userDAO = new UserDAO(config.GetConnectionString("DefaultConnection"));
    }

    public bool validarUser(string username, string password) {
        return true;
    }

    public Task<Utilizador> getUser(string email) {
        return this.userDAO.getAsync(email);
    }
}
