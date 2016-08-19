using System; 
using System.Collections.ObjectModel;
using System.ComponentModel;
using Exceedra.Common.Utilities;

namespace Exceedra.Chart.Model
{
    [Serializable]
    public class Record : INotifyPropertyChanged
    {

        public Record Clone()
        {
            return CreateDeepCopy();
        }

        public string Item_Type { get; set; }
        public string Item_Idx { get; set; }
        public int Item_RowSortOrder { get; set; }

        private ObservableCollection<Datapoint> _datapoints;
        public ObservableCollection<Datapoint> Datapoints
        {
            get { return _datapoints; }
            set
            {
                _datapoints = value;
                PropertyChanged.Raise(this, "Datapoints");
            }
        }

        private Chart _chart;
        public Chart Chart
        {
            get { return _chart; }
            set
            {
                _chart = value;
                PropertyChanged.Raise(this, "Chart");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        Record CreateDeepCopy()
        {
            object result = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
                                            new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bf.Serialize(ms, this);

                ms.Position = 0;
                result = bf.Deserialize(ms);
            }

            return result as Record;
        }
    }
}
