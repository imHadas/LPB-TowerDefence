using System;
using System.Threading.Tasks;
using TowerDefenceGame_LPB.Persistence;

namespace TowerDefenceGame_LPB.DataAccess
{
    public interface IDataAccess<TSaveObject> where TSaveObject : SaveObject
    {
        Task<TSaveObject> LoadAsync(string path);
        Task SaveAsync(string path, TSaveObject saveObject);
    }
}
