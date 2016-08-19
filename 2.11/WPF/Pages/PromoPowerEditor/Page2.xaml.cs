using System;
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
using WPF.ViewModels.PromotionPowerEditor;

namespace WPF.Pages.PromoPowerEditor
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        public Page2() : this(null)
        { }

        public Page2(PromotionPowerEditorViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
