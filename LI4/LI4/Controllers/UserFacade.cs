﻿using LI4.Dados;

namespace LI4.Controllers;

public class UserFacade {
    private UserDAO userDAO;

    public UserFacade(ConfigurationManager config) {
        this.userDAO = new UserDAO(config.GetConnectionString("DefaultConnection"));
    }

    public async Task<bool> validarUser(string email, string password) {

        if (email == null || password == null) { return false; }
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) { return false; }


        bool emailExists = await userDAO.emailExistsAsync(email);
        if (emailExists) {

            bool athenticate = await userDAO.authenticateAsync(email, password);
            return athenticate;
        }
        return false;
    }

    public async Task<Utilizador> getUser(string email) {
        return await this.userDAO.getAsync(email);
    }

    public bool updateUser(string email, string username, string password) {
        return Task.Run(async () => {
            if (!await userDAO.emailExistsAsync(email)) {
                throw new ArgumentException("The account doesn't exist.");
            }

            if (await userDAO.usernameExistsAsync(username)) {
                throw new ArgumentException("The username is already in use.");
            }

            var allUsers = await userDAO.getAllAsync();
            var userToUpdate = allUsers.FirstOrDefault(u => u.email == email);

            if (userToUpdate == null) {
                throw new InvalidOperationException("The user to update was not found.");
            }

            userToUpdate.email = email;
            userToUpdate.username = username;
            userToUpdate.palavraPasse = password;

            bool result = await userDAO.updateAsync(userToUpdate);
            return result;
        }).Result;
    }
}
