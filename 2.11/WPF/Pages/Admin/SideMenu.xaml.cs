using System;
 
using System.Windows;
using System.Windows.Controls;
 
using WPF.ViewModels.Admin;
using System.ComponentModel;
 
using Coder.WPF.UI;

namespace WPF.Pages.Admin
{
    /// <summary>
    /// Interaction logic for SideMenu.xaml
    /// </summary>
    public partial class SideMenu 
    {
        public MenuGroupViewModel thisMenuGroupViewModel
        {
            get;
            set;
        }

        private MenuGroupViewModel m_thisMenuGroupViewModel;

        public ISearchableTreeViewNodeEventsConsumer rightTreeForVM;

        public SideMenu()
        {
            InitializeComponent();
            DataContext = thisMenuGroupViewModel = new MenuGroupViewModel();
            thisMenuGroupViewModel.PropertyChanged += thisMenuGroupViewModel_PropertyChanged;
           // DataContext = this;
           thisMenuGroupViewModel.NewPattern2ListVM(Pattern2ControlForEventConsumer as ISearchableTreeViewNodeEventsConsumer);
           thisMenuGroupViewModel.NewPattern1List(Pattern1Ctrl as ISearchableTreeViewNodeEventsConsumer);
           Loaded += Page_Loaded;
           mainHolder.Visibility = System.Windows.Visibility.Hidden;
        }

        public ISearchableTreeViewNodeEventsConsumer leftTree(ISearchableTreeViewNodeEventsConsumer thisTree)
        {
            return thisTree;
        }

        private void thisMenuGroupViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Contains("Pattern2"))
            {
                Pattern2ControlForEventConsumer.DataSource = thisMenuGroupViewModel.Pattern2VM;
                mainHolder.Visibility = System.Windows.Visibility.Visible;

            }
            if (e.PropertyName == "Pattern1VM")
            {
                try
                {
                    Pattern1Ctrl.DataSource = thisMenuGroupViewModel.Pattern1VM;
                }
                catch (Exception ex)
                {
                    var r = ex;
                }


                
                mainHolder.Visibility = System.Windows.Visibility.Visible;
            }
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
          
        }
  
        private void Send_Click_To_VM(object sender, RoutedEventArgs e)
        {
            //Pattern2ViewModel thisVM = new Pattern2ViewModel();
            //thisMenuGroupViewModel.SelectionClick(sender, e);
            //thisVM.SelectionClick(e);
        }
     
    }
}
