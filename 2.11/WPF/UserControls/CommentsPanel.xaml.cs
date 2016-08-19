using Exceedra.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPF.ViewModels.Shared;

namespace WPF.UserControls
{
    /// <summary>
    /// Interaction logic for CommentsPanel.xaml
    /// </summary>
    public partial class CommentsPanel 
    {
        public CommentsPanel()
        { 
            InitializeComponent();
            //CommentTypeFilters.IsEnabled = FilterIsEnabled;
        }

        private void OnCommentTypeComboWithCheckboxesLostFocus(object sender, RoutedEventArgs e)
        {
           FilterNotesByNoteType();
        }

        private void FilterNotesByNoteType()
        {
            if (DgComments != null && DgComments.Items != null)
            {
                SetNoteRowVisibilityByFilterText();
            }
        }

        private void SetNoteRowVisibilityByFilterText()
        {
            if (DgComments == null || DgComments.Items.Count == 0) return;
            foreach (DataGridRow row in DgComments.GetRows())
            {
                var data = row.DataContext as CommentViewModel;
                row.Visibility = NotesDataMatchesFilterText(data) ? Visibility.Visible : Visibility.Collapsed;
            }
            DgComments.ScrollIntoView(DgComments.GetRow(0).DataContext);
        }

        private bool NotesDataMatchesFilterText(CommentViewModel data)
        {
            var selectedItems = CommentTypeFilters.DataSource.SelectedItems;
            if (selectedItems==null || selectedItems.Any()==false)
            {
                return false;
            }
            else
            {
                return selectedItems.Select(s=>s.Name).Contains(data.CommentType, false);
            }
        }

        private bool _filterIsEnabled;
        public bool FilterIsEnabled
        {
            get { return _filterIsEnabled; }
            set
            {
                _filterIsEnabled = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("FilterIsEnabled"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void CommentTypeFilters_Loaded(object sender, RoutedEventArgs e)
        {
            CommentTypeFilters.SelectionChanged += CommentsPanel_OnSelectionChanged;
        }

        private void CommentsPanel_OnSelectionChanged(object sender, EventArgs e)
        {
            FilterNotesByNoteType();
        }

    }
}
