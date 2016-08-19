using Exceedra.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF.ViewModels.Phasing;

namespace WPF.Pages.Phasings
{
    /// <summary>
    /// Interaction logic for PromotionPhasing.xaml
    /// </summary>
    public partial class PromotionPhasing : INotifyPropertyChanged
    {
        public PromotionPhasingViewModel ViewModel { get; set; }

        public PromotionPhasing()
        {
            ViewModel = new PromotionPhasingViewModel();
            PropertyChanged.Raise(this, "ViewModel");

            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
