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

        internal Tower(Player player, (uint, uint) coords) : base(coords, player) { }
    }
    public class BasicTower : Tower
    {
        public override uint Cost => Constants.BASIC_TOWER_COST;

        public override void LevelUp() { }

        public BasicTower(Player player, (uint, uint) coords) : base (player, coords) { }
    }
    public class SniperTower : Tower
    {
        public override uint Cost => Constants.SNIPER_TOWER_COST;

        public override void LevelUp() { }

        public SniperTower(Player player, (uint, uint) coords) : base(player, coords) { }
    }
    public class BomberTower : Tower
    {
        public override uint Cost => Constants.BOMBER_TOWER_COST;

        public override void LevelUp() { }

        public BomberTower(Player player, (uint, uint) coords) : base(player, coords) { }
    }

}
