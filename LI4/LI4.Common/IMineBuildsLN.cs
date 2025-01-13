namespace LI4.Common;
public interface IMineBuildsLN {

    public bool authenticate(string username, string password);
    public bool updateUtilizador(string email, string username,string password);
}
