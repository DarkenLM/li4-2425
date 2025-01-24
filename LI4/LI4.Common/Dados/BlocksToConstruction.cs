using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LI4.Common.Dados;
public class BlocksToConstruction {
    public int constructionPropertiesID { get; }
    public int blockPropertiesID { get; }
    public Dictionary<int, int> stages { get; }

    public BlocksToConstruction() { }

    public BlocksToConstruction(int constructionPropertiesID, int blockPropertiesID) {
        this.constructionPropertiesID = constructionPropertiesID;
        this.blockPropertiesID = blockPropertiesID;
        this.stages = new();
    }

    public string toString() {
        return $"BlocksToConstruction [ConstructionPropertiesID: {constructionPropertiesID}, BlockPropertiesID: {blockPropertiesID}, Stages: {stages}]";
    }
}
