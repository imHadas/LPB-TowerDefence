using System;
using System.Collections.Generic;

namespace TowerDefenceBackend.Persistence
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
        public uint Money { get; set; }

        public Castle? Castle { get; set; }

        public ISet<Tower> Towers { get; set; }
        public ISet<Unit> Units { get; set; }
        public ISet<Barrack> Barracks { get; set; }

        /// <summary>
        /// Check for the validity of the <c>Player</c>
        /// </summary>
        public bool Valid => Type is PlayerType.RED or PlayerType.BLUE &&
                             Castle is not null &&
                             Barracks.Count == 2;

        public Player(PlayerType type, Castle? castle = null, ICollection<Barrack>? barracks = null)
        {
            if (type == PlayerType.NEUTRAL) 
                throw new ArgumentException("You cannot instatiate a player as NEUTRAL", nameof(type));
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
        
    }
}
