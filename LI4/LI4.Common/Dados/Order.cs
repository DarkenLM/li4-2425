
using System.Diagnostics.CodeAnalysis;

namespace LI4.Dados;

public class Order {
    public int id { get; set; }
    public required int idUser { get; set; }
    public required DateTime orderDate { get; set; }

    public Order() { }

    [SetsRequiredMembers]
    public Order(int idUser, DateTime orderDate) {
        this.id = 0;
        this.idUser = idUser;
        this.orderDate = orderDate;
    }

    [SetsRequiredMembers]
    public Order(int id, int idUser, DateTime orderDate) {
        this.id = id;
        this.idUser = idUser;
        this.orderDate = orderDate;
    }

    public static Order empty() {
        return new Order(0, 0, DateTime.MinValue);
    }

    public static Order from(Order order) {
        return new Order(
            order.id,
            order.idUser,
            order.orderDate
        );
    }

    public override string ToString() => "Order{"
        + "id=" + this.id + ", "
        + "idUser=" + this.idUser + ", "
        + "orderDate=" + this.orderDate.ToString("yyyy-MM-dd HH:mm:ss")
        + "}";
}
