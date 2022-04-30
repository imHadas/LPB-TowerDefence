using TowerDefenceBackend.Persistence;

namespace TowerDefenceBackend.DataAccess
{
    public abstract class SaveObject { }

    public class GameSaveObject : SaveObject
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
