using Model.Entity;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Exceedra.Schedule.Model;
using WPF.UserControls;
using WPF.ViewModels.ScheduleNewFilters;

namespace WPF.Pages
{
    /// <summary>
    /// Interaction logic for Schedule.xaml
    /// </summary>
    public partial class SchedulePageNewFilters
    {
        private const string FilterWatermark = "Filter...";
        public const string SelectAllItem = "0";
        private readonly PromotionTimeLineViewModelV3 _viewModel;

        private PromotionTimelineItem SelectedPromotion { get; set; }

        public SchedulePageNewFilters()
        {
            InitializeComponent();
            FilterCaretBtn.CaretSource = this.rowFilter;

            DataContext = _viewModel = PromotionTimeLineViewModelV3.New();

        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {


            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // string[] FileNames = FileName.Split('.');
            dlg.FileName = "Promotion-Schedule"; // Default file name FileNames[0]

            dlg.DefaultExt = ".xlsx"; // Default file extension

            dlg.Filter = "Excel documents (.xlsx)|*.xlsx"; // Filter files by extension


            // Show save file dialog box
            //Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (dlg.ShowDialog().Value)
            {
                //var d = _viewModel.Appointments;
                //Exceedra.Documents.ExcelOutput.SavePromotions(dlg.FileName, d.ToList());
            }



        }

        private void SetPromoEdit()
        {
            if (promoEdit.Width == new GridLength(0))
            {
                promoEdit.Width = new GridLength(300);
                split1.Visibility = Visibility.Visible;
            }
            //else
            //{
            //    promoEdit.Width = new GridLength(0);
            //}
        }

        public void Border_MouseRightButtonUp(string idx, string type, bool canEdit, string flag)
        {
            // SelectedPromotion = (PromotionTimelineItem)(((Telerik.Windows.Controls.DataItemBase)(((System.Windows.FrameworkElement)(sender)).DataContext)).DataItem); 

            //if (type.ToLower().Contains("promotion"))
            //{
            SetPromoEdit();
            _viewModel.LoadIndividual(idx, type, canEdit, flag);

            editGrid.Focus();
            // }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //SetPromoEdit();
            SelectedPromoID.Text = "";
            _viewModel.EditableGrid = null;
            promoEdit.Width = new GridLength(0);
            split1.Visibility = Visibility.Collapsed;
        }

        //private void Button_Click_2(object sender, RoutedEventArgs e)
        //{
        //    if (DateTime.Parse(CopyPromoStart.Text) < DateTime.Parse(CopyPromoEnd.Text))
        //    {
        //        ////Copy promo with new dates
        //        //var p = new PromotionAccess();
        //        //p.CopyPromotion(SelectedPromoID.Text, CopyPromoStart.Text, CopyPromoEnd.Text);

        //        ////reload timeline
        //        //_viewModel.ApplyFilter(new object());
        //    }
        //    else
        //    {
        //        MessageBox.Show("Promotion copy: start date must be before end date");
        //    }



        //}

        private void CheckBoxClick(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null && (string)checkBox.CommandParameter == SelectAllItem)
            {
                var current = _viewModel.AllStatuses.Where(t => t.Name == checkBox.Tag.ToString()).FirstOrDefault().Statuses;
                if (checkBox.IsChecked.GetValueOrDefault())
                {

                    foreach (var item in current)
                    {
                        item.IsSelected = true;
                    }
                }
                else
                {
                    foreach (var item in current)
                    {
                        item.IsSelected = false;
                    }
                }
            }
            else
            {
                var current = _viewModel.AllStatuses.Where(t => t.Name == checkBox.Tag.ToString()).FirstOrDefault().Statuses;
                var selected = (current.Count() - 1);
                var selected2 = current.Where(t => t.ID != SelectAllItem).Where(r => r.IsSelected == true).Count();

                current.FirstOrDefault().IsSelected = (selected == selected2);


            }
        }


        //private void btnResize_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    FilterCaret.SwapFilterIcon(rowFilter, btnResize);
        //}

        //private void ReSize()
        //{
        //    if (rowFilter.Height == new GridLength(0))
        //    {
        //        rowFilter.Height = new GridLength(250);
        //        var uri = new Uri("/Images/caret/up.png", UriKind.RelativeOrAbsolute);
        //        var bitmap = new BitmapImage(uri);
        //        btnResize.Source = bitmap;
        //    }
        //    else
        //    { 
        //        rowFilter.Height = new GridLength(0);
        //        var uri = new Uri("Images/caret/down.png", UriKind.RelativeOrAbsolute);
        //        var bitmap = new BitmapImage(uri);
        //        btnResize.Source = bitmap;
        //    }
        //}

        //private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    FilterCaret.SwapFilterIcon(rowFilter, btnResize);
        //}

    }


}
