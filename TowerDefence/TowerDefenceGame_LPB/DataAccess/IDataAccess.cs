using System;
using System.Threading.Tasks;
using TowerDefenceBackend.Persistence;

namespace TowerDefenceBackend.DataAccess
{
    public interface IDataAccess<TSaveObject> where TSaveObject : SaveObject
    {
        Task<TSaveObject> LoadAsync(string path);
        Task SaveAsync(string path, TSaveObject saveObject);
    }
}
