using System.Collections.Generic;

namespace TowerDefenceGame_LPB.Persistence
{
    public enum TerrainType { Mountain, Lake }

    public class Placement
    {
        public Player? Owner { get; private set; }
        public (uint x, uint y) Coords { get; private set; }

        protected Placement((uint x, uint y) coords, Player? player = null)
        {
            Owner = player;
            Coords = coords;
        }
    }
    public class Terrain : Placement
    {
        public TerrainType Type { get; private set; }

        public int NumericType => (int)Type; // maybe unnecessary

        public Terrain(Player owner, uint x, uint y, TerrainType type) : base((x, y), owner)
        {
            Type = type;
        }
    }
    public class Castle : Placement
    {
        public uint Health { get; private set; }  // changed to unsigned

        public Castle(Player owner, uint x, uint y) : base((x, y), owner)
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
        public (uint,uint) WhereToPlace { get; private set; }

        public Barrack(Player owner, uint x, uint y) : base((x, y), owner)
        {
            WhereToPlace = new(x + 1, y);
        }

        public void NewPath(IList<(uint,uint)> path)
        {
            WhereToPlace = path[0];
        }
        
        public void NewPath((uint,uint) tile)
        {
            WhereToPlace = tile;
        }
    }
}
