using System.Windows;
using Exceedra.Controls.DynamicRow.Controls;
using Exceedra.Controls.DynamicRow.Models;

namespace WPF.Wizard
{
    using System;
    using System.Windows.Controls;
    using ViewModels;

    /// <summary>
    /// Interaction logic for Attributes.xaml
    /// </summary>
    public partial class Attributes : Page
    {
        public Attributes()
            : this(null)
        {
        }

        public Attributes(PromotionWizardViewModelBase viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            InitializeComponent();
            DataContext = viewModel;



        }
    }
}