using Model.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.Utilities;

namespace Model.Entity
{
    public class EventDetail : INotifyPropertyChanged
    {
        public string Event_Idx { get; set; }
        public string Event_Start_Date { get; set; }
        public string Event_End_Date { get; set; }
        public string Event_Name { get; set; }
        public string Event_Type { get; set; }
        public string Event_Sub_Type { get; set; }
        public string Event_Status_Idx { get; set; }

        public string Total_Accrual { get; set; }
        public string ReportURL { get; set; }
        public string Net_Accrual
        {
            get
            {
                return (Convert.ToDouble(Total_Accrual) + TotalAdjustment).ToString();
            }
        }




        public string Settled { get; set; }
        public string MatchedClaims { get; set; }

        public string Reason_Code_Idx { get; set; }
        public double TotalAdjustment
        {
            get
            {
                return Adjustments.Sum(r => Convert.ToDouble(r.Adjustment_Value));
            }
        }

        private ObservableCollection<EventDetailAdjustment> _adjustments;
        public ObservableCollection<EventDetailAdjustment> Adjustments
        {
            get
            { return _adjustments; }
            set
            {
                _adjustments = value;
                OnPropertyChanged("Adjustments");
                OnPropertyChanged("TotalAdjustment");
            }
        }
        const string dateFormat = "yyyy-MM-dd";

        public static EventDetail FromXml(XElement element)
        {
            var res = new EventDetail
            {
                Event_Idx = element.GetValue<string>("Event_Idx"),
                Event_Name = element.GetValue<string>("Event_Name"),
                Event_Start_Date = DateTime.ParseExact(element.GetValue<string>("Event_Start_Date"), dateFormat, null).ToShortDateString(),
                Event_End_Date = DateTime.ParseExact(element.GetValue<string>("Event_End_Date"), dateFormat, null).ToShortDateString(),
                Event_Type = element.GetValue<string>("Event_Type"),
                Event_Sub_Type = element.GetValue<string>("Event_Sub_Type"),
                Event_Status_Idx = element.GetValue<string>("Event_Status_Idx"),
                Total_Accrual = element.GetValue<string>("Total_Accrual"),
                Settled = element.GetValue<string>("Settled"),
                MatchedClaims = element.GetValue<string>("MatchedClaims"),
                Reason_Code_Idx = element.GetValue<string>("Reason_Code_Idx"),
                ReportURL = element.Element("Report_URL").MaybeValue(),
                Adjustments = EventDetailAdjustment.FromXml(element.GetElement("Results"))
            };

            return res;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        //    public string Id { get; set; }
        //    public string Name { get; set; }
        //    public string EventType { get; set; }
        //    public string EventSubType { get; set; }
        //    public string EventStatusId { get; set; }
        //    public string TotalAccrual { get; set; }
        //    public string Settled { get; set; }
        //    public string EventAdjustment { get; set; }

        //    public DateTime StartDate { get; set; }
        //    public DateTime EndDate { get; set; }


        //    //public string AvailableAccrual { get; set; }
        //    public string MatchedClaims { get; set; }
        //    public string ReasonCodeId { get; set; }

        //    public static EventDetail FromXml(XElement element)
        //    {
        //        const string idElement = "Event_Idx";
        //        const string nameElement = "Event_Name";
        //        const string typeElement = "Event_Type";
        //        const string subtypeElement = "Event_Sub_Type";
        //        const string statusElement = "Event_Status_Idx";
        //        const string totalAccrualElement = "Total_Accrual";
        //        const string settledElement = "Settled";
        //        const string eventAdjustmentElement = "Event_Adjustment";
        //        //const string availableAccrualElement = "Available_Accrual";
        //        const string matchedClaimslement = "MatchedClaims";
        //        const string reasonCodeIdElement = "Reason_Code_Idx";

        //        const string sDate = "AdjustmentStartDate";
        //        const string eDate = "AdjustmentEndDate";


        //      var r =  new EventDetail
        //        {
        //            Id = element.GetValue<string>(idElement),
        //            Name = element.GetValue<string>(nameElement),
        //            EventType = element.GetValue<string>(typeElement),
        //            EventSubType = element.GetValue<string>(subtypeElement),
        //            EventStatusId = element.GetValue<string>(statusElement),
        //            TotalAccrual = element.GetValue<string>(totalAccrualElement),
        //            Settled = element.GetValue<string>(settledElement),
        //            EventAdjustment = element.GetValue<string>(eventAdjustmentElement),
        //            //AvailableAccrual = element.GetValue<string>(availableAccrualElement),
        //            MatchedClaims = element.GetValue<string>(matchedClaimslement),
        //            ReasonCodeId = element.GetValue<string>(reasonCodeIdElement),
        //            StartDate = element.GetValue<DateTime>(sDate),
        //            EndDate = element.GetValue<DateTime>(eDate)
        //        };

        //      return r;
        //    }


    }
    public class EventDetailAdjustment : INotifyPropertyChanged
    {

