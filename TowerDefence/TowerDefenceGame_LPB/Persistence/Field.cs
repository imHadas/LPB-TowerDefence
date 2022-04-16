using System.Collections.Generic;

namespace TowerDefenceBackend.Persistence
{
    public class Field
    {
        public (uint x, uint y) Coords { get; private set; }
        public Placement? Placement { get; set; }
        public ISet<Unit> Units { get; set; }

        public Player? Owner => Placement is null || Placement is Terrain ? null : Placement.Owner;
        public PlayerType OwnerType => Owner is null ? PlayerType.NEUTRAL : Owner.Type;

        public Field(uint x, uint y)
        {
            Coords = new(x,y);
            Units = new HashSet<Unit>();
            Placement = null;
        }

        public Field((uint x, uint y) coords) : this(coords.x, coords.y) { }
    }
}
