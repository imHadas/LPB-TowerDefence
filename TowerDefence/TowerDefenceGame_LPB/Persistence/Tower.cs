namespace TowerDefenceGame_LPB.Persistence
{
    public abstract class Tower : Placement
    {
        public int Speed { get; protected set; }
        public int Damage { get; protected set; }
        public int Range { get; protected set; }
        public int Level { get; protected set; }

        public virtual void LevelUp() { }

        internal Tower(Player player, (int, int) coords) : base(coords, player) { }
    }
    public class BasicTower : Tower
    {
        public override void LevelUp() { }

        public BasicTower(Player player, (int, int) coords) : base (player, coords) { }
    }
    public class SniperTower : Tower
    {
        public override void LevelUp() { }

        public SniperTower(Player player, (int, int) coords) : base(player, coords) { }
    }
    public class BomberTower : Tower
    {
        public override void LevelUp() { }

        public BomberTower(Player player, (int, int) coords) : base(player, coords) { }
    }

}
