using System;
using System.Threading.Tasks;
using TowerDefenceGame_LPB.Persistence;
using System.IO;
using System.Text.Json;  //use the .NET 6 serializer
using System.Text.Json.Serialization;

namespace TowerDefenceGame_LPB.DataAccess
{
    public class JsonDataAccess : IDataAccess<GameSaveObject>
    {
        private static readonly JsonSerializerOptions _options
        = new() 
        { 
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true,
        };

        public async Task<GameSaveObject> LoadAsync(string path)
        {
            GameSaveObject? gameSaveObject;
            using (FileStream fs = File.OpenRead(path))
            {
                gameSaveObject = (GameSaveObject?)await JsonSerializer.DeserializeAsync(fs, typeof(GameSaveObject), _options);
            }

            if (gameSaveObject == null) throw new Exception("Error reading file");

            return gameSaveObject;

        }
        public async Task SaveAsync(string path, GameSaveObject gameSaveObject)
        {
            using (FileStream fs = File.Create(path))
            {
                await JsonSerializer.SerializeAsync(fs, gameSaveObject, _options);
            }
        }
    }
}
