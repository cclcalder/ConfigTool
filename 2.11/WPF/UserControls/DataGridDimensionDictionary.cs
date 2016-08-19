namespace WPF.UserControls
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows.Controls;

    public class DataGridDimensionDictionary : Dictionary<string, DataGridLength>, INotifyCollectionChanged
    {
        public new DataGridLength this[string index]
        {
            get
            {
                if (!ContainsKey(index)) Add(index, 0);
                return base[index];
            }
            set
            {
                if (value.Value > this[index].Value)
                {
                    base[index] = value;
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion
    }
}