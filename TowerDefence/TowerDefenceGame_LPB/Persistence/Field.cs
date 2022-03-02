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
        private Unit[] units;
    }
}
