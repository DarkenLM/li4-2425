using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LI4.Common.Dados;

public enum ConstructionState {
    COMPLETED,
    BUILDING,
    WAITING
}

public enum ConstructionDificulty {
    LOW,
    MEDIUM,
    HIGH
}

public class Construction {
    public int id { get; set; }
    public string name { get; set; }
    public ConstructionState state { get; set; }
    public ConstructionDificulty dificulty { get; set; }
    public int nStages { get; set; }

    public Construction() { }

    public Construction(int id, string name, ConstructionState state, ConstructionDificulty dificulty, int nStages) {
        this.id = id;
        this.name = name;
        this.state = state;
        this.dificulty = dificulty;
        this.nStages = nStages;
    }
}