        public string Adjustment_Row_Idx { get; set; }

        private DateTime? _adjustment_Start_Date;
        public DateTime? Adjustment_Start_Date
        {
            get { return _adjustment_Start_Date; }
            set
            {
                _adjustment_Start_Date = value;
                PropertyChanged.Raise(this, "Adjustment_Start_Date");
                PropertyChanged.Raise(this, "Adjustment_Start_Date_Format");
            }
        }
        public string Adjustment_Start_Date_Format
        {
            get
            {
                if (Adjustment_Start_Date != null)
                    return ((DateTime)Adjustment_Start_Date).ToShortDateString();

                return string.Empty;
            }
        }

        private DateTime? _adjustment_End_Date;
        public DateTime? Adjustment_End_Date
        {
            get { return _adjustment_End_Date; }
            set
            {
                _adjustment_End_Date = value;
                PropertyChanged.Raise(this, "Adjustment_End_Date");
                PropertyChanged.Raise(this, "Adjustment_End_Date_Format");
            }
        }
        public string Adjustment_End_Date_Format
        {
            get
            {
                if (Adjustment_End_Date != null)
                    return ((DateTime)Adjustment_End_Date).ToShortDateString();

                return string.Empty;
            }
        }

        private string _adjustment_Value;
        public string Adjustment_Value
        {
            get;
            set;
            //get
            //{
            //    return _adjustment_Value;
            //}
            //set
            //{
            //    _adjustment_Value = value;
            //    OnPropertyChanged("EventDetailAdjustment");
            //}
        }
        public string Adjustment_Comment { get; set; }

        public string Adjustment_User { get; set; }
        public string Adjustment_Create_Date { get; set; }


        public static ObservableCollection<EventDetailAdjustment> FromXml(XElement element)
        {
            const string dateFormat = "yyyy-MM-dd";
            var adjustments = new List<EventDetailAdjustment>();

            if (element == null)
                return new ObservableCollection<EventDetailAdjustment>(adjustments);

            var ri = (from r in element.Descendants("Adjustment")
                      select r);

            if (ri == null)
                return new ObservableCollection<EventDetailAdjustment>(adjustments);

            foreach (var rec in ri.ToList())
            {

                var p = new EventDetailAdjustment()
                {
                    Adjustment_Row_Idx = rec.GetValue<string>("Adjustment_Row_Idx"),
                    Adjustment_Start_Date = DateTime.ParseExact(rec.GetValue<string>("Adjustment_Start_Date"), dateFormat, null),
                    Adjustment_End_Date = DateTime.ParseExact(rec.GetValue<string>("Adjustment_End_Date"), dateFormat, null),
                    Adjustment_Value = rec.GetValue<string>("Adjustment_Value"),
                    Adjustment_Comment = rec.Element("Adjustment_Comment").MaybeValue(),
                    Adjustment_User = rec.GetValue<string>("Adjustment_User"),
                    Adjustment_Create_Date = DateTime.Parse(rec.GetValue<string>("Adjustment_Create_Date")).ToString()
                };

                adjustments.Add(p);
            }


            return new ObservableCollection<EventDetailAdjustment>(adjustments);
        }


        public event PropertyChangedEventHandler PropertyChanged;

    }



}
