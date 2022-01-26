using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AONS_ConfigV2.XX_Exception
{
    public class AONS_ConfigExceptions : Exception
    {
        public const int ERR_NO = -1;

        public AONS_ConfigExceptions(string pMessage) : base(pMessage) { }

        public virtual int GetErrorNo() => ERR_NO;
    }

    public class AONS_ConfigFileNotFoundException : AONS_ConfigExceptions
    {
        public new const int ERR_NO = 1;

        public AONS_ConfigFileNotFoundException(string pMessage) : base(pMessage) { }
    }
}
