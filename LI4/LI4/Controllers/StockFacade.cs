using LI4.Common.Dados;
using LI4.Controllers.DAO;
using LI4.Dados;
using System.Diagnostics.Contracts;

namespace LI4.Controllers;

public class StockFacade {
    private OrderDAO orderDAO;
    private BlockDAO blockDAO;
    private Dictionary<int, BlockProperties> blockProperties;

    public StockFacade(ConfigurationManager config) {
        this.blockDAO = new BlockDAO(config.GetConnectionString("DefaultConnection"));
        this.orderDAO = new OrderDAO(config.GetConnectionString("DefaultConnection"));
        this.blockProperties = new Dictionary<int, BlockProperties>();
    }

    public async Task initializeBlockPropertiesAsync() {
        try {
            var blockProperties = await blockDAO.getAllBlockPropertiesAsync();
            this.blockProperties = blockProperties.ToDictionary(b => b.Value.id, b => b.Value);
        } catch (Exception ex) {
            throw new Exception("Failed to initialize Block Properties", ex);
        }
    }

    #region//---- INTERNAL STRUCTS ----//
    public BlockProperties getBlockProperties(int blockPropertiesID) {
        return this.blockProperties[blockPropertiesID];
    }
    #endregion

    #region//---- ORDER METHODS ----//
    public async Task<Dictionary<string, int>> getOrderContentAsync(int id) {
        return await orderDAO.getOrderContentAsync(id);
    }

    public async Task<int> makeOrderAsync(int idUser, Dictionary<int, int> blocks) {
        int estimatedTime = 0;
        Order order = new Order(idUser, DateTime.Now);
        int orderId = await orderDAO.addAsync(order);

        foreach (KeyValuePair<int, int> entry in blocks) { 
            int idBlockProperties = entry.Key;
            int blockQuantity = entry.Value;

            int timeToAcquire = await blockDAO.getTimeToAcquireById(idBlockProperties);
            estimatedTime += blockQuantity * timeToAcquire;

            await orderDAO.addBlocksInOrder(orderId, idBlockProperties, blockQuantity);
            // TODO: Add to Blocks table, but needs control time (estimatedTime)
        }
        return estimatedTime;
    }

    public async Task<List<Order>> getOrdersUser(int id) {
        return await orderDAO.getUserOrdersAsync(id);
    }
    #endregion

    #region//---- BLOCK METHODS----//
    public async Task<Block?> getBlockByIdAsync(int id) {
        return await blockDAO.getBlockByIdAsync(id);
    }

    public async Task<IEnumerable<Block>> getAllBlocksAsync() {
        return await blockDAO.getAllBlockInstancesAsync();
    }

    public async Task<Dictionary<string, int>> getStockUser(int id) {
        return await blockDAO.getAllBlocksUser(id);
    }
    #endregion
}
