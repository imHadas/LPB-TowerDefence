using System.Collections.Generic;

namespace TowerDefenceBackend.Persistence
{
    /// <summary>
    /// Multiple visual representaion for terrain. (for expandability)
    /// </summary>
    public enum TerrainType { Mountain, Lake }

    /// <summary>
    /// Class containing generic data related structures placed on a <c>Field</c>.
    /// Cannot be directly constructed, must be derived.
    /// </summary>
    /// <remarks>Immutable</remarks>
    public class Placement
    {
        public Player? Owner { get; private set; }
        public (uint x, uint y) Coords { get; private set; }

        public PlayerType OwnerType => Owner?.Type ?? PlayerType.NEUTRAL;

        protected Placement((uint x, uint y) coords, Player? player = null)
        {
            Owner = player;
            Coords = coords;
        }
    }
    public class Terrain : Placement
    {
        public TerrainType Type { get; private set; }

        /// <summary>
        /// Property for implicit casting of <c>Type</c>
        /// </summary>
        public int NumericType => (int)Type;

        public Terrain(uint x, uint y, TerrainType type) : base((x, y))
        {
            Type = type;
        }
    }

    /// <summary>
    /// <c>Player</c>'s base structure
    /// </summary>
    public class Castle : Placement
    {
        public uint Health { get; private set; }  // changed to unsigned

        public Castle(Player owner, uint x, uint y) : base((x, y), owner)
        {
            Health = Constants.CASTLE_STARTING_HEALTH;
        }

        public void Damage(uint amount = 1)  
        {
            if(Health != 0) Health -= amount;
        }
    }

    /// <summary>
    /// Structure where the <c>Player</c>'s <c>Unit</c>s will be placed down after training
    /// </summary>
    public class Barrack : Placement
    {
        public (uint,uint) WhereToPlace { get; private set; }

        public Queue<Unit> UnitQueue { get; set; }

        public Barrack(Player owner, uint x, uint y) : base((x, y), owner)
        {
            WhereToPlace = new(x + 1, y);
            UnitQueue = new Queue<Unit>();
        }

        /// <summary>
        /// Method to modify the side of the <c>Barrack</c> where trained <c>Unit</c>s will be placed down
        /// </summary>
        /// <param name="path">Path from the <c>Barrack</c> to the enemy <c>Castle</c></param>
        public void NewPath(IList<(uint,uint)> path)
        {
            WhereToPlace = path[0];
        }

        /// <summary>
        /// Method to modify the side of the <c>Barrack</c> where trained <c>Unit</c>s will be placed down
        /// </summary>
        /// <param name="tile">Coordinates of where the <c>Unit</c>s should be placed</param>
        public void NewPath((uint,uint) tile)
        {
            WhereToPlace = tile;
        }

    }
}
