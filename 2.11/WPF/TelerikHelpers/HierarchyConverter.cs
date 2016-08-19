using Model.Entity.Admin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using ViewModels;

namespace WPF.TelerikHelpers
{
    public class HierarchyConverter : ViewModelBase, IValueConverter 
    {
        //Somewhat modified from the original telerik code in order to work with admin screen pattern 2
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // We are binding an item
            ObservableCollection<AdminApplySelectionList> item = value as ObservableCollection<AdminApplySelectionList>;
            if (item != null)
            {
                foreach(var element in item)
                {
                    if (item.Where(i => i.ParentID == element.ID) != null)
                    {
                        return element;
                    }
                }              
            }

            // We are binding the treeview
            ObservableCollection<AdminApplySelectionList> items = value as ObservableCollection<AdminApplySelectionList>;
            if (items != null)
            {
                foreach (var element in items)
                {
                    if (element.ParentID == "0")
                    {
                        return element;
                    }
                    
                }
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
