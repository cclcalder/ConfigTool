using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exceedra.Common;

namespace Model.Entity
{
    using System.Xml.Linq;

    public class Item
    {
        private readonly string _id;
        private readonly string _name;

        public Item(XElement element) : this(element.GetValueOrDefault<string>("ID"), element.GetValueOrDefault<string>("Name"))
        {
            
        }

        public Item(string id, string name)
        {
            _id = id;
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public string ID
        {
            get { return _id; }
        }
    }
}
