using System;
using System.Threading.Tasks;
using TowerDefenceGame_LPB.Persistence;
using System.IO;
using System.Text.Json;  //use the .NET 6 serializer
using System.Text.Json.Serialization;

namespace TowerDefenceGame_LPB.DataAccess
{
    public class JsonDataAccess : IDataAccess
    {
        private static readonly JsonSerializerOptions _options
        = new() 
        { 
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true,
        };

        public async Task<(Table table, Player blue, Player red)> LoadAsync(string path)
        {
            GameSaveObject? gameSaveObject;
            using (FileStream fs = File.Create(path))
            {
                gameSaveObject = (GameSaveObject?)await JsonSerializer.DeserializeAsync(fs, typeof(GameSaveObject), _options);
            }

            if (gameSaveObject == null) throw new Exception("Error reading file");

            return (gameSaveObject.Table, gameSaveObject.BluePlayer, gameSaveObject.RedPlayer);

        }
        public async Task SaveAsync(string path, Table table, Player bluePlayer, Player redPlayer)
        {
            GameSaveObject gameSaveObject = new(table, bluePlayer, redPlayer);

            using (FileStream fs = File.Create(path))
            {
                await JsonSerializer.SerializeAsync(fs, gameSaveObject, _options);
            }
        }
    }
}
