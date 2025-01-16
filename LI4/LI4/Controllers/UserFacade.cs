using LI4.Controllers.DAO;
using LI4.Dados;

namespace LI4.Controllers;

public class UserFacade {
    private UserDAO userDAO;

    public UserFacade(ConfigurationManager config) {
        this.userDAO = new UserDAO(config.GetConnectionString("DefaultConnection"));
    }

    public async Task<IEnumerable<User>> getAllAsync() {
        return await userDAO.getAllAsync();
    }

    public async Task<bool> createUser(string email, string username, string password) {
        if (email == null || username == null || password == null) { return false; }
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) { return false; }

        bool emailInUse = await userDAO.emailExistsAsync(email);
        bool usernameInUse = await userDAO.usernameExistsAsync(username);

        if (!emailInUse && !usernameInUse) {
            User tmpUser = new User(username, email, password);
            await userDAO.addAsync(tmpUser);
            return true;
        };

        return false;
    }

    public async Task<bool> validateUser(string email, string password) {

        if (email == null || password == null) { return false; }
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) { return false; }


        bool emailExists = await userDAO.emailExistsAsync(email);
        if (emailExists) {

            bool athenticate = await userDAO.authenticateAsync(email, password);
            return athenticate;
        }
        return false;
    }

    public async Task<User> getUserByEmail(string email) {
        return await this.userDAO.getByEmailAsync(email);
    }

    public async Task<bool> updateUser(int id, string email, string username, string password) {
        var account = await userDAO.getByIdAsync(id);
        bool usernameExists = await userDAO.usernameNoOtherExistsAsync(id, username);
        if ((account != null) && usernameExists) {

            account.email = email;
            account.username = username;
            account.userPassword = password;

            bool result = await userDAO.updateAsync(account);
            return result;
        }
        return false;
    }
}
