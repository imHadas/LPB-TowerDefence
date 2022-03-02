using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenceGame_LPB.Persistence
{
    public class Tower : Placement
    {
        private int speed;
        private int damage;
        private int range;
        private int level;


        public virtual void LevelUp() { }
    }
    public class BasicTower : Tower
    {
        public override void LevelUp() { }
    }
    public class SniperTower : Tower
    {
        public override void LevelUp() { }
    }
    public class BomberTower : Tower
    {
        public override void LevelUp() { }
    }

}
