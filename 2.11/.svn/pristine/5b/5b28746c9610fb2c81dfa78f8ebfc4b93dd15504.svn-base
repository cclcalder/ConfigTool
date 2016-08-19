using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Coder.UI.WPF
{
    public class BulkObservableCollection<T> : ObservableCollection<T>
    {
        private bool _suppressNotification;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
            {
                base.OnCollectionChanged(e);
            }
        }

        public void AddBulkRange(IEnumerable<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            _suppressNotification = true;

            this.AddRange(list);

            _suppressNotification = false;

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
