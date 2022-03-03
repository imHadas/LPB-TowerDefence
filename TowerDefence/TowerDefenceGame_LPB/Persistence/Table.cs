using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenceGame_LPB.Persistence
{
    public class Table
    {
        private Field[,] fields;
        private int round;

        public (int x, int y) Size { get; set; }
        public Field[,] Fields { get { return fields; } set { fields = value; } }

        public Table((int x, int y) size) 
        {
            Size = size;
            fields = new Field[Size.x,Size.y];
        }

    }
}
