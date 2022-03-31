using System;
using System.Threading.Tasks;
using TowerDefenceGame_LPB.Persistence;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TowerDefenceGame_LPB.DataAccess
{
    public class DataAccess : IDataAccess
    {
        private static readonly JsonSerializerSettings _options
        = new() { NullValueHandling = NullValueHandling.Ignore };

        public async Task<Table> LoadAsync(String path)
        {
            throw new NotImplementedException();
        }
        public async Task SaveAsync(String path, Table table)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;
            serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

            using (StreamWriter sw = new StreamWriter(path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, table);
            }
            /*
            string jsonString = JsonConvert.SerializeObject(table);

            //var serializedObject = Newtonsoft.Json.JsonConvert.SerializeObject(table);

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(jsonString);
            }*/
        }
    }
}
