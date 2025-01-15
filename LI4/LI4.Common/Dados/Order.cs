
using System.Diagnostics.CodeAnalysis;

namespace LI4.Dados;

public class Order {
    public int id { get; set; }
    public required string idUtilizador { get; set; }
    public required DateTime orderDate { get; set; }

    public Order() { }

    [SetsRequiredMembers]
    public Order(string idUtilizador, DateTime orderDate) {
        this.id = 0;
        this.idUtilizador = idUtilizador;
        this.orderDate = orderDate;
    }

    [SetsRequiredMembers]
    public Order(int id, string idUtilizador, DateTime orderDate) {
        this.id = id;
        this.idUtilizador = idUtilizador;
        this.orderDate = orderDate;
    }

    public static Order empty() {
        return new Order(0, "", DateTime.MinValue);
    }

    public static Order from(Order order) {
        return new Order(
            order.id,
            order.idUtilizador,
            order.orderDate
        );
    }

    public override string ToString() => "Order{"
        + "id=" + this.id + ", "
        + "idUtilizador=" + this.idUtilizador + ", "
        + "orderDate=" + this.orderDate.ToString("yyyy-MM-dd HH:mm:ss")
        + "}";
}
