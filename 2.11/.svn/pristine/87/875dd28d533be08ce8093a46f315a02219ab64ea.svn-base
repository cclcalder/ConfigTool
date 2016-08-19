using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Model.Annotations;
using WPF.ViewModels.Demand;
using Exceedra.Common.Utilities;
using System;
using Model.Entity.Diagnostics;
using System.Windows.Media.Imaging;

namespace WPF.Pages.Demand
{
    /// <summary>
    /// Interaction logic for DPMain.xaml
    /// </summary>
    public partial class DPMain : INotifyPropertyChanged
    {
        public DPMainViewModel ViewModel { get; set; }

        public DPMain()
        {
            InitializeComponent();

            IsOpen = true;
            ViewModel = new DPMainViewModel();
            PropertyChanged.Raise(this, "ViewModel");

            var widthDescriptor = DependencyPropertyDescriptor.FromProperty(ColumnDefinition.WidthProperty, typeof(ItemsControl));
            widthDescriptor.AddValueChanged(ModelDataColumn, HeightChanged);

            IsOpen = true;
        }


        private void HeightChanged(object sender, EventArgs e)
        {
            var shouldOpen = ModelDataColumn.Width.Value > 50;

            if (IsOpen != shouldOpen)
            {
                IsOpen = shouldOpen;
            }
        }


        private GridLength _formerHeight;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isOpen;
        private bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                if (_isOpen == value) return;

                _isOpen = value;

                ModelDataResizeImg.Icon = IsOpen ? FontAwesome.WPF.FontAwesomeIcon.ArrowCircleRight : FontAwesome.WPF.FontAwesomeIcon.ArrowCircleLeft;
            }
        }

        private void btnResizeModel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IsOpen = !IsOpen;

            if (IsOpen)
                ModelDataColumn.Width = new GridLength(350);
            else
                ModelDataColumn.Width = new GridLength(0);
        }
    }


    public class ResizeObject
    {
        public ColumnDefinition Column { get; set; }
        public GridLength? FormerWidth { get; set; }
    }
}
