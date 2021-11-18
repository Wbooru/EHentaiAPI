using EHentaiAPI.ExtendFunction;
using EHentaiAPI.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.TestConsole
{
    public class FileSharedPreferences : ISharedPreferences
    {
        private Dictionary<string, object> store = new Dictionary<string, object>();
        private const string FILE_PATH = "config.json";

        public FileSharedPreferences()
        {
            if (File.Exists(FILE_PATH))
            {
                Log.LogImplement.I("FileSharedPreferences", $"Load config from {FILE_PATH}");
                store = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(FILE_PATH));
            }
        }

        public T getValue<T>(string key, T defValue = default)
        {
            var value = store.TryGetValue(key, out var d) ? (T)d : defValue;
            Log.LogImplement.I("FileSharedPreferences", $"Get {key} ===> {value} {(defValue.Equals(value) ? "(DEFAULT)" : "")}");
            return value;
        }

        public ISharedPreferences setValue<T>(string key, T value = default)
        {
            Log.LogImplement.I("FileSharedPreferences", $"Set {key} = {value}");
            store[key] = value;
            File.WriteAllText(FILE_PATH, JsonConvert.SerializeObject(store, Formatting.Indented));
            return this;
        }
    }
}
