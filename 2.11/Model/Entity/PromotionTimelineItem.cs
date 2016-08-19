using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Telerik.Windows.Controls; 
namespace Model.Entity
{
    public class PromotionTimelineItem : INotifyPropertyChanged
    {
        private Brush groupBrush;

        public PromotionTimelineItem()
        {
            // default brush
            this.GroupBrush = new SolidColorBrush(Color.FromArgb(255, 118, 118, 118));
            if (Resources == null)
            {
                Resources = new ResourceCollection();
            }
        }
        public string PromotionID
        {
            get;
            set;
        }

        public string sDuration
        {
            get;
            set;
        }

        public TimeSpan Duration
        {
            get;
            set;
        }

        public string Location
        {
            get;
            set;
        }

        public DateTime StartDate
        {
            get;
            set;
        }

        public DateTime EndDate
        {
            get;
            set;
        }

        public string sStartDate
        {
            get { return StartDate.ToShortDateString(); }
        }

        public string sEndDate
        {
            get { return EndDate.ToShortDateString(); }
        }

        public string Name
        {
            get;
            set;
        }


        public string Customer
        {
            get;
            set;
        }

        public string ShortType
        {
            get { return Type.Left(2); }
         
        }

        public string Type
        {
            get;
            set;
        }


        public string Status
        {
            get;
            set;
        }

        public string StatusColour
        {
            get;
            set;
        }


        public string Category
        {
            get;
            set;
        }

        //public string MatchHeader
        //{
        //    get
        //    {
        //        return string.Format("MatchHeader");
        //    }
        //}

        public string Body
        {
            get;
            set;
        }

        //public string ToolTipHeader
        //{
        //    get
        //    {
        //        return string.Format("Tooltip");
        //    }
        //}

        public Brush GroupBrush
        {
            get
            {
                return this.groupBrush;
            }
            set
            {
                if (this.groupBrush != value)
                {
                    this.groupBrush = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("GroupBrush"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, args);
            }
        }

        public  ResourceCollection Resources { get; set; }

        private Brush _labelBrush;
        public Brush LabelBrush
        {
            get
            {
                return _labelBrush;
            }
            set
            {

                if (_labelBrush != value)
                {
                    _labelBrush = value;

                }
            }
        }

        private SolidColorBrush _backBrush;
        public SolidColorBrush BackBrush
        {
            get
            {
                return _backBrush;
            }
            set
            {

                if (_backBrush != value)
                {
                    _backBrush = value;

                }
            }
        }

        private StatusBrushStorage _availableBrushes;
        public PromotionTimelineItem(StatusBrushStorage brushes)
        {
            if (Resources == null)
            {
                Resources = new ResourceCollection();
            }
            _availableBrushes = brushes;
        }


        public Brush GetProgrammeBrush(string programme)
        {
            programme = programme.Replace("PH", "");
            return _availableBrushes.ProgrammeBrushes[programme];
        }



    }
}
