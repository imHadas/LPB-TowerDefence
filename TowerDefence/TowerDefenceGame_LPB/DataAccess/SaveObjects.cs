using TowerDefenceBackend.Persistence;

namespace TowerDefenceBackend.DataAccess
{
    /// <summary>
    /// Generic object for storing saveable data. (exists for expandability reasons)
    /// </summary>
    public abstract class SaveObject
    {
    }

    /// <summary>
    /// Object for storing entire gamestate
    /// </summary>
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
