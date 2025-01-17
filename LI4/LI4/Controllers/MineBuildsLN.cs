﻿using LI4.Common.Dados;
using LI4.Controllers.DAO;
using LI4.Dados;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using System.Threading.Tasks.Dataflow;

namespace LI4.Controllers;

public class MineBuildsLN : Common.IMineBuildsLN {
    private UserFacade userFacade;
    private StockFacade stockFacade;
    private ConstructionFacade constructionFacade;

    public MineBuildsLN(ConfigurationManager config) {
        this.userFacade = new UserFacade(config);
        this.stockFacade = new StockFacade(config);
        this.constructionFacade = new ConstructionFacade(config);
    }

    #region//---- USER METHODS ----//
    public async Task<IEnumerable<User>> getAllUsersAsync() {
        return await userFacade.getAllAsync();
    }

    public async Task<bool> authenticate(string email, string password) {
        return await userFacade.validateUser(email, password);
    }

    public async Task<User> getUserByEmail(string email) {
        return await userFacade.getUserByEmail(email);
    }

    public async Task<bool> updateUtilizador(int id, string email, string username, string password) {
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
    #endregion

    #region//---- CONSTRUCTION METHODS ----//
    public async Task<IEnumerable<Construction>> getAllConstructionInstancesAsync() {
        return await constructionFacade.getAllConstructionInstancesAsync();
    }

    public async Task<bool> hasStock(int userID, int constructionPropertyID) {
        var result = await this.calculateMissingBlocks(userID, constructionPropertyID);
        return (result.ToDictionary().Count()==0);
    }

    public async Task<bool> addConstructionToQueue(int userID, int constructionPropertyID) {
        if (!await this.hasStock(userID, constructionPropertyID)) {
            throw new UserHasNotEnoughBlocksException($"User has not enough blocks to build construction with id: {constructionPropertyID}");
        }
        int? instanceID = await constructionFacade.addConstructionInstanceAsync(ConstructionState.WAITING, constructionPropertyID, userID);
        //It adds that instance to the queue
        Console.WriteLine("Here we should had the build to the queue!!");
        return true;
    }

    public async Task<Dictionary<string, int>> calculateMissingBlocks(int userdID, int constructionPropertyID) {
        Dictionary<string, int> missingBlocks = new Dictionary<string, int>();
        Dictionary<string, int> quantityByName = await constructionFacade.getBlocksNeeded(constructionPropertyID);
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

    #endregion
}
