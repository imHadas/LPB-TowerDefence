using System;
using System.Collections.Generic;

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
        public PlayerType Type { get; private set; }
        public uint Money { get; set; }  // changed to unsigned

        public Castle? Castle { get; set; }

        // all have been changed to sets, since order doesn't matter
        public ISet<Tower> Towers { get; set; }
        public ISet<Unit> Units { get; set; }
        public ISet<Barrack> Barracks { get; private set; }

        public Player(PlayerType type, Castle? castle = null, ICollection<Barrack>? barracks = null)
        {
            Castle = castle;
            Type = type;
            Money = Constants.PLAYER_STARTING_MONEY;
            Towers = new HashSet<Tower>();
            Units = new HashSet<Unit>();
            if(barracks != null)
                Barracks = new HashSet<Barrack>(barracks);
            else
                Barracks = new HashSet<Barrack>();
        }

        public void AddBarrack(Barrack barrack)
        {
            if (Barracks.Count >= 2) throw new ArgumentException("Player can't have more than 2 barracks");
            Barracks.Add(barrack);
        }
    }
}
