using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace Exceedra.SearchableMultiSelect
{
    /// <summary>
    /// Interaction logic for SearchableMultiSelectControl.xaml
    /// </summary>
    public partial class SearchableMultiSelectControl : UserControl
    {
        private SearchacbleMultiSelectControlViewModel _viewModel;

        public SearchableMultiSelectControl()
        {
            InitializeComponent();

            _viewModel = new SearchacbleMultiSelectControlViewModel();
            DataContext = _viewModel;
        }

        private void UIElement_OnTextInput(object sender, TextCompositionEventArgs e)
        {
            var itemsToRemove = _viewModel.VisiaibleComboBoxItems.Where(a => !a.Name.Contains(e.Text));

            if (itemsToRemove.Any())
            {
                _viewModel.VisiaibleComboBoxItems.Remove(itemsToRemove);
            }
        }

        //private void SearchControl_GotFocus_1(object sender, RoutedEventArgs e)
        //{
        //    var autoCompleteBox = sender as RadAutoCompleteBox;
        //    if (!autoCompleteBox.IsDropDownOpen)
        //    {
        //        if (string.IsNullOrEmpty(autoCompleteBox.SearchText))
        //        {
        //            autoCompleteBox.Populate("");
        //        }
        //        else
        //        {
        //            autoCompleteBox.Populate(autoCompleteBox.SearchText);
        //        }
        //    }
        //}
    }

    //public class MyCustomFilteringBehavior : FilteringBehavior
    //{
    //    public override IEnumerable<object> FindMatchingItems(string searchText, IList items, IEnumerable<object> escapedItems, string textSearchPath, TextSearchMode textSearchMode)
    //    {
    //        var result = base.FindMatchingItems(searchText, items, escapedItems, textSearchPath, textSearchMode) as IEnumerable<object>;

    //        if (string.IsNullOrEmpty(searchText) || !result.Any())
    //        {
    //            return ((IEnumerable<object>)items).Where(x => !escapedItems.Contains(x));
    //        }

    //        return result;
    //    }
    //}
}
