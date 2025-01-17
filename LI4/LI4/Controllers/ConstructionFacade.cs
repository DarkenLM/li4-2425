using LI4.Common.Dados;
using LI4.Common.Exceptions.ConstructionExceptions;
using LI4.Controllers.DAO;
using LI4.Dados;
using Microsoft.Data.SqlClient;

namespace LI4.Controllers;

public class ConstructionFacade {
    private ConstructionDAO constructionDAO;

    public ConstructionFacade(ConfigurationManager config) {
        this.constructionDAO = new ConstructionDAO(config.GetConnectionString("DefaultConnection"));
    }

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
    

    public async Task<Dictionary<string,int>> getBlocksNeeded(int constructionPropertyID) {
        return await constructionDAO.getBlocksNeeded(constructionPropertyID);
    }

    public async Task<Dictionary<string, int>> getConstructionsOfState(int userID, int state) {
        return await constructionDAO.getConstructionsOfState(userID, state);
    }
    #endregion

    public async Task<Dictionary<string, int>> getCompletedConstruction(int userId, int constructionId) {
        return await constructionDAO.getCompletedConstructionBlocks(userId, constructionId);
    }

}
