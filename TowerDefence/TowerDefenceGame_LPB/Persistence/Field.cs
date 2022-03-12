using System.Collections.Generic;

namespace TowerDefenceGame_LPB.Persistence
{
    public class Field
    {
        public (int x, int y) Coords { get; private set; }
        public Placement? Placement;
        public ISet<Unit> Units;

        public Player? Owner => Placement is null || Placement is Terrain ? null : Placement.Owner;
        public PlayerType OwnerType => Owner is null ? PlayerType.NEUTRAL : Owner.Type;

        public Field(int x, int y)
        {
            Coords = new(x,y);
            Units = new HashSet<Unit>();
            Placement = null;
        }

        public Field((int x, int y) coords) : this(coords.x, coords.y) { }
    }
}
