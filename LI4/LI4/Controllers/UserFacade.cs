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

    public bool updateUser(string email, string username, string password) {
        return Task.Run(async () => {
            if (!await userDAO.EmailExistsAsync(email)) {
                throw new ArgumentException("The account doesn't exist.");
            }

            if (await userDAO.UsernameExistsAsync(username)) {
                throw new ArgumentException("The username is already in use.");
            }

            var allUsers = await userDAO.GetAllAsync();
            var userToUpdate = allUsers.FirstOrDefault(u => u.email == email);

            if (userToUpdate == null) {
                throw new InvalidOperationException("The user to update was not found.");
            }

            userToUpdate.email = email;
            userToUpdate.username = username;
            userToUpdate.palavraPasse = password;

            bool result = await userDAO.UpdateAsync(userToUpdate);
            return result;
        }).Result;
    }
}
