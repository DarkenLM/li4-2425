using LI4.Dados;

namespace LI4.Common;
public interface IMineBuildsLN {

    public bool authenticate(string username, string password);
    public bool updateUtilizador(string email, string username,string password);

    public Task<Utilizador> getUser(string email);
}
