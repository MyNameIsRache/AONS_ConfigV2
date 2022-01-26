using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AONS_ConfigV2.Base
{
    public class SaveClass
    {
        public string Header { get; set; } = null!;
        List<SaveClassItem> _Items = new List<SaveClassItem>();

        public SaveClass()
        {

        }

        public SaveClass(string pHeader)
        {
            Header = pHeader;
        }

        public SaveClass(string pHeader, params SaveClassItem[] pSCI) : this(pHeader)
        {
            _Items.AddRange(pSCI);
        }

        public SaveClass(string pHeader, params (string pName, string pValue)[] pSCI) : this(pHeader)
        {
            foreach (var item in pSCI)
                AddItem(item.pName, item.pValue);
        }

        public SaveClassItem[] GetItems() => _Items.ToArray();

        public void AddItem(SaveClassItem sci) => _Items.Add(sci);

        public void AddItem(string pName, string pValue, string pComment = null!) => _Items.Add(new SaveClassItem { Name = pName, Value = pValue, Comment = pComment });

        public void RemoveItem(SaveClassItem sci) => _Items.Remove(sci);
    }

    public class SaveClassItem
    {
        public string Comment { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
