using AONS_ConfigV2.Base;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AONS_ConfigV2.XmlSerialization
{
    public class AONS_ConfigXML<T> : AONS_ConfigBase where T : new()
    {
        public bool DoXmlOverhead { get; set; } = false;

        public bool DoPrettyPrint { get; set; } = true;
        public byte Indent { get; set; } = 4; //todo: currently not used

        public bool RemoveDefaultNamespace { get; set; } = true;

        public T Content { get; set; } = default!;
        public XmlRootAttribute Root { get; set; } = null!;
        public XmlWriterSettings WriterSettings { get; set; } = null!;
        public XmlSerializerNamespaces Namespace { get; set; } = null!;

        public AONS_ConfigXML(string pSavePath) : base(pSavePath)
        {

        }

        public AONS_ConfigXML(string pSavePath, bool pReadFile) : this(pSavePath)
        {
            if (pReadFile)
                LoadConfig();
        }

        private void SetWriterSettings()
        {
            if (WriterSettings == null)
                WriterSettings = new XmlWriterSettings();
            WriterSettings.Indent = DoPrettyPrint;
            WriterSettings.IndentChars = new string(' ', Indent);
            WriterSettings.Encoding = Encoding;
            WriterSettings.NewLineChars = NewLine;
            WriterSettings.OmitXmlDeclaration = !DoXmlOverhead;
        }

        private void SetNamespaceSettings()
        {
            if (Namespace == null)
                Namespace = new XmlSerializerNamespaces();

            if (RemoveDefaultNamespace)
                Namespace.Add("", ""); //add empty namespace to ultimatly remove it
        }

        private XmlSerializer GetSerializer()
        {
            if (Root != null)
                return new XmlSerializer(typeof(T), Root);
            else
                return new XmlSerializer(typeof(T));
        }

        public override void LoadConfig()
        {
            using (Stream fs = GetFileStream(false))
            {
                Content = (T)GetSerializer().Deserialize(fs)!;
            }
        }

        public override void SaveConfig()
        {
            using (Stream fs = GetFileStream(true))
            {
                SetWriterSettings();
                SetNamespaceSettings();

                var xmlWriter = XmlWriter.Create(fs, WriterSettings);

                GetSerializer().Serialize(xmlWriter, Content, Namespace);
                fs.Flush();
            }
        }
    }
}
