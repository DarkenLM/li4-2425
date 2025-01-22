
using System.Diagnostics.CodeAnalysis;

namespace LI4.Dados;

public class Order {
    public int id { get; set; }
    public required int idUser { get; set; }
    public required DateTime orderDate { get; set; }
    public required bool delivered { get; set; }

    public Order() { }

    [SetsRequiredMembers]
    public Order(int idUser, DateTime orderDate, bool delivered) {
        this.id = 0;
        this.idUser = idUser;
        this.orderDate = orderDate;
        this.delivered = delivered;
    }

    [SetsRequiredMembers]
    public Order(int id, int idUser, DateTime orderDate, bool delivered) {
        this.id = id;
        this.idUser = idUser;
        this.orderDate = orderDate;
        this.delivered = delivered;
    }

    public static Order empty() {
        return new Order(0, 0, DateTime.MinValue, false);
    }

    public static Order from(Order order) {
        return new Order(
            order.id,
            order.idUser,
            order.orderDate,
            order.delivered
        );
    }

    public override string ToString() => "Order{"
        + "id=" + this.id + ", "
        + "idUser=" + this.idUser + ", "
        + "orderDate=" + this.orderDate.ToString("yyyy-MM-dd HH:mm:ss") + ", "
        + "delivered=" + this.delivered
        + "}";
}
