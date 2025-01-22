using LI4.Common.Dados;
using LI4.Common.Exceptions.ConstructionExceptions;
using LI4.Dados;

namespace LI4.Controllers;

public class MineBuildsLN : Common.IMineBuildsLN {
    private static UserFacade userFacade;
    private static StockFacade stockFacade;
    private static ConstructionFacade constructionFacade;

    static public async Task initStaticDataAsync(ConfigurationManager config) {
        stockFacade = new StockFacade(config);
        userFacade = new UserFacade(config);
        constructionFacade = new ConstructionFacade(config);
        await stockFacade.initializeBlockPropertiesAsync();
        await constructionFacade.initializeConstructionPropertiesAsync();
        await constructionFacade.initializeBlocksToConstructionsAsync();
    }

    static public async Task initDynamicDataAsync(ConfigurationManager config) {
        await constructionFacade.initializeAssemblyLinesAsync();
        await stockFacade.initializeOrdersAsync();
    }

    public MineBuildsLN(ConfigurationManager config) {
    }

    #region//---- USER METHODS ----//
    public async Task<IEnumerable<User>> getAllUsersAsync() {
        return await userFacade.getAllAsync();
    }

    public async Task<User> authenticateAsync(string email, string password) {
        return await userFacade.validateUserAsync(email, password);
    }

    public async Task<User> getUserByEmailAsync(string email) {
        return await userFacade.getUserByEmailAsync(email);
    }

    public async Task<User> updateUserAsync(int id, string email, string username, string password) {
        return await userFacade.updateUserAsync(id, email, username, password);
    }

    public async Task<bool> registerUserAsync(string email, string username, string password) {
        return await userFacade.registerUserAsync(email, username, password);
    }
    #endregion

    #region//---- STOCK METHODS ----//
    public async Task<IEnumerable<Block>> getAllBlocksAsync() {
        return await stockFacade.getAllBlocksAsync();
    }

    public async Task<Dictionary<string, int>> getOrderContentAsync(int id) {
        return await stockFacade.getOrderContentAsync(id);
    }
    public async Task<List<Order>> getUserOrdersAsync(int id) {
        return await stockFacade.getUserOrdersAsync(id);
    }

    public async Task<Dictionary<string, int>> getUserStockAsync(int id) {
        return await stockFacade.getUserStockAsync(id);
    }

    public async Task<int> createOrderAsync(int id, Dictionary<int, int> blocks) {
        return await stockFacade.createOrderAsync(id, blocks);
    }

    public BlockProperties getBlockProperties(int blockPropertiesID) {
        return stockFacade.getBlockProperties(blockPropertiesID);
    }

    public Dictionary<int, BlockProperties> getAllBlockProperties() {
        return stockFacade.getAllBlockProperties();
    }
    #endregion

    #region//---- CONSTRUCTION METHODS ----//
    public async Task<IEnumerable<Construction>> getAllConstructionInstancesAsync() {
        return await constructionFacade.getAllConstructionInstancesAsync();
    }

    public async Task<bool> hasStockAsync(int userID, int constructionPropertiesID) {
        var result = await this.calculateMissingBlocksAsync(userID, constructionPropertiesID);
        return (result.ToDictionary().Count()==0);
    }

    public async Task<bool> addConstructionToQueueAsync(int userID, int constructionPropertiesID) {
        if (!await this.hasStockAsync(userID, constructionPropertiesID)) {
            throw new UserHasNotEnoughBlocksException($"User has not enough blocks to build construction with id: {constructionPropertiesID}");
        }

        Dictionary<string, int> blocksNeededByName = await constructionFacade.getBlocksNeededAsync(constructionPropertiesID);
        Dictionary<int, int> blocksNeededByID = new Dictionary<int, int>();

        foreach (KeyValuePair<string, int> block in blocksNeededByName) {
            int id = stockFacade.getBlockPropertiesIDbyName(block.Key);
            blocksNeededByID[id] = block.Value;
        }

        return await constructionFacade.addConstructionToQueueAsync(blocksNeededByID, userID, constructionPropertiesID);
    }

    public async Task<bool> updateConstructionStateToBuilding(int idConstruction) {
        return await constructionFacade.updateConstructionStateAsync(idConstruction, (int) ConstructionState.BUILDING);
    }

    public async Task<Dictionary<string, int>> calculateMissingBlocksAsync(int userdID, int constructionPropertiesID) {
        Dictionary<string, int> missingBlocks = new Dictionary<string, int>();
        Dictionary<string, int> quantityByName = await constructionFacade.getBlocksNeededAsync(constructionPropertiesID);
        Dictionary<string, int> userStock = await stockFacade.getUserStockAsync(userdID);
        foreach (string blockName in quantityByName.Keys) {
            int quantityNeeded = quantityByName[blockName];
            int quantityInStock = userStock.TryGetValue(blockName, out int stock) ? stock : 0;
            if (quantityInStock < quantityNeeded) {
                missingBlocks[blockName] = quantityNeeded - quantityInStock;
            }
        }
        return missingBlocks;
    }

    public async Task<List<int>> getAwaitingConstructionsIdsAsync(int userID) {
        return await constructionFacade.getConstructionsOfStateIdsAsync(userID, (int) ConstructionState.WAITING);
    }

    public async Task<Dictionary<string, int>> getAwaitingConstructionsAsync(int userID) {
        return await constructionFacade.getConstructionsOfStateAsync(userID, (int) ConstructionState.WAITING);
    }

    public async Task<Dictionary<string, int>> getCompletedConstructionsAsync(int userID) {
        return await constructionFacade.getConstructionsOfStateAsync(userID, (int) ConstructionState.COMPLETED);
    }

    public async Task<Dictionary<string, int>> getBuildingConstructionsAsync(int userID) {
        return await constructionFacade.getConstructionsOfStateAsync(userID, (int) ConstructionState.BUILDING);
    }

    public async Task<Dictionary<string, int>> viewCompletedConstructionAsync(int userId, int constructionId) {
        return await constructionFacade.getCompletedConstructionAsync(userId, constructionId);
    }

    public async Task<bool> removeConstructionAsync(int idUser, int idConstruction) {
        return await constructionFacade.removeConstructionInWaitingAsync(idUser, idConstruction);
    }

    public async Task<Dictionary<int,string>> getCatalogAsync() {
        return await constructionFacade.getConstructionsAsync();
    }

    public ConstructionProperties getConstructionProperties(int constructionPropertiesID) {
        return constructionFacade.getConstructionProperties(constructionPropertiesID);
    }

    public BlocksToConstruction getBlocksToConstruction(int constructionPropertiesID, int blockPropertiesID) {
        Tuple<int,int> blockToConstructionID = Tuple.Create(constructionPropertiesID, blockPropertiesID);
        return constructionFacade.getBlocksToConstruction(blockToConstructionID);
    }

    public async Task<Dictionary<string, int>> getAllBlocksConstructionAsync(int constructionPropertiesID) {
        return await constructionFacade.getBlocksNeededAsync(constructionPropertiesID);
    }

    #endregion
}
