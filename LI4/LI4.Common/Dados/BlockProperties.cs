using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LI4.Common.Dados;

public enum BlockRarity {
    COMMON,
    RARE,
    EPIC
}

public class BlockProperties {
    public int id { get; }
    public string name { get; }
    public BlockRarity rarity { get; }
    public int timeToAcquire { get; }

    public BlockProperties() {}

    public BlockProperties(int id, string name, BlockRarity rarity, int timeToAcquire) {
        this.id = id;
        this.name = name;
        this.rarity = rarity;
        this.timeToAcquire = timeToAcquire;
    }

    public string toString() {
        return $"BlockProperties [ID: {id}, Name: {name}, Rarity: {rarity}, Time to Acquire: {timeToAcquire} seconds]";
    }
}
