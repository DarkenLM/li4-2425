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

    public async Task initializeOrdersAsync() {
        try {
            List<(int idOrder, int idUser, int idBlockProperties, int quantity)> orderLines = await orderDAO.getRemainingOrders();
            //Dictionary<idOrder, (idUser, Dictionary<idBlockProperties, quantity>)> 
            Dictionary<int, KeyValuePair<int, Dictionary<int, int>>> remainingOrders = new();

            // Tratamento do output da query
            foreach(var line in orderLines) {
                if (remainingOrders.ContainsKey(line.idOrder)) {
                    remainingOrders[line.idOrder].Value.Add(line.idBlockProperties, line.quantity);
                } else {
                    Dictionary<int, int> blocks = new();
                    blocks.Add(line.idBlockProperties, line.quantity);
                    KeyValuePair<int, Dictionary<int, int>> remainingBlocksForUser = new(line.idUser, blocks);
                    remainingOrders.Add(line.idOrder, remainingBlocksForUser);
                }
            }

            // foreach (var entry in remainingOrders) {
            //     int idOrder = entry.Key;
            //     int idUser = entry.Value.Key;
            //     Dictionary<int, int> blocks = entry.Value.Value;

            //     Console.WriteLine($"idOrder: {idOrder}, idUser: {idUser}, blocks:");
            //     foreach(var block in blocks) {
            //         Console.WriteLine($"BlockProperties: {block.Key}, Quantity: {block.Value}");
            //     }
            // }

            // Inicialização do comportamento
            foreach (var entry in remainingOrders) {
                int idOrder = entry.Key;
                int idUser = entry.Value.Key;
                Dictionary<int, int> blocks = entry.Value.Value;
                int estimatedTime = 0;

                foreach (KeyValuePair<int, int> blockPair in blocks) { 
                    int idBlockProperties = blockPair.Key;
                    int blockQuantity = blockPair.Value;

                    int timeToAcquire = blockProperties[idBlockProperties].timeToAcquire;
                    estimatedTime += blockQuantity * timeToAcquire;
                }

                // Console.WriteLine($"Iniciado timer para order {idOrder} de {estimatedTime} segundos.");
                TimerWrapper tw = new TimerWrapper(estimatedTime * 1000, async () => await addStockFromOrderAsync(idUser, blocks, idOrder), false);
                tw.start();
            }
        } catch (Exception ex) {
            throw new Exception("Failed to initialize orders", ex);
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


    private async Task addStockFromOrderAsync(int idUser, Dictionary<int, int> blocks, int idOrder) {
        // Console.WriteLine($"Encomenda do jogador '{idUser}' processada em: {DateTime.Now}");
        foreach (var entry in blocks) {
            await blockDAO.addBlockInstanceAsync(idUser, entry.Key, entry.Value);
            // Console.WriteLine("Adicionado ao Stock: ");
            // Console.WriteLine($"Propriedade do Bloco: {entry.Key}. Quantidade: {entry.Value}.");
        }

        await orderDAO.updateOrderDeliveredAsync(idOrder, true);
    }


    public async Task<int> createOrderAsync(int idUser, Dictionary<int, int> blocks) {
        int estimatedTime = 0;
        Order order = new Order(idUser, DateTime.Now, false);
        int orderId = await orderDAO.addAsync(order);

        foreach (KeyValuePair<int, int> entry in blocks) { 
            int idBlockProperties = entry.Key;
            int blockQuantity = entry.Value;

            int timeToAcquire = blockProperties[idBlockProperties].timeToAcquire;
            estimatedTime += blockQuantity * timeToAcquire;

            await orderDAO.addBlocksInOrderAsync(orderId, idBlockProperties, blockQuantity);
        }
        // Console.WriteLine("Tempo que vai demorar: " + estimatedTime * 1000 + " milisegundos.");
        // Console.WriteLine("Comecei em: " + DateTime.Now);

        TimerWrapper tw = new TimerWrapper(estimatedTime * 1000, async () => await addStockFromOrderAsync(idUser, blocks, orderId), false);
        tw.start();

        return estimatedTime;
    }

    public async Task<List<Order>> getUserOrdersAsync(int id) {
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

    public async Task<Dictionary<string, int>> getUserStockAsync(int id) {
        return await blockDAO.getAllUserBlocksAsync(id);
    }
    #endregion
}
