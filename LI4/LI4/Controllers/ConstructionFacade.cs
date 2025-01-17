using LI4.Common.Dados;
using LI4.Common.Exceptions.ConstructionExceptions;
using LI4.Controllers.DAO;
using LI4.Dados;
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;

namespace LI4.Controllers;

public class ConstructionFacade {
    private ConstructionDAO constructionDAO;
    private Dictionary<int, ConstructionProperties> constructionProperties;
    private Dictionary<Tuple<int, int>, BlocksToConstruction> blocksToConstruction;

    public ConstructionFacade(ConfigurationManager config) {
        this.constructionDAO = new ConstructionDAO(config.GetConnectionString("DefaultConnection"));
    }

    public async Task initializeConstructionPropertiesAsync() {
        try {
            var constructionProperties = await constructionDAO.getAllConstructionPropertiesAsync();
            this.constructionProperties = constructionProperties.ToDictionary(c => c.Value.id, c => c.Value);
        } catch (Exception ex) {
            throw new Exception("Failed to initialize Construction Properties", ex);
        }
    }

    public async Task initializeBlocksToConstructions() {
        try {
            var blocksToConstruction = await constructionDAO.getAllBlocksToConstructionAsync();
            this.blocksToConstruction = blocksToConstruction.ToDictionary(r => r.Key, r => r.Value);
        } catch (Exception ex) {
            throw new Exception("Failed to initialize Blocks To Construction", ex);
        }
    }

    #region//---- INTERNAL STRUCTS ----//
    public ConstructionProperties getConstructionProperties(int constructionPropertiesID) {
        return this.constructionProperties[constructionPropertiesID];
    }

    public BlocksToConstruction getBlocksToConstruction(Tuple<int, int> blockToConstructionID) {
        return this.blocksToConstruction[blockToConstructionID];
    }
    #endregion

    #region//---- XXX METHODS ----//
    public async Task<int?> addConstructionInstanceAsync(ConstructionState state, int constructionPropertiesID, int userID) {
        return await constructionDAO.addConstructionInstanceAsync(state, constructionPropertiesID, userID);
    }

    public async Task<Construction?> getByIDAsync(int constructionID) {
        return await constructionDAO.getByIDAsync(constructionID);
    }

    public async Task<IEnumerable<Construction>> getAllConstructionInstancesAsync() {
        return await constructionDAO.getAllConstructionInstancesAsync();
    }

    public async Task<bool> updateConstructionInstanceAsync(int constructionID, ConstructionState state, int constructionPropertiesID, int userID) {
        return await constructionDAO.updateConstructionInstanceAsync(constructionID, state, constructionPropertiesID, userID);
    }

    public async Task<bool> deleteConstructionInstanceAsync(int id) {
        return await constructionDAO.deleteConstructionInstanceAsync(id);
    }

    public async Task<Dictionary<string, int>> getBlocksNeeded(int constructionPropertyID) {
        return await constructionDAO.getBlocksNeeded(constructionPropertyID);
    }

    public async Task<Dictionary<string, int>> getConstructionsOfState(int userID, int state) {
        return await constructionDAO.getConstructionsOfState(userID, state);
    }

    public async Task<Dictionary<string, int>> getCompletedConstruction(int userId, int constructionId) {
        return await constructionDAO.getCompletedConstructionBlocks(userId, constructionId);
    }
    public async Task<bool> removeConstructionInWaiting(int idUser, int idConstruction) {
        return await constructionDAO.removeConstructionInWaitingAsync(idUser, idConstruction);
    }

    public async Task<Dictionary<int, string>> getConstructions() {
        return await constructionDAO.getConstructionsAsync();
    }

    #endregion
}
