using LI4.Dados;

namespace LI4.Common;
public interface IMineBuildsLN {

    public Task<bool> authenticate(string email, string password);
    
    public Task<bool> updateUtilizador(string emailId, string email, string username,string password);

    public Task<User> getUserByEmail(string email);

    public Task<bool> registerUser(string email, string username, string password);
}
