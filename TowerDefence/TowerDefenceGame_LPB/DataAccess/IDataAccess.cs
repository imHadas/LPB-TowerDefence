using System;
using System.Threading.Tasks;
using TowerDefenceBackend.Persistence;

namespace TowerDefenceBackend.DataAccess
{
    /// <summary>
    /// Interface for saving and loading gamestate
    /// </summary>
    /// <typeparam name="TSaveObject">The type of <c>Object</c> the implementation will return and use to save with</typeparam>
    public interface IDataAccess<TSaveObject> where TSaveObject : SaveObject
    {
        /// <summary>
        /// Async method for loading from a file
        /// </summary>
        /// <param name="path">The path of the file to read from</param>
        /// <returns>A <c>TSaveObject</c> containing loaded gamestate</returns>
        Task<TSaveObject> LoadAsync(string path);

        /// <summary>
        /// Async method for saving to a file
        /// </summary>
        /// <param name="path">Path of the file to save to</param>
        /// <param name="saveObject"><c>TSaveObject</c> containing the state of the game which will be saved</param>
        /// <returns></returns>
        Task SaveAsync(string path, TSaveObject saveObject);
    }
}
