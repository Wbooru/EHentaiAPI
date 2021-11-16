using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Utils
{
    internal class DefaultTemporaryMemorySharedPreferences : ISharedPreferences
    {
        private Dictionary<string, object> preferences = new();

        public T getValue<T>(string key, T defValue = default) => preferences.TryGetValue(key, out var d) ? (T)d : defValue;
        public ISharedPreferences setValue<T>(string key, T value = default)
        {
            preferences[key] = value;
            return this;
        }
    }
}
