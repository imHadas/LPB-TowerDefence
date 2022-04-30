using System;

namespace TowerDefenceBackend.Persistence
{
    public abstract class Tower : Placement
    {
        /// <summary>
        /// Amount of cooldown after firing.
        /// </summary>
        public uint Speed { get; protected set; }
        public uint Damage { get; protected set; }
        public uint Range { get; protected set; }
        public uint Level { get; protected set; }
        public uint Cooldown { get; protected set; }
        public abstract uint Cost { get; }

        public abstract void LevelUp();
        public virtual bool InRange((uint x, uint y) coords)
        {
            return Math.Sqrt(Math.Pow((int)coords.x - (int)Coords.x, 2) + Math.Pow((int)coords.y - (int)Coords.y, 2)) <= Range;
        }
        public virtual void Fire()
        {
            if (Cooldown != 0)
                Cooldown--;
        }
        public virtual void ResetCooldown()
        {
            Cooldown = Speed;
        }

        internal Tower(Player player, (uint, uint) coords) : base(coords, player) { }
    }
    public class BasicTower : Tower
    {
        public override uint Cost => Constants.BASIC_TOWER_COST;

        public override void LevelUp() 
        {
            if (Owner.Money < Constants.BASIC_TOWER_COST / 2)
                return;
            if (Level < Constants.MAX_TOWER_LEVEL)
            {
                Owner.Money -= Constants.BASIC_TOWER_COST / 2;
                switch (Level)
                {
                    case 1:
                        Range++;
                        Level++;
                        break;
                    case 2:
                        Speed--;
                        Level++;
                        break;
                }
            }
        }

        public BasicTower(Player player, (uint, uint) coords) : base (player, coords) 
        {
            Speed = 2;
            Damage = 1;
            Range = 1;
            Level = 1;
        }
    }
    public class SniperTower : Tower
    {
        public override uint Cost => Constants.SNIPER_TOWER_COST;

        public override void LevelUp() 
        {
            if (Owner.Money < Constants.SNIPER_TOWER_COST / 2)
                return;
            if (Level < 3)
            {
                Owner.Money -= Constants.SNIPER_TOWER_COST / 2;
                switch (Level)
                {
                    case 1:
                        Range++;
                        Level++;
                        break;
                    case 2:
                        Speed--;
                        Level++;
                        break;
                }
            }
            
        }

        public SniperTower(Player player, (uint, uint) coords) : base(player, coords) 
        {
            Speed = 4;
            Damage = 1;
            Range = 2;
            Level = 1;
        }
    }
    public class BomberTower : Tower
    {
        public override uint Cost => Constants.BOMBER_TOWER_COST;

        public override void LevelUp()
        {
            if (Owner.Money < Constants.BOMBER_TOWER_COST / 2)
                return;
            if (Level < 3)
            {
                Owner.Money -= Constants.BOMBER_TOWER_COST / 2;
                switch (Level)
                {
                    case 1:
                        Speed--;
                        Level++;
                        break;
                    case 2:
                        Range++;
                        Level++;
                        break;
                }
            }
            
        }

        public BomberTower(Player player, (uint, uint) coords) : base(player, coords) 
        {
            Speed = 5;
            Damage = 3;
            Range = 1;
            Level = 1;
        }
    }

}
