using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity.Storage.Azure
{
    public class Settings
    {
        public Settings()
        {
        }

        public Settings(XElement items)
        {
            AccountName = items.Element("AccountName").MaybeValue();
            AccountKey = items.Element("AccountKey").MaybeValue();
            Container = items.Element("Container").MaybeValue();
        }

        public string AccountName { get; set; }
        public string AccountKey { get; set; }    
        public string Container { get; set; }
    }
}

