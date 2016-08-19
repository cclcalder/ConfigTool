using ViewModels; 
using Exceedra.Common.Utilities; 

namespace WPF.ViewModels.Generic
{
    public class DataGridViewModel : ViewModelBase
    {

        private DynamicDataView _dataItems;
        public  DynamicDataView DataItems { 
            get
            { return _dataItems; }
            set {
                _dataItems = value;
                NotifyPropertyChanged(this, vm => vm.DataItems);
            }
        }

      
        public DataGridViewModel()
        {
            DataItems = new  DynamicDataView();
        }


    }
}
