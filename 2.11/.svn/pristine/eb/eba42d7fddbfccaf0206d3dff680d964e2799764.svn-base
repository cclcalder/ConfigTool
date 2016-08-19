using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.ViewModels;
using Telerik.Windows.Controls;

namespace Exceedra.SearchableMultiSelect
{
    public class SearchacbleMultiSelectControlViewModel : Base
    {
        public SearchacbleMultiSelectControlViewModel()
        {
            AllComboBoxItems = new ObservableCollection<ComobBoxSourceModel>();
            VisiaibleComboBoxItems = new ObservableCollection<ComobBoxSourceModel>();
        }

        //public SearchacbleMultiSelectControlViewModel(ObservableCollection<Record> recordObservableCollection)
        //{

        //    foreach (var row in recordObservableCollection)
        //    {
        //        var thisItem = new RadComboBoxItem();
        //        thisItem.Content = row.Properties.First(a => a.ColumnCode == column.ColumnCode).Value;

        //    }
        //}

        public void UpdateComboBoxItems(string Id, string name, bool isSelected = true)
        {
            ComobBoxSourceModel thisModel = new ComobBoxSourceModel();


            thisModel.Id = Id;
            thisModel.Name = name;
            thisModel.IsSelected = isSelected;

            AllComboBoxItems.Add(thisModel);
            
        }


        public void loadWithDummyData()
        {
            var a = 0;
            while (a < 10)
            {
                ComobBoxSourceModel newModel = new ComobBoxSourceModel();
                newModel.Id = a.ToString();
                newModel.Name = "name" + a.ToString();
                newModel.IsSelected = true;
                AllComboBoxItems.Add(newModel);
                a = a + 1;
            }

            VisiaibleComboBoxItems.AddRange(AllComboBoxItems);
        }

        private ObservableCollection<ComobBoxSourceModel> _allComboBoxItems; 
        public ObservableCollection<ComobBoxSourceModel> AllComboBoxItems
        {
            get { return _allComboBoxItems; }
            set
            {
                _allComboBoxItems = value;
                NotifyPropertyChanged(this, vm => vm.AllComboBoxItems);
            }
        }

        private ObservableCollection<ComobBoxSourceModel> _visibleComboBoxItems;

        public ObservableCollection<ComobBoxSourceModel> VisiaibleComboBoxItems
        {
            get { return _visibleComboBoxItems; }
            set
            {
                _visibleComboBoxItems = value;
                NotifyPropertyChanged(this, vm => vm.VisiaibleComboBoxItems);
            }
        }

    }
}
