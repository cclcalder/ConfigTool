using Exceedra.Common.Utilities;

namespace WPF.ViewModels.Shared
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using Model;

    public class GridValuesViewModel<T> : INotifyPropertyChanged
    {
        private readonly ObservableCollection<T> _values;

        public GridValuesViewModel(string rowName, ObservableCollection<T> values): this(values)
        {
            RowName = rowName;
        }

        public GridValuesViewModel(ObservableCollection<T> values)
        {
            _values = values;
            _values.CollectionChanged += ValuesOnCollectionChanged;
        }

        public ObservableCollection<T> Values
        {
            get { return _values; }
        }

        public string RowName { get; private set; }

        public T Total
        {
            get
            {
                return
                    (T)
                    Convert.ChangeType(_values.Sum(v => (decimal) Convert.ChangeType(v, typeof (decimal))), typeof (T));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void ValuesOnCollectionChanged(object sender,
                                               NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            PropertyChanged.Raise(this, "Total");
        }
    }
}