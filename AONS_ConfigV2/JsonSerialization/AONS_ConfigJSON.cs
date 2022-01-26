using AONS_ConfigV2.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace AONS_ConfigV2.JsonSerialization
{
    public class AONS_ConfigJSON<T> : AONS_ConfigBase where T : new()
    {
        public bool DoPrettyPrint = true;

        public T Content { get; set; } = default!;
        public JsonSerializerOptions JsonOptions { get; set; } = null!;

        public AONS_ConfigJSON(string pSavePath) : base(pSavePath)
        {

        }

        public AONS_ConfigJSON(string pSavePath, bool pReadFile) : this(pSavePath)
        {
            if (pReadFile)
                LoadConfig();
        }

        public override void LoadConfig()
        {
            using (Stream fs = GetFileStream(false))
            {
                Content = (T)JsonSerializer.Deserialize(fs, typeof(T), JsonOptions)!;
            }
        }

        public override void SaveConfig()
        {
            using (Stream fs = GetFileStream(true))
            {
                //ensure there is an jsonOptions and it has the correct Indented value set
                (JsonOptions = JsonOptions ?? new JsonSerializerOptions()).WriteIndented = DoPrettyPrint;

                JsonSerializer.Serialize(fs, Content, JsonOptions);
                fs.Flush();
            }
        }
    }
}
