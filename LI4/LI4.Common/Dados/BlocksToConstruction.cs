using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LI4.Common.Dados;
public class BlocksToConstruction {
    public int constructionPropertiesID { get; }
    public int blockPropertiesID { get; }
    public int blockQuantity { get; }

    public BlocksToConstruction() { }

    public BlocksToConstruction(int constructionPropertiesID, int blockPropertiesID, int blockQuantity) {
        this.constructionPropertiesID = constructionPropertiesID;
        this.blockPropertiesID = blockPropertiesID;
        this.blockQuantity = blockQuantity;
    }

    public string toString() {
        return $"BlocksToConstruction [ConstructionPropertiesID: {constructionPropertiesID}, BlockPropertiesID: {blockPropertiesID}, BlockQuantity: {blockQuantity}]";
    }
}
