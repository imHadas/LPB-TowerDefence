using TowerDefenceGame_LPB.Persistence;

namespace TowerDefenceGame_LPB.DataAccess
{
    internal class GameSaveObject
    {
        public Table Table { get; private set; }
        public Player BluePlayer { get; private set; }
        public Player RedPlayer { get; private set; }

        public GameSaveObject(Table table, Player bluePlayer, Player redPlayer)
        {
            Table = table;
            BluePlayer = bluePlayer;
            RedPlayer = redPlayer;
        }
    }
}
