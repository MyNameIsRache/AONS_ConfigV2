using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AONS_ConfigV2.Base
{
    //normal Config type:
    //Structure:
    //;comment
    //[Title]
    //;comment
    //Test = Value    ;Possible Comment
    //
    //[OtherTitle]
    //Test = Value     #Possible Comment

    public class AONS_Config : AONS_ConfigBase
    {
        //public bool AutoEscapeValues = true;//todo:

        public char CommentChar = ';'; //other default would be #
        public int CommentIndent = 20; //how much space between value and comment -> only relevant therefore for value comments

        //int -> line, string -> the comment
        public Dictionary<int, string> Comments = new Dictionary<int, string>();
        public List<SaveClass> Content { get; set; } = new List<SaveClass>();

        public AONS_Config(string pSavePath) : base(pSavePath) { }

        public AONS_Config(string pSavePath, bool pReadFile) : this(pSavePath)
        {
            if (pReadFile)
                LoadConfig();
        }

        public void AddComment(int pLine, string pComment) => Comments.Add(pLine, pComment);

        private void CheckWriteComment(ref int pLineNr, StreamWriter pSW)
        {
            if (Comments.ContainsKey(pLineNr))
            {
                pSW.Write($"{CommentChar} {Comments[pLineNr]}");
                WriteNewLine(ref pLineNr, pSW);
            }
        }

        private void WriteNewLine(ref int pLineNr, StreamWriter pSW)
        {
            pSW.Write(NewLine);
            pLineNr++;

            CheckWriteComment(ref pLineNr, pSW);
        }

        public override void SaveConfig()
        {
            using (Stream fs = GetFileStream(true))
            {
                StreamWriter sw = new StreamWriter(fs, Encoding);

                int lnCount = 1;

                foreach (var _Header in Content)
                {
                    CheckWriteComment(ref lnCount, sw);
                    sw.Write($"[{_Header.Header}]");
                    WriteNewLine(ref lnCount, sw);
                    foreach (var _Item in _Header.GetItems())
                    {
                        string pLine = $"{_Item.Name} = {_Item.Value}";
                        sw.Write(pLine);
                        int calcCommentIndent = CommentIndent - pLine.Length;
                        if (!string.IsNullOrEmpty(_Item.Comment))
                            sw.Write($"{new string(' ', calcCommentIndent > 0 ? calcCommentIndent : 5)}{CommentChar} {_Item.Comment}");
                        WriteNewLine(ref lnCount, sw);
                    }
                    //to improve readability add empty line
                    WriteNewLine(ref lnCount, sw);
                }
                //fill up all other comments
                foreach(var comment in Comments.Where(pX => pX.Key > lnCount))
                {
                    while (lnCount < comment.Key)
                        WriteNewLine(ref lnCount, sw);
                }

                sw.Flush();
            }
        }

        private string ReadLine(StreamReader sr)
        {
            //todo: feels incredibly shady, but was the first idea and it worked, so whatever for now

            StringBuilder sb = new StringBuilder();
            string endBuffer = string.Empty;

            while (!sr.EndOfStream)
            {
                endBuffer = string.Empty;
                foreach (var nlChar in NewLine)
                {
                    char c = (char)sr.Read();
                    if (c == nlChar)
                    {
                        endBuffer += c;
                        if (endBuffer == NewLine)
                            return sb.ToString();
                    }
                    else
                    {
                        sb.Append(c);
                        break;
                    }
                }
            }

            return sb.ToString();
        }

        private bool CheckIsCommentLine(int pLineCount, string pLine)
        {
            bool isComment = false;
            if (!string.IsNullOrEmpty(pLine))
            {
                if (pLine.StartsWith(CommentChar))
                {
                    isComment = true;
                }
                else if (pLine.StartsWith(';'))
                {
                    CommentChar = ';';
                    isComment = true;
                }
                else if (pLine.StartsWith('#'))
                {
                    CommentChar = '#';
                    isComment = true;
                }

                if (isComment)
                    Comments[pLineCount] = pLine.Trim().Substring(1);
            }
            return isComment;
        }

        public override void LoadConfig()
        {
            using (Stream fs = GetFileStream(false))
            {
                StreamReader sr = new StreamReader(fs, Encoding);
                int pLineCount = 0;
                SaveClass sc = null!;

                while (!sr.EndOfStream)
                {
                    pLineCount++;

                    string line = ReadLine(sr);

                    if (string.IsNullOrEmpty(line))
                        continue; //no point in checking a empty line

                    if (!CheckIsCommentLine(pLineCount, line))
                    {
                        //is start of Header
                        if (line.StartsWith('['))
                        {
                            if (sc != null)
                                Content.Add(sc);

                            sc = new SaveClass();
                            sc.Header = line.Substring(1, line.IndexOf(']') - 1);
                        }
                        else {
                            string[] pItems = line.Split(new string[] { "=", CommentChar.ToString(), ";", "#" }, 3, StringSplitOptions.RemoveEmptyEntries);
                            sc.AddItem(new SaveClassItem { Name = pItems[0].Trim(), Value = pItems[1].Trim(), Comment = pItems.Length > 2 ? pItems[2].Trim() : default!});
                        }
                    }
                }

                if (sc != null && !Content.Contains(sc))
                    Content.Add(sc);
            }
        }
    }
}
