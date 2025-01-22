using LI4.Common.Dados;
using LI4.Common.Exceptions.ConstructionExceptions;
using LI4.Dados;

namespace LI4.Controllers;

public class MineBuildsLN : Common.IMineBuildsLN {
    private static UserFacade userFacade;
    private static StockFacade stockFacade;
    private static ConstructionFacade constructionFacade;

    static public async Task initStaticData(ConfigurationManager config) {
        stockFacade = new StockFacade(config);
        userFacade = new UserFacade(config);
        constructionFacade = new ConstructionFacade(config);
        await stockFacade.initializeBlockPropertiesAsync();
        await constructionFacade.initializeConstructionPropertiesAsync();
        await constructionFacade.initializeBlocksToConstructions();
    }

    static public async Task initDynamicData(ConfigurationManager config) {
        await constructionFacade.initializeAssemblyLines();
        await stockFacade.initializeOrders();
    }

    public MineBuildsLN(ConfigurationManager config) {
    }

    #region//---- USER METHODS ----//
    public async Task<IEnumerable<User>> getAllUsersAsync() {
        return await userFacade.getAllAsync();
    }

    public async Task<User> authenticate(string email, string password) {
        return await userFacade.validateUser(email, password);
    }

    public async Task<User> getUserByEmail(string email) {
        return await userFacade.getUserByEmail(email);
    }

    public async Task<User> updateUtilizador(int id, string email, string username, string password) {
        return await userFacade.updateUser(id, email, username, password);
    }

    public async Task<bool> registerUser(string email, string username, string password) {
        return await userFacade.createUser(email, username, password);
    }
    #endregion

    #region//---- STOCK METHODS ----//
    public async Task<IEnumerable<Block>> getAllBlocksAsync() {
        return await stockFacade.getAllBlocksAsync();
    }

    public async Task<Dictionary<string, int>> getOrderContentAsync(int id) {
        return await stockFacade.getOrderContentAsync(id);
    }
    public async Task<List<Order>> getOrders(int id) {
        return await stockFacade.getOrdersUser(id);
    }

    public async Task<Dictionary<string, int>> getStock(int id) {
        return await stockFacade.getStockUser(id);
    }

    public async Task<int> createOrderAsync(int id, Dictionary<int, int> blocks) {
        return await stockFacade.makeOrderAsync(id, blocks);
    }

    public BlockProperties getBlockProperties(int blockPropertiesID) {
        return stockFacade.getBlockProperties(blockPropertiesID);
    }
    #endregion

    #region//---- CONSTRUCTION METHODS ----//
    public async Task<IEnumerable<Construction>> getAllConstructionInstancesAsync() {
        return await constructionFacade.getAllConstructionInstancesAsync();
    }

    public async Task<bool> hasStock(int userID, int constructionPropertiesID) {
        var result = await this.calculateMissingBlocks(userID, constructionPropertiesID);
        return (result.ToDictionary().Count()==0);
    }

    public async Task<bool> addConstructionToQueue(int userID, int constructionPropertiesID) {
        if (!await this.hasStock(userID, constructionPropertiesID)) {
            throw new UserHasNotEnoughBlocksException($"User has not enough blocks to build construction with id: {constructionPropertiesID}");
        }

        Dictionary<string, int> blocksNeededByName = await constructionFacade.getBlocksNeeded(constructionPropertiesID);
        Dictionary<int, int> blocksNeededByID = new Dictionary<int, int>();

        foreach (KeyValuePair<string, int> block in blocksNeededByName) {
            int id = stockFacade.getBlockPropertiesIDbyName(block.Key);
            blocksNeededByID[id] = block.Value;
        }

        return await constructionFacade.addConstructionToQueue(blocksNeededByID, userID, constructionPropertiesID);
    }

    public async Task<bool> updateConstructionStateToBuilding(int idConstruction) {
        return await constructionFacade.updateConstructionState(idConstruction, (int) ConstructionState.BUILDING);
    }

    public async Task<Dictionary<string, int>> calculateMissingBlocks(int userdID, int constructionPropertiesID) {
        Dictionary<string, int> missingBlocks = new Dictionary<string, int>();
        Dictionary<string, int> quantityByName = await constructionFacade.getBlocksNeeded(constructionPropertiesID);
        Dictionary<string, int> userStock = await stockFacade.getStockUser(userdID);
        foreach (string blockName in quantityByName.Keys) {
            int quantityNeeded = quantityByName[blockName];
            int quantityInStock = userStock.TryGetValue(blockName, out int stock) ? stock : 0;
            if (quantityInStock < quantityNeeded) {
                missingBlocks[blockName] = quantityNeeded - quantityInStock;
            }
        }
        return missingBlocks;
    }

    public async Task<List<int>> getAwaitingConstructionsIds(int userID) {
        return await constructionFacade.getConstructionsOfStateIds(userID, (int) ConstructionState.WAITING);
    }

    public async Task<Dictionary<string, int>> getAwaitingConstructions(int userID) {
        return await constructionFacade.getConstructionsOfState(userID, (int) ConstructionState.WAITING);
    }

    public async Task<Dictionary<string, int>> getCompletedConstructions(int userID) {
        return await constructionFacade.getConstructionsOfState(userID, (int) ConstructionState.COMPLETED);
    }

    public async Task<Dictionary<string, int>> getBuildingConstructions(int userID) {
        return await constructionFacade.getConstructionsOfState(userID, (int) ConstructionState.BUILDING);
    }

    public async Task<Dictionary<string, int>> viewCompletedConstruction(int userId, int constructionId) {
        return await constructionFacade.getCompletedConstruction(userId, constructionId);
    }

    public async Task<bool> removeConstruction(int idUser, int idConstruction) {
        return await constructionFacade.removeConstructionInWaiting(idUser, idConstruction);
    }

    public async Task<Dictionary<int,string>> getCatalog() {
        return await constructionFacade.getConstructions();
    }

    public ConstructionProperties getConstructionProperties(int constructionPropertiesID) {
        return constructionFacade.getConstructionProperties(constructionPropertiesID);
    }

    public BlocksToConstruction getBlocksToConstruction(int constructionPropertiesID, int blockPropertiesID) {
        Tuple<int,int> blockToConstructionID = Tuple.Create(constructionPropertiesID, blockPropertiesID);
        return constructionFacade.getBlocksToConstruction(blockToConstructionID);
    }
    #endregion
}
