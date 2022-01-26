using AONS_ConfigV2.XX_Exception;
using System.Text;

namespace AONS_ConfigV2.Base
{
    //todo: - SaveClass Support for other Types? -> probably not though
    //      - SaveType with pipes, ignoring variable names, just mapping the values splitted by the pipes: "-1|1|3|Hello"
    //      - Encryption, probably simply taking the file and using a predefined Key to it

    public abstract class AONS_ConfigBase
    {
        public string SavePath;
        public Encoding Encoding = Encoding.Default;
        public string NewLine = Environment.NewLine;

        public AONS_ConfigBase(string pSavePath)
        {
            SavePath = pSavePath;
        }

        ~AONS_ConfigBase()
        {
            CloseFileStream();
        }

        #region Abstract Functions
        public abstract void SaveConfig();

        public abstract void LoadConfig();
        #endregion

        #region FileHandling
        FileStream fsSaveStream = null!;

        protected bool DoesFileExist()
        {
            return File.Exists(SavePath);
        }

        protected Stream GetFileStream(bool pNewFile)
        {
            //in case there is an old filestream open, close it
            CloseFileStream();

            if (!pNewFile && !DoesFileExist())
                throw new AONS_ConfigFileNotFoundException($"File at \"{SavePath}\" can not be found!");

            fsSaveStream = new FileStream(SavePath, pNewFile ? FileMode.Create : FileMode.Open, FileAccess.ReadWrite);
            return fsSaveStream;
        }

        protected void CloseFileStream()
        {
            fsSaveStream?.Dispose();
        }
        #endregion
    }
}