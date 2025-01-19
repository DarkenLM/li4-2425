using LI4.Common.Dados;
using LI4.Controllers.DAO;
using LI4.Dados;


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

    public int getBlockPropertiesIDbyName(string name) {
        foreach (var blockProperties in this.blockProperties.Values) { 
            if(blockProperties.name == name){
                return blockProperties.id;
            }
        }
        return -1;
    }
    #endregion

    #region//---- ORDER METHODS ----//
    public async Task<Dictionary<string, int>> getOrderContentAsync(int id) {
        return await orderDAO.getOrderContentAsync(id);
    }


    private async Task addStockFromOrder(int idUser, Dictionary<int, int> blocks) {
        // Console.WriteLine($"Encomenda do jogador '{idUser}' processada em: {DateTime.Now}");
        foreach (var entry in blocks) {
            await blockDAO.addBlockInstanceAsync(idUser, entry.Key, entry.Value);
            // Console.WriteLine("Adicionado ao Stock: ");
            // Console.WriteLine($"Propriedade do Bloco: {entry.Key}. Quantidade: {entry.Value}.");
        }
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
        }
        // Console.WriteLine("Tempo que vai demorar: " + estimatedTime * 1000 + " milisegundos.");
        // Console.WriteLine("Comecei em: " + DateTime.Now);

        TimerWrapper tw = new TimerWrapper(estimatedTime * 1000, async () => await addStockFromOrder(idUser, blocks), false);
        tw.start();

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
