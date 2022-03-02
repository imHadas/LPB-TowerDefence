using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenceGame_LPB.ViewModel
{
    public class TestField
    {
        private int blueBasic;
        private int blueTank;
        private int redBasic;
        private int redTank;

        public int RedTank
        {
            get { return redTank; }
            set { redTank = value; }
        }

        public int RedBasic
        {
            get { return redBasic; }
            set { redBasic = value; }
        }

        public int BlueTank
        {
            get { return blueTank; }
            set { blueTank = value; }
        }

        public int BlueBasic
        {
            get { return blueBasic; }
            set { blueBasic = value; }
        }
    }
}
