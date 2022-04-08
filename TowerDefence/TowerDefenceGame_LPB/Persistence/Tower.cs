using System;

namespace TowerDefenceGame_LPB.Persistence
{
    public abstract class Tower : Placement
    {
        public uint Speed { get; protected set; }
        public uint Damage { get; protected set; }
        public uint Range { get; protected set; }
        public uint Level { get; protected set; }
        public abstract uint Cost { get; }

        public abstract void LevelUp();
        public abstract bool InRange((uint x, uint y) coords);
        public abstract void Fire();
        public abstract void ResetSpeed();

        internal Tower(Player player, (uint, uint) coords) : base(coords, player) { }
    }
    public class BasicTower : Tower
    {
        public override uint Cost => Constants.BASIC_TOWER_COST;

        public override void LevelUp() 
        {
            if (Owner.Money < Constants.BASIC_TOWER_COST / 2)
                return;
            if (Level < 3)
                Owner.Money -= Constants.BASIC_TOWER_COST / 2;
            switch (Level)
            {
                case 1:
                    Damage++;
                    Level++;
                    break;
                case 2:
                    Speed++;
                    Level++;
                    break;
                case 3:
                    Range++;
                    Level++;
                    break;
                default:
                    break;
            }
        }
        public override bool InRange((uint x, uint y) coords) 
        {
            if (Math.Sqrt(Math.Pow((int)coords.x - (int)Coords.x,2) + Math.Pow((int)coords.y - (int)Coords.y,2)) <= Range)
            {
                return true;
            }
            return false;
        }

        public override void Fire()
        {
            if (Speed != 0)
                Speed--;
        }

        public override void ResetSpeed()
        {
            Speed = 3;
        }

        public BasicTower(Player player, (uint, uint) coords) : base (player, coords) 
        {
            Speed = 3;
            Damage = 1;
            Range = 2;
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
                Owner.Money -= Constants.SNIPER_TOWER_COST / 2;
            switch (Level)
            {
                case 1:
                    Damage++;
                    Level++;
                    break;
                case 2:
                    Speed++;
                    Level++;
                    break;
                case 3:
                    Range++;
                    Level++;
                    break;
                default:
                    break;
            }
        }
        public override bool InRange((uint x, uint y) coords)
        {
            if (Math.Sqrt(Math.Abs(coords.x - Coords.x) ^ 2 + Math.Abs(coords.y - Coords.y) ^ 2) < Range)
            {
                return true;
            }
            return false;
        }

        public override void Fire()
        {
            if (Speed != 0)
                Speed--;
        }

        public override void ResetSpeed()
        {
            Speed = 2;
        }

        public SniperTower(Player player, (uint, uint) coords) : base(player, coords) 
        {
            Speed = 2;
            Damage = 2;
            Range = 4;
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
                Owner.Money -= Constants.BOMBER_TOWER_COST / 2;
            switch (Level)
            {
                case 1:
                    Damage++;
                    Level++;
                    break;
                case 2:
                    Speed++;
                    Level++;
                    break;
                case 3:
                    Range++;
                    Level++;
                    break;
                default:
                    break;
            }
        }
        public override bool InRange((uint x, uint y) coords)
        {
            if (Math.Sqrt(Math.Abs(coords.x - Coords.x) ^ 2 + Math.Abs(coords.y - Coords.y) ^ 2) < Range)
            {
                return true;
            }
            return false;
        }

        public override void Fire()
        {
            if(Speed!=0)
                Speed--;
        }

        public override void ResetSpeed()
        {
            Speed = 1;
        }

        public BomberTower(Player player, (uint, uint) coords) : base(player, coords) 
        {
            Speed = 1;
            Damage = 3;
            Range = 2;
            Level = 1;
        }
    }

}
