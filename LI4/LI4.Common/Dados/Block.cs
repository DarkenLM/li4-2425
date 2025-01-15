namespace LI4.Common.Dados {
    public enum BlockRarity {
        COMMON,
        RARE,
        EPIC
    }

    public class Block {
        private int id { get; set; }
        private string name { get; set; }
        private BlockRarity rarity { get; set; }
        private int timeToAcquire { get; set;  }
        private int orderID { get; set;  }

        public Block(int id, string name, BlockRarity rarity, int timeToAcquire, int orderID) {
            this.id = id;
            this.name = name;
            this.rarity = rarity;
            this.timeToAcquire = timeToAcquire;
            this.orderID = orderID;
        }
    }
}
