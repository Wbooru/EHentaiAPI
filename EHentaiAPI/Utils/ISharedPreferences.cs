using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Utils
{
    public interface ISharedPreferences
    {
        public T getValue<T>(string key, T defValue = default);
        public ISharedPreferences setValue<T>(string key, T value = default);
    }
}
