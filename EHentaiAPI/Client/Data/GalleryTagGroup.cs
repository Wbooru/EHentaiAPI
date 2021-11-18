using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Data
{
    public class GalleryTagGroup
    {
        public List<string> TagList { get; set; }
        public string TagGroupName { get; set; }

        public int Size => TagList.Count;
        public string GetTagAt(int index) => TagList.ElementAtOrDefault(index);
        public void AddTag(string tag) => TagList.Add(tag);
    }
}
