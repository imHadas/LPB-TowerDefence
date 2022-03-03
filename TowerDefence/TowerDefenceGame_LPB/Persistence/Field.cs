using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenceGame_LPB.Persistence
{
    public class Field
    {
        private (int x, int y) coords;
        private Placement placement;
        private List<Unit> units;

        public (int x, int y) Coords { get { return coords; } set { coords = value; } }
        public Placement Placement { get { return placement; } set { placement = value; } } 
        public List<Unit> Units { get { return units; } set { units = value; } }

        public Field((int x, int y) _coords, Placement _placement)
        {
            coords = _coords;
            placement = _placement;
            units = new List<Unit>();
        }
    }
}
