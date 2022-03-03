using System;
using System.Threading.Tasks;
using TowerDefenceGame_LPB.Persistence;

namespace TowerDefenceGame_LPB.DataAccess
{
    public class DataAccess : IDataAccess
    {
        public async Task<Table> LoadAsync(String path)
        {
            throw new NotImplementedException();
        }
        public async Task SaveAsync(String path, Table table)
        {
            throw new NotImplementedException();
        }
    }
}
