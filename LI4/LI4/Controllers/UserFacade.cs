using LI4.Common.Exceptions.UserExceptions;
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

    public async Task<User> validateUser(string email, string password) {

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) {
            throw new UserNotFoundException("You can't pass a null email or password to verify for a user!"); 
        }


        bool emailExists = await userDAO.emailExistsAsync(email);
        if (emailExists) {
            User user = await userDAO.authenticateAsync(email, password);
            return user;
        } else {
            throw new UserNotFoundException("There was an error while getting the user.");
        }
    }

    public async Task<User> getUserByEmail(string email) {
        return await this.userDAO.getByEmailAsync(email);
    }

    public async Task<User> updateUser(int id, string email, string username, string password) {
        User account = await userDAO.getByIdAsync(id);
        bool emailNoExists = await userDAO.emailNoOtherExistsAsync(id, email);
        bool usernameNoExists = await userDAO.usernameNoOtherExistsAsync(id, username);
        if ((account != null) && usernameNoExists && emailNoExists) {

            account.email = email;
            account.username = username;
            account.userPassword = password;

            bool res = await userDAO.updateAsync(account);
            if (res) {
                return account;
            } else {
                throw new UserNotFoundException("Error while updating user info.");
            }
        } else {
            throw new UserAlreadyExistsException("Another user has already the email or username given.");
        }
    }
}
