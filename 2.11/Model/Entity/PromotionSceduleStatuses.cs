using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.Utilities;

namespace Model.Entity
{
    [Serializable()]
    public class ScheduleStatuses : INotifyPropertyChanged
    {
  //<Results>
  //  <Type>
  //    <Type_Idx>0</Type_Idx>
  //    <Type_Name>Promotion</Type_Name>
  //    <Statuses>
  //     <Status>
  //      <Status_Idx>1</Status_Idx>
  //      <Status_Name>Draft</Status_Name>
  //      <Status_Colour>#FFFFFF</Status_Colour>
  //      <IsSelected>1</IsSelected>
  //     </Status>
  //     <Status>
  //      <Status_Idx>2</Status_Idx>
  //      <Status_Name>Planned</Status_Name>
  //      <Status_Colour>#8B668B</Status_Colour>
  //      <IsSelected>1</IsSelected>
  //     </Status>
  //    </Statuses>
  //  </Type>
  //</Results>

        public string ID {get;set;}
        public string Name { get; set; }

        private List<ScheduleStatusesStatus> _statuses;
        public List<ScheduleStatusesStatus> Statuses { 
            get {
                return _statuses;
            }
            set {
                _statuses = value;
                PropertyChanged.Raise(this,"Statuses");                 
            }
        
        }

       
        public ScheduleStatuses() { }
        public ScheduleStatuses(XElement el)
        {
        

            ID = el.GetValue<string>("Type_Idx");
            Name = el.GetValue<string>("Type_Name");
            Statuses  = el.Elements("Statuses").Elements("Status").Select(ScheduleStatusesStatus.FromXml).ToList();

            foreach (var s in Statuses)
            {
                s.Tag = Name;
            }
        }

        public static ScheduleStatuses FromXml(XElement el)
        {
            ScheduleStatuses res = new ScheduleStatuses(el);
            return res;
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ScheduleStatusesStatus : INotifyPropertyChanged
        {
        public event PropertyChangedEventHandler PropertyChanged;

            public ScheduleStatusesStatus(XElement el)
            {
                ID = el.GetValue<string>("Status_Idx");
                Name = el.GetValue<string>("Status_Name");
                IsSelected = el.GetValue<int>("IsSelected") == 1 ? true : false;
                IsEnabled = true;// el.GetValue<int>("IsEnabled") == 1 ? true : false;

                var c = el.Element("Status_Colour").MaybeValue();

                Colour = (c=="" ? "#ffffff" : c);
                
            }

            #region ID
            /// <summary>
            /// Gets or sets the Id of this PromotionStatus.
            /// </summary>
            public string ID { get; set; }
            #endregion

            #region Name
            /// <summary>
            /// Gets or sets the Name of this PromotionStatus.
            /// </summary>
            public string Name { get; set; }
            #endregion

            #region IsSelected
            /// <summary>
            /// Gets or sets the IsSelected of this PromotionStatus.
            /// </summary>
            private bool _isSelected; 
            public bool IsSelected {
                get {
                    return _isSelected;
                }
                set {
                    if (_isSelected != value)
                    {
                        _isSelected = value; 
                    }                   
                
                    PropertyChanged.Raise(this, "IsSelected");
                    }
            }
            #endregion

           

            #region IsEnabled
            /// <summary>
            /// Gets or sets the IsEnabled of this PromotionStatus.
            /// </summary>
            public bool IsEnabled { get; set; }
            #endregion

            public string Colour { get; set; }

            public static ScheduleStatusesStatus FromXml(XElement el)
            {
                ScheduleStatusesStatus res = new ScheduleStatusesStatus(el);
                return res;
            }

            public string Tag { get; set; }
        }
}