namespace TowerDefenceGame_LPB.Persistence
{
    public enum TerrainType { Mountain, Lake }

    public class Placement
    {
        public Player? Owner { get; private set; }
        public (int x, int y) Coords { get; private set; }

        internal Placement((int x, int y) coords, Player? player = null)
        {
            Owner = player;
            Coords = coords;
        }
    }
    public class Terrain : Placement
    {
        public TerrainType Type { get; private set; }

        public int NumericType => (int)Type; // maybe unnecessary

        public Terrain(Player owner, int x, int y, TerrainType type) : base((x, y), owner)
        {
            Type = type;
        }
    }
    public class Castle : Placement
    {
        public uint Health { get; private set; }  // changed to unsigned

        public Castle(Player owner, int x, int y) : base((x, y), owner)
        {
            Health = Constants.CASTLE_STARTING_HEALTH;
        }

        public void Damage(uint amount = 1)  
        {
            Health -= amount;
        }
    }

    public class Barrack : Placement
    {
        public Barrack(Player owner, int x, int y) : base((x, y), owner)
        {

        }
    }
}
