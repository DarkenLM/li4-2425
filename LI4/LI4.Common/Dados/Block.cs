namespace LI4.Common.Dados {

    public class Block {
        public int id { get; set; }
        public int quantity { get; set; }
        public string name { get; set; }
        public BlockRarity rarity { get; set; }
        public int timeToAcquire { get; set;  }

        public Block() { }

        public Block(int id, int quantity, string name, BlockRarity rarity, int timeToAcquire) {
            this.id = id;
            this.quantity = quantity;
            this.name = name;
            this.rarity = rarity;
            this.timeToAcquire = timeToAcquire;
        }

        public override string ToString() => "Block{"
            + "id=" + this.id + ", "
            + "quantity=" + this.quantity + ", "
            + "name='" + this.name + "', "
            + "rarity=" + this.rarity + ", "
            + "timeToAcquire=" + this.timeToAcquire
            + "}";
    }
}
