
using Exceedra.Common;

namespace Model.Language
{
    public class LanguageSet //: ViewModelBase
    {
        private System.Xml.Linq.XElement r;

       public LanguageSet(string c, string n)
       {
           Code = c;
           Name = n;
       }

       public LanguageSet(System.Xml.Linq.XElement el)
       {
            Code = el.Element("LabelKey").MaybeValue() ?? el.Element("Message_Code").MaybeValue();
            Name = el.Element("LabelValue").MaybeValue() ?? el.Element("Message_Text").MaybeValue();

        }
       public string Code { get; set; }
       public string Name { get; set; }

    }
}
