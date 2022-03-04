using System.Collections.Generic;

namespace TowerDefenceGame_LPB.Persistence
{
    public class Unit
    {
        public Player Owner { get; private set; }
        public uint Health { get; protected set; }
        public uint Speed { get; private set; }
        public uint Stamina { get; private set; }
        public LinkedList<(int x, int y)> Path { get; private set; } // order matters, also O(1) removing first element

        public Unit(Player player, uint health, uint speed)
        {
            Owner = player;
            Health = health;
            Speed = speed;
            Stamina = speed;
            Path = new LinkedList<(int x, int y)>();
        }

        public void ResetStamina() => Stamina = Speed;

        public void Moved() => Path.RemoveFirst();

        public void NewPath(LinkedList<(int, int)> path) => Path = new(path);  //if being a linked list directly

        public void NewPath(IList<(int, int)> path) => Path = new(path);  // if we get an indexed collection (e.g. array)
    }
    public class BasicUnit : Unit
    {
        public BasicUnit(Player player) : base(
            player, 
            Constants.BASIC_UNIT_STARTING_HEALTH, 
            Constants.BASIC_UNIT_SPEED
            ) 
        { }
    }

    public class TankUnit : Unit
    {
        public TankUnit(Player player) : base(
            player,
            Constants.TANK_UNIT_STARTING_HEALTH,
            Constants.TANK_UNIT_SPEED
            )
        { }
    }
}
