using System.Collections.Generic;

namespace TowerDefenceBackend.Persistence
{
    /// <summary>
    /// Generic class for storing data about a specific unit.
    /// Cannot be instatiated directly, must be derived.
    /// </summary>
    public class Unit
    {
        public Player Owner { get; private set; }
        public uint Health { get; protected set; }
        /// <summary>
        /// Maximum stamina, the amount of tiles the unit will move in one turn
        /// </summary>
        public uint Speed { get; private set; }
        public uint Stamina { get; private set; }
        public LinkedList<(uint x, uint y)> Path { get; private set; } // order matters, also O(1) removing first element
        virtual public uint Cost { get; } 

        protected Unit(Player player, uint health, uint speed)
        {
            Owner = player;
            Health = health;
            Speed = speed;
            Stamina = speed;
            Path = new LinkedList<(uint x, uint y)>();
        }

        /// <summary>
        /// Sets stamina back to max (speed)
        /// </summary>
        public void ResetStamina() => Stamina = Speed;

        /// <summary>
        /// Lowers <c>Unit</c>'s health
        /// </summary>
        /// <param name="amount">Amount to lower by</param>
        public void Damage(uint amount = 1)
        {
            if (amount > Health)
                Health = 0;
            else
                Health -= amount;
        }

        /// <summary>
        /// Removes the first element of the path and decrements stamina
        /// </summary>
        public void Moved()
        {
            Path.RemoveFirst();
            Stamina--;
        }

        public void NewPath(LinkedList<(uint, uint)> path) => Path = new(path);  //if being a linked list directly

        public void NewPath(IList<(uint, uint)> path) => Path = new(path);  // if we get an indexed collection (e.g. array)
    }

    /// <summary>
    /// Basic unit entity. Has low health, but fast.
    /// </summary>
    public class BasicUnit : Unit
    {
        public BasicUnit(Player player) : base(
            player, 
            Constants.BASIC_UNIT_STARTING_HEALTH, 
            Constants.BASIC_UNIT_SPEED
            ) 
        { }

        public override uint Cost => Constants.BASIC_UNIT_COST;
    }

    /// <summary>
    /// Tank unit entity. Has high health, but slow.
    /// </summary>
    public class TankUnit : Unit
    {
        public TankUnit(Player player) : base(
            player,
            Constants.TANK_UNIT_STARTING_HEALTH,
            Constants.TANK_UNIT_SPEED
            )
        { }

        public override uint Cost => Constants.TANK_UNIT_COST;
    }
}
