using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Localization.Common;
using Newtonsoft.Json;

namespace CSLocalization.Shared
{
    public class Project
    {
        public string Title { get; set; }

        public GeneratorConfig GeneratorConfig { get; set; } = new GeneratorConfig();

        public LocalizationDictionary Localization { get; set; } = new LocalizationDictionary();

        [JsonIgnore()]
        public string Path { get; internal set; }

        [JsonIgnore()]
        public ProjectManager Manager { get; internal set; }

        public void Export()
        {
            Directory.CreateDirectory(Manager.RootDirectory);
            using (var sw = File.CreateText(Path))
            {
                sw.Write(JsonConvert.SerializeObject(this));
            }
        }
    }
}
