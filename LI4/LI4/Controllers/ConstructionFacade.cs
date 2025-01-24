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
    private Dictionary<int, Dictionary<int, LinhaDeMontagem>> assemblyLines;

    public ConstructionFacade(ConfigurationManager config) {
        this.constructionDAO = new ConstructionDAO(config.GetConnectionString("DefaultConnection"));
    }

    public async Task initializeConstructionPropertiesAsync() {
        try {
            var constructionProperties = await constructionDAO.getAllConstructionPropertiesAsync();
            var constructionStages = await constructionDAO.getAllConstructionStagesPropertiesAsync();
            this.constructionProperties = constructionProperties.ToDictionary(c => c.Value.id, c => c.Value);
            Dictionary<int, List<(int, int)>> stagesPerConstructionProperties = new();

            foreach (var line in constructionStages) {
                if (!stagesPerConstructionProperties.ContainsKey(line.Item1)) {
                    stagesPerConstructionProperties.Add(line.Item1, new());
                }

                stagesPerConstructionProperties[line.Item1].Add((line.Item2 - 1, line.Item3));
            }

            foreach (var construction in stagesPerConstructionProperties) {
                int nStages = this.constructionProperties[construction.Key].nStages;
                this.constructionProperties[construction.Key].stages = new Estagio[nStages];
                for (int i = 0; i < nStages; i++) {
                    this.constructionProperties[construction.Key].stages[i] = new Estagio();
                }

                foreach (var stage in construction.Value) {
                    this.constructionProperties[construction.Key].stages[stage.Item1].setupTimer(stage.Item2);
                    // Console.WriteLine("Construcao ID" + construction.Key + ". Estagio: " + stage.Item1 + ". Tempo: " + stage.Item2);
                }
            }

        } catch (Exception ex) {
            throw new Exception("Failed to initialize Construction Properties", ex);
        }
    }

    public async Task initializeBlocksToConstructionsAsync() {
        try {
            var blocksToConstruction = await constructionDAO.getAllBlocksToConstructionAsync();
            this.blocksToConstruction = new();
            foreach (var line in blocksToConstruction) {
                if (!this.blocksToConstruction.ContainsKey(new Tuple<int, int>(line.Item1, line.Item2))) {
                    BlocksToConstruction blocks = new(line.Item1, line.Item2);
                    blocks.stages.Add(line.Item3, line.Item4);
                    this.blocksToConstruction.Add(Tuple.Create(line.Item1, line.Item2), blocks);
                } else {
                    if (this.blocksToConstruction[Tuple.Create(line.Item1, line.Item2)].stages != null)
                        this.blocksToConstruction[Tuple.Create(line.Item1, line.Item2)].stages.Add(line.Item3, line.Item4);
                }
            }
        } catch (Exception ex) {
            throw new Exception("Failed to initialize Blocks To Construction", ex);
        }
    }

    public async Task initializeAssemblyLinesAsync() {
        try {
            this.assemblyLines = new();

            var constructionsBuilding = await constructionDAO.getUserIdAndConstructionsIdOfStateAsync((int) ConstructionState.BUILDING);
            foreach(var consWaiting in constructionsBuilding) {
                int idConstruction = consWaiting.Item1;
                int idConstructionProperties = consWaiting.Item2;
                int idUser = consWaiting.Item3;
                // Console.WriteLine("Adicionado com prioridade: " + idConstruction);

                addConstructionWaitingList(idConstructionProperties, idConstruction, idUser);
            }

            var constructionsWaiting = await constructionDAO.getUserIdAndConstructionsIdOfStateAsync((int) ConstructionState.WAITING);
            foreach(var consWaiting in constructionsWaiting) {
                int idConstruction = consWaiting.Item1;
                int idConstructionProperties = consWaiting.Item2;
                int idUser = consWaiting.Item3;
                // Console.WriteLine("Adicionado sem prioridade: " + idConstruction);

                addConstructionWaitingList(idConstructionProperties, idConstruction, idUser);
            }
        } catch (Exception ex) {
            throw new Exception("Failed to initialize assembly lines", ex);
        }
    }

    #region//---- ASSEMBLY LINE METHODS ----//
    private void addConstructionWaitingList(int idConstructionProperties, int idConstruction, int idUser) {
        if (this.assemblyLines.ContainsKey(idUser)) {
            if (!this.assemblyLines[idUser].ContainsKey(idConstructionProperties)) {
                ConstructionProperties cp = getConstructionProperties(idConstructionProperties);
                this.assemblyLines[idUser].Add(idConstructionProperties, new LinhaDeMontagem(cp.nStages, cp.stages));
            } 
        } else {
            Dictionary<int, LinhaDeMontagem> userLines = new();
            ConstructionProperties cp = getConstructionProperties(idConstructionProperties);
            userLines.Add(idConstructionProperties, new LinhaDeMontagem(cp.nStages, cp.stages));
            this.assemblyLines.Add(idUser, userLines);
        }

        // Comportamento
        addConstructionWaitingAsync(idConstruction, this.assemblyLines[idUser][idConstructionProperties]);
    }

    private async void addConstructionWaitingAsync(int idConstruction, LinhaDeMontagem linha) {
        if (linha.stages[0].idConstruction == null) {
            linha.stages[0].idConstruction = idConstruction;
            linha.stages[0].tw = new TimerWrapper(linha.stages[0].tempo * 1000, () => this.changeConstructionStageAsync(0, linha), false);
            linha.stages[0].tw.start();
            await updateConstructionStateAsync(idConstruction, (int) ConstructionState.BUILDING);
        } else {
            linha.waitingConstructions.Add(idConstruction);
        }
    }

    private async void changeConstructionStageAsync(int index, LinhaDeMontagem linha) {
        if (index < 0 || index >= linha.nStages) return;
        linha.stages[index].tw = null!;

        // Último estágio
        if (index == linha.nStages - 1) {
            await updateConstructionStateAsync((int) linha.stages[index].idConstruction!, (int) ConstructionState.COMPLETED);
            if (linha.stages[index - 1].tw == null && linha.stages[index - 1].idConstruction != null) {
                linha.stages[index].idConstruction = linha.stages[index - 1].idConstruction;
                linha.stages[index].tw = new TimerWrapper(linha.stages[index].tempo * 1000, () => this.changeConstructionStageAsync(index, linha), false);
                linha.stages[index].tw.start();
                linha.stages[index - 1].idConstruction = null;
                // Console.WriteLine("Timer iniciado no último estágio, após a anterior já ter validado se podia avançar.");
            } else {
                linha.stages[index].idConstruction = null;
            }
        } else {
            if (linha.stages[index + 1].idConstruction == null) {
                linha.stages[index + 1].idConstruction = linha.stages[index].idConstruction;
                linha.stages[index + 1].tw = new TimerWrapper(linha.stages[index + 1].tempo * 1000, () => this.changeConstructionStageAsync(index + 1, linha), false);
                linha.stages[index + 1].tw.start();
                linha.stages[index].idConstruction = null;
                // Console.WriteLine($"Timer iniciado no estágio {index + 1}, após mover de posição. Construção: {linha.stages[index + 1].idConstruction}");

                if (index == 0 && linha.waitingConstructions.Count > 0) {
                    int tmpId = linha.waitingConstructions[0];
                    linha.waitingConstructions.RemoveAt(0);
                    linha.stages[index].idConstruction = tmpId;
                    linha.stages[index].tw = new TimerWrapper(linha.stages[index].tempo * 1000, () => this.changeConstructionStageAsync(index, linha), false);
                    linha.stages[index].tw.start();
                    await updateConstructionStateAsync(tmpId, (int) ConstructionState.BUILDING);
                    // Console.WriteLine($"Timer iniciado no primeiro estágio, após retirar construção da fila de espera. Construção: {linha.stages[index].idConstruction}");
                }
                
                if (index != 0 && linha.stages[index - 1].tw == null && linha.stages[index - 1].idConstruction != null) {
                    linha.stages[index].idConstruction = linha.stages[index - 1].idConstruction;
                    linha.stages[index].tw = new TimerWrapper(linha.stages[index].tempo * 1000, () => this.changeConstructionStageAsync(index, linha), false);
                    linha.stages[index].tw.start();
                    linha.stages[index - 1].idConstruction = null;
                    // Console.WriteLine($"Timer iniciado num estágio do meio, após o anterior já ter validado se podia avançar. Construção: {linha.stages[index].idConstruction}");
                }
            }
        } 
    }


    #endregion

    #region//---- INTERNAL STRUCTS ----//
    public ConstructionProperties getConstructionProperties(int constructionPropertiesID) {
        return this.constructionProperties[constructionPropertiesID];
    }

    public BlocksToConstruction getBlocksToConstruction(Tuple<int, int> blockToConstructionID) {
        return this.blocksToConstruction[blockToConstructionID];
    }

    public Estagio[] getConstructionStages(int idConstructionProperties) {
        return this.constructionProperties[idConstructionProperties].stages;
    }

    #endregion

    #region//---- XXX METHODS ----//
    public async Task<bool> addConstructionToQueueAsync(Dictionary<int, int> blocksNeeded, int userID, int constructionPropertiesID) {
        int idConstruction = await constructionDAO.addConstructionToQueueAsync(blocksNeeded, userID, constructionPropertiesID);
        addConstructionWaitingList(constructionPropertiesID, idConstruction, userID);
        return idConstruction >= 0;
    }

    public async Task<int?> addConstructionInstanceAsync(ConstructionState state, int constructionPropertiesID, int userID) {
        return await constructionDAO.addConstructionInstanceAsync(state, constructionPropertiesID, userID);
    }

    public async Task<Construction?> getByIDAsync(int constructionID) {
        return await constructionDAO.getByIDAsync(constructionID);
    }

    public async Task<IEnumerable<Construction>> getAllConstructionInstancesAsync() {
        return await constructionDAO.getAllConstructionInstancesAsync();
    }

    public int getConstructionStage(int idConstruction, int idUser, int idConstructionProperties) {
        if (this.assemblyLines.ContainsKey(idUser) && this.assemblyLines[idUser].ContainsKey(idConstructionProperties)) {
            for (int i = 0 ; i < this.assemblyLines[idUser][idConstructionProperties].nStages ; i++) {
                if (this.assemblyLines[idUser][idConstructionProperties].stages[i].idConstruction == idConstruction)
                    return i+1;
            }
        }
        
        throw new ConstructionNotFoundException("Couldn't find current stage of the construction.");
    }

    public async Task<bool> updateConstructionInstanceAsync(int constructionID, ConstructionState state, int constructionPropertiesID, int userID) {
        return await constructionDAO.updateConstructionInstanceAsync(constructionID, state, constructionPropertiesID, userID);
    }

    public async Task<bool> deleteConstructionInstanceAsync(int id) {
        return await constructionDAO.deleteConstructionInstanceAsync(id);
    }

    public async Task<Dictionary<string, int>> getBlocksNeededAsync(int constructionPropertiesID) {
        return await constructionDAO.getBlocksNeededAsync(constructionPropertiesID);
    }

    public Dictionary<int, int> getBlocksAtStageConstruction(int constructionPropertiesID, int stage) {
        Dictionary<int, int> res = new();
        foreach(KeyValuePair<Tuple<int, int>, BlocksToConstruction> entry in this.blocksToConstruction) {
            if (entry.Key.Item1 == constructionPropertiesID) {
                int quantity = 0;
                foreach(KeyValuePair<int, int> constructionStage in this.blocksToConstruction[entry.Key].stages) {
                    if (constructionStage.Key <= stage) quantity += constructionStage.Value;
                }
                res.Add(entry.Key.Item2, quantity);
            }
        }

        return res;
    }

    public async Task<List<int>> getConstructionsOfStateIdsAsync(int userID, int state) {
        return await constructionDAO.getConstructionsOfStateIdsAsync(userID, state);
    }

    public async Task<Dictionary<int, int>> getConstructionsOfStateAsync(int userID, int state) {
        return await constructionDAO.getConstructionsOfStateAsync(userID, state);
    }

    public async Task<Dictionary<string, int>> getCompletedConstructionAsync(int userId, int constructionId) {
        return await constructionDAO.getCompletedConstructionBlocksAsync(userId, constructionId);
    }

    public async Task<bool> updateConstructionStateAsync(int idConstruction, int state) {
        return await constructionDAO.updateConstructionStateAsync(idConstruction, state);
    }

    public async Task<bool> removeConstructionInWaitingAsync(int idUser, int idConstructionProperties) {
        return await constructionDAO.removeConstructionInWaitingAsync(idUser, idConstructionProperties);
    }

    public List<int> getConstructions() {
        return constructionProperties.Select(kvp => kvp.Value.id).ToList();
    }

    public async Task<List<int>> getBuildingIdsConstructionsAsync(int idUser, int idConstructionProperties) {
        return await constructionDAO.getBuildingIdsConstructionsAsync(idUser, idConstructionProperties);
    }

    public int getEstimatedTime(int idUser, int idConstructionProperties, int stage) {
        if (!assemblyLines.TryGetValue(idUser, out var temp)) {
            return 0;
        }
        if (!temp.TryGetValue(idConstructionProperties, out var line)) {
            return 0;
        }

        if (stage < 0 || stage >= line.stages.Length) {
            return 0;
        }
        //Dictionary<int, LinhaDeMontagem> temp = assemblyLines[idUser];
        //LinhaDeMontagem line = temp[idConstructionProperties];
        int time = 0;
        for (int index = stage; index < line.stages.Length; index++) {
            time += line.stages[index].tempo;
        }

        return time;
    }

    public async Task<int> getConstructionPropertyIdAsync(int idConstruction) {
        return await constructionDAO.getConstructionPropertyIdAsync(idConstruction);
    }

    #endregion
}
