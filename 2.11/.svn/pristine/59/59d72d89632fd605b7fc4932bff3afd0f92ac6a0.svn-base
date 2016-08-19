using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Entity
{
    using System.Xml.Linq;

    public class ItemCustomer
    {
        private readonly string _id;
        private readonly string _name;

        public string ID
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public ItemCustomer(string id, string name)
        {
            _id = id;
            _name = name;
        }

        public ItemCustomer(XElement xml)
        {
            _id = xml.GetValue<string>("ID");
            _name = xml.GetValue<string>("Name");
        }
    }
}
