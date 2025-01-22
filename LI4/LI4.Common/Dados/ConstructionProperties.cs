﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LI4.Common.Dados;

public enum ConstructionDificulty {
    LOW,
    MEDIUM,
    HIGH
}

public class ConstructionProperties {
    public int id {  get; }
    public string name { get; }
    public ConstructionDificulty dificulty { get; }
    public int nStages { get; }
    public Estagio[] stages { get; set; }

    public ConstructionProperties() { }

    public ConstructionProperties(int id, string name, ConstructionDificulty dificulty, int nStages) {
        this.id = id;
        this.name = name;
        this.dificulty = dificulty;
        this.nStages = nStages;
    }

    public string toString() {
        return $"ConstructionProperties [ID: {id}, Name: {name}, dificulty: {dificulty}, Number of stages: {nStages} stages]";
    }
}
