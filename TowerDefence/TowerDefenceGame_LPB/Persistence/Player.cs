using System.Collections.Generic;
using System.Collections.Immutable;

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

        public Castle Castle { get; private set; }

        // all have been changed to sets, since order doesn't matter
        public ISet<Tower> Towers { get; set; }
        public ISet<Unit> Units { get; set; }
        public ISet<Barrack>? Barracks { get; set; }  // added nullability for neutral player

        public Player(PlayerType type, Castle castle, ICollection<Barrack> barracks)
        {
            Castle = castle;
            Type = type;
            Money = Constants.PLAYER_STARTING_MONEY;
            Towers = new HashSet<Tower>();
            Units = new HashSet<Unit>();
            Barracks = barracks?.ToImmutableHashSet();  // immutable since the barracks won't change mid-game
        }
    }
}
