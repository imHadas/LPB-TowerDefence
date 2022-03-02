using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenceGame_LPB.Persistence
{
    public class Placement
    {
        private Player owner;
        private (int x, int y) coords;
    }
    public class Terrain : Placement
    {
        private int type;
    }
    public class Castle : Placement
    {
        private int health;

        public void Damage(int amount)
        {
            health -= amount;
        }
    }

    public class Barrack : Placement
    {

    }
}
