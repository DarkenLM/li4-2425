namespace LI4.Common.Dados {
    public enum BlockRarity {
        COMMON,
        RARE,
        EPIC
    }

    public class Block {
        public int id { get; set; }
        public string name { get; set; }
        public BlockRarity rarity { get; set; }
        public int timeToAcquire { get; set;  }

        public Block() { }

        public Block(int id, string name, BlockRarity rarity, int timeToAcquire) {
            this.id = id;
            this.name = name;
            this.rarity = rarity;
            this.timeToAcquire = timeToAcquire;
        }
    }
}
