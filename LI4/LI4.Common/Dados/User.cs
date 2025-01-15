using System.Diagnostics.CodeAnalysis;

namespace LI4.Dados;

public class User {
    public int id { get; set; }
    public required string username { get; set; }
    public required string email { get; set; }
    public required string userPassword { get; set; }

    public User() {

    }


    [SetsRequiredMembers]
    public User(string username, string email, string palavraPasse) {
        this.id = 0;
        this.username = username;
        this.email = email;
        this.userPassword = palavraPasse;
    }

    [SetsRequiredMembers]
    public User(int id, string username, string email, string palavraPasse) {
        this.id = id;
        this.username = username;
        this.email = email;
        this.userPassword = palavraPasse;
    }

    public static User empty() {
        return new User(0, "", "", "");
    }

    public static User from(User utilizador) {
        return new User(
            utilizador.id,
            utilizador.username,
            utilizador.email,
            utilizador.userPassword
        );
    }

    public override string ToString() => "Utilizador{"
        + "id=" + this.id + ", "
        + "username=" + this.username + ", "
        + "email=" + this.email + ", "
        + "userPassword=" + this.userPassword
        + "}";
}
