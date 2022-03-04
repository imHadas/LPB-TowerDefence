using System;
using System.Threading.Tasks;
using TowerDefenceGame_LPB.Persistence;

namespace TowerDefenceGame_LPB.DataAccess
{
    public interface IDataAccess
    {
        Task<Table> LoadAsync(String path);
        Task SaveAsync(String path, Table table);
    }
}
