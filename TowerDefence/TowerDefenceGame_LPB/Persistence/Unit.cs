using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenceGame_LPB.Persistence
{
    public class Unit
    {
        private Player owner;
        private int health;
        private int speed;
        private int stamina;
        private (int x, int y)[] path;

        public void resetStamina()
        {
            stamina = speed;
        }
    }
    public class BasicUnit : Unit
    {

    }

    public class TankUnit : Unit
    {

    }
}
