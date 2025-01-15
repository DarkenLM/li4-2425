using LI4.Dados;

namespace LI4.Controllers;

public class StockFacade {
    //private BlockDAO blockDAO;
    private OrderDAO orderDAO;
    public StockFacade(ConfigurationManager config) {
        //this.blockDAO = new BlockDAO(config.GetConnectionString("DefaultConnection"));
        this.orderDAO = new OrderDAO(config.GetConnectionString("DefaultConnection"));
    }
}
