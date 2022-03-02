using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenceGame_LPB.Persistence
{
    public enum PlayerType
    {
        NEUTRAL,
        RED,
        BLUE
    }
    public class Player
    {
        private PlayerType type;
        private int Money;
        private Tower[] towers;
        private Unit[] units;
        private Barrack[] barracks;
    }
}
