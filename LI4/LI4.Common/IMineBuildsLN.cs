﻿using LI4.Dados;

namespace LI4.Common;
public interface IMineBuildsLN {

    public Task<bool> authenticate(string username, string password);
    
    public Task<bool> updateUtilizador(string emailId, string email, string username,string password);

    public Task<Utilizador> getUserByEmail(string email);

    public Task<bool> registerUser(string email, string username, string password);
}
