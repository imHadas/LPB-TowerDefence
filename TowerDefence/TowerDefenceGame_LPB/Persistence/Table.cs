using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenceGame_LPB.Persistence
{
    public class Table
    {
        private Field[][] table;
        private int round;

        public (int x, int y) Size { get; set; }

    }
}
