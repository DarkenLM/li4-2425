using System.Diagnostics.CodeAnalysis;

namespace LI4.Dados;

public class Utilizador {
    public int id { get; set; }
    public required string username { get; set; }
    public required string email { get; set; }
    public required string palavraPasse { get; set; }

    public Utilizador() {

    }


    [SetsRequiredMembers]
    public Utilizador(string username, string email, string palavraPasse) {
        this.id = 0;
        this.username = username;
        this.email = email;
        this.palavraPasse = palavraPasse;
    }

    [SetsRequiredMembers]
    public Utilizador(int id, string username, string email, string palavraPasse) {
        this.id = id;
        this.username = username;
        this.email = email;
        this.palavraPasse = palavraPasse;
    }

    public static Utilizador empty() {
        return new Utilizador(0, "", "", "");
    }

    public static Utilizador from(Utilizador utilizador) {
        return new Utilizador(
            utilizador.id,
            utilizador.username,
            utilizador.email,
            utilizador.palavraPasse
        );
    }

    public override string ToString() => "Utilizador{"
        + "id=" + this.id + ", "
        + "username=" + this.username + ", "
        + "email=" + this.email + ", "
        + "palavraPasse=" + this.palavraPasse
        + "}";
}
