using System;
using System.Collections.Generic;

namespace TowerDefenceBackend.Persistence
{
    /// <summary>
    /// For visual representation and differentiation of <c>Player</c>s
    /// </summary>
    public enum PlayerType
    {
        NEUTRAL,
        RED,
        BLUE
    }

    /// <summary>
    /// Class for storing data related to a specific player
    /// </summary>
    public class Player
    {
        public PlayerType Type { get; private set; }
        public uint Money { get; set; }
        public Castle? Castle { get; set; }
        public ISet<Tower> Towers { get; set; }
        public ISet<Unit> Units { get; set; }
        public ISet<Barrack> Barracks { get; set; }

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

        /// <summary>
        /// Residual method for adding <c>Barrack</c>s
        /// </summary>
        /// <param name="barrack"><c>Barrack</c> to add to <c>Player</c>'s collection</param>
        /// <exception cref="ArgumentException">Thrown if <c>Player</c> already has 2 <c>Barrack</c>s</exception>
        [Obsolete("You can directly add to Barracks collection", false)]
        public void AddBarrack(Barrack barrack)
        {
            if (Barracks.Count >= 2) throw new ArgumentException("Player can't have more than 2 barracks");
            Barracks.Add(barrack);
        }
    }
}
