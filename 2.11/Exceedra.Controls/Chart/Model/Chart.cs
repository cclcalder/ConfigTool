using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Exceedra.Chart.Model
{
    [Serializable]
    public class Chart : INotifyPropertyChanged
    {
        public string XAxisTitle { get; set; }
        public string YAxisTitle { get; set; }
        public string XAxisType { get; set; }
        public string YAxisType { get; set; }
        public string XAxisBrush { get; set; }
        public string YAxisBrush { get; set; }
        public string ChartType { get; set; }
        public string Title { get; set; }

        public Visibility DisplayXandYTooltip { get; set; }

        public ObservableCollection<SingleSeries> Series { get; set; }



        public event PropertyChangedEventHandler PropertyChanged;

        public List<string> Outliers { get; set; }
    }
}
