using Exceedra.Common.Utilities;

namespace WPF.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Model;
    using Model.DataAccess;
    using Model.Entity;
    using Shared;

    public abstract class PhasingProfileViewModel : INotifyPropertyChanged
    {
        private readonly PhasingProfile _profile;
        private readonly ObservableCollection<decimal> _values;

        protected PhasingProfileViewModel(PhasingProfile profile)
        {
            _profile = profile;
            _values = new ObservableCollection<decimal>(_profile.Values);
            _values.CollectionChanged += ValuesOnCollectionChanged;
        }

        protected PhasingProfile Profile
        {
            get { return _profile; }
        }

        public int Size
        {
            get { return _profile.Size; }
            set
            {
                if (_profile.Size != value)
                {
                    _profile.Size = value;
                    PropertyChanged.Raise(this, "Size");

                    if (Values.Count != Size)
                        UpdateValuesToCorrectSize(Size);
                }
            }
        }

        public string Name
        {
            get { return _profile.Name; }
            set
            {
                if (_profile.Name != value)
                {
                    _profile.Name = value;
                    PropertyChanged.Raise(this, "Name");
                }
            }
        }

        public string ID
        {
            get { return _profile.ID; }
        }

        public decimal Total
        {
            get { return _profile.Total; }
        }

        public ObservableCollection<decimal> Values
        {
            get { return _values; }
        }

        public ObservableCollection<GridValuesViewModel<decimal>> BindableValues
        {
            get
            {
                return
                    new ObservableCollection<GridValuesViewModel<decimal>>(new[]
                        {new GridValuesViewModel<decimal>(Values)});
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void ValuesOnCollectionChanged(object sender,
                                               NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            _profile.Values = _values.ToList();
            PropertyChanged.Raise(this, "Total");
        }

        public static PhasingProfileViewModel<T> Create<T>(T profile)
            where T : PhasingProfile
        {
            return new PhasingProfileViewModel<T>(profile);
        }

        public Task SaveAsync(IPhasingAccess access)
        {
            return access.SaveAsync(_profile);
        }

        public Task DeleteAsync(IPhasingAccess access)
        {
            return access.DeleteAsync(_profile);
        }

        public void UpdateValuesToCorrectSize(int currentSize)
        {
            if (currentSize > Values.Count)
            {
                var diff = currentSize - Values.Count;
                for (int i = 0; i < diff; i++)
                {
                    Values.Add(0m);
                }
            }
            else
            {
                var diff = Values.Count - currentSize;
                for (int i = 0; i < diff; i++)
                {
                    Values.RemoveAt(Values.Count - 1);
                }
            }
        }
    }

    public class PhasingProfileViewModel<T> : PhasingProfileViewModel
        where T : PhasingProfile
    {
        public PhasingProfileViewModel(T profile) : base(profile)
        {
        }
    }
}