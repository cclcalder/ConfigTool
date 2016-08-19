using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Exceedra.Common.Utilities;
using Model.Annotations;
using Telerik.Windows.Controls.ChartView;

namespace Exceedra.Chart.Model
{
    public class SingleSeries : INotifyPropertyChanged
    {
        public string SeriesName { get; set; }
        public string SeriesType { get; set; }
        public string SeriesBrush { get; set; }

        private ObservableCollection<Datapoint> _dataPoints;
        public ObservableCollection<Datapoint> Datapoints
        {
            get
            {
                return _dataPoints;
            }
            set
            {
                _dataPoints = value;
                PropertyChanged.Raise(this, "Datapoints");
            }
        }

        public CartesianAxis IndividualAxis { get; set; }

        // In order to change the colour of the bar series we cannot simply change the "fill", "stroke" or any reasonable property, as they are not accessible.
        // Therefore, we must create our own template, which is a rectangle with customized fill property. This template is stored in the PointTemplate property.
        // The other series types don't need their own templates.
        public DataTemplate PointTemplate { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if(PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}