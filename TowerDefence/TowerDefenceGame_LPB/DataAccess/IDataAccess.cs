using System;
using System.Threading.Tasks;
using TowerDefenceGame_LPB.Persistence;

namespace TowerDefenceGame_LPB.DataAccess
{
    public interface IDataAccess
    {
        Task<(Table table, Player blue, Player red)> LoadAsync(string path);
        Task SaveAsync(string path, Table table, Player bluePlayer, Player redPlayer);
    }
}
