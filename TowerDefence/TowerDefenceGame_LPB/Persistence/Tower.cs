﻿using System;

namespace TowerDefenceBackend.Persistence
{
    /// <summary>
    /// Generic class for the tower structure.
    /// Cannot be instantiated directly, must be derived.
    /// </summary>
    public abstract class Tower : Placement
    {
        /// <summary>
        /// Amount of cooldown after firing.
        /// </summary>
        /// <remarks>A lower value means faster firing</remarks>
        public uint Speed { get; protected set; }
        public uint Damage { get; protected set; }
        public uint Range { get; protected set; }
        public uint Level { get; protected set; }
        public uint Cooldown { get; protected set; }
        public abstract uint Cost { get; }

        /// <summary>
        /// Method for leveling up the tower
        /// </summary>
        public abstract void LevelUp();
        
        /// <summary>
        /// Starts cooldown of the <c>Tower</c>
        /// </summary>
        public virtual void Fire()
        {
            Cooldown = Speed;
        }

        /// <summary>
        /// Decrements the <c>Tower</c>'s cooldown
        /// </summary>
        /// <param name="amount">Amount to subtract</param>
        public virtual void Cool(uint amount = 1)
        {
            if(amount > Cooldown) Cooldown = 0;
            else Cooldown -= amount;
        }

        /// <summary>
        /// Sets cooldown to 0, meaning the <c>Tower</c> can immediately fire again
        /// </summary>
        public virtual void ResetCooldown()
        {
            Cooldown = 0;
        }

        internal Tower(Player player, (uint, uint) coords) : base(coords, player) { Cooldown = 0; }
    }

    /// <summary>
    /// Basic tower structure. Shoots at directly adjacent <c>Unit</c>s, dealing low damage.
    /// </summary>
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

    /// <summary>
    /// Sniper tower structure. Has higher range than basic.
    /// </summary>
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

    /// <summary>
    /// Bomber tower structure. Deals more damage than basic.
    /// </summary>
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
