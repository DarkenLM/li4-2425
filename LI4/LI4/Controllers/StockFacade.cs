using LI4.Common.Dados;
using LI4.Controllers.DAO;
using LI4.Dados;
using System.Diagnostics.Contracts;

namespace LI4.Controllers;

public class StockFacade {
    private OrderDAO orderDAO;
    private BlockDAO blockDAO;

    public StockFacade(ConfigurationManager config) {
        this.blockDAO = new BlockDAO(config.GetConnectionString("DefaultConnection"));
        this.orderDAO = new OrderDAO(config.GetConnectionString("DefaultConnection"));
    }
    
    //---- Order related Methods ----//
    public async Task<Dictionary<string, int>> getOrderContentAsync(int id) {
        return await orderDAO.getOrderContentAsync(id);
    }

    public async Task<List<Order>> getOrdersUser(string email) {
        return await orderDAO.getOrdersAsync(email);
    }

    //---- Block related Methods ----//
    public async Task<Block?> getBlockByIdAsync(int id) {
        return await blockDAO.getByIdAsync(id);
    }

    public async Task<IEnumerable<Block>> getAllBlocksAsync() {
        return await blockDAO.getAllAsync();
    }

    public async Task<Dictionary<string, int>> getStockUser(int id) {
        return await blockDAO.getAllBlocksUser(id);
    }
}
