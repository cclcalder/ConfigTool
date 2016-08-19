
using System;
using System.ComponentModel;
using System.Windows.Data;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.Utilities;
using Model.DataAccess.Converters;

namespace Model.Entity.Funds
{
    public class FundDetail : INotifyPropertyChanged
    {

        public string ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool Amendable { get; set; }

        private bool _isParent;
        public bool IsParent 
        {
            get { return _isParent; }
            set
            {
                _isParent = value;
                PropertyChanged.Raise(this, "IsParent");
            } 
        }

        private DateTime? _date_start;
        public DateTime? Date_Start
        {
            get
            {
                return _date_start;
            }
            set
            {
                _date_start = value;
                PropertyChanged.Raise(this, "Date_Start");
            }
        }

        private DateTime? _date_end;
        public DateTime? Date_End
        {
            get
            {
                return _date_end;
            }
            set
            {
                _date_end = value;
                PropertyChanged.Raise(this, "Date_End");
            }
        }

        public static FundDetail FromXml(XElement xml)
        {
            return new FundDetail
            {
                ID = xml.Element("Fund_Idx").MaybeValue(),
                Name = xml.Element("Fund_Name").MaybeValue(),
                Date_Start = Convert.ToDateTime(xml.Element("Date_Start").MaybeValue()),
                Date_End = Convert.ToDateTime(xml.Element("Date_End").MaybeValue()),
                Amendable = xml.Element("Amendable").MaybeValue() == "1",
                IsParent = xml.MaybeElement("IsParent").MaybeValue() == "1",
                Code = xml.Element("Fund_Code").MaybeValue(),
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
