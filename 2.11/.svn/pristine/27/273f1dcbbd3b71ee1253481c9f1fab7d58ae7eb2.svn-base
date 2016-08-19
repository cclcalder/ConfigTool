namespace WPF.PromoTemplates
{
    using System;
    using System.Windows.Controls;
    using ViewModels;

    /// <summary>
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class TemplateCustomer : Page
    {
        private PromotionTemplateViewModelBase _viewModel;

        public TemplateCustomer(PromotionTemplateViewModelBase viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            InitializeComponent();
            DataContext = _viewModel = viewModel;

        }

        private bool _isFiltered;

        public bool isFiltered
        {
            get { return _isFiltered; }
            set { _isFiltered = value; }
        }

        private string m_filterText;

        public string filterText
        {
            get { return m_filterText; }
            set { m_filterText = value; }
        }




        private bool _isFiltered3;

        public bool isFiltered3
        {
            get { return _isFiltered3; }
            set { _isFiltered3 = value; }
        }

        private string m_filterText3;

        public string filterText3
        {
            get { return m_filterText3; }
            set { m_filterText3 = value; }
        }


    }

}