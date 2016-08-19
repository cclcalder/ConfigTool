using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Exceedra.Common.Utilities;
using Model.Annotations;
using Model.Entity;
using Model;
using System.Windows.Media.Imaging;

namespace Exceedra.Controls.Caret
{
    /// <summary>
    /// Interaction logic for FilterCaretBtn.xaml
    /// </summary>
    public partial class FilterCaretBtn : INotifyPropertyChanged
    {
        public FilterCaretBtn()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CaretSourceProperty =
             DependencyProperty.Register("CaretSource",
             typeof(RowDefinition),
             typeof(FilterCaretBtn),
             new FrameworkPropertyMetadata { PropertyChangedCallback = OnDataChanged, BindsTwoWayByDefault = false }
         );

        public RowDefinition CaretSource
        {
            get { return (RowDefinition)GetValue(CaretSourceProperty); }
            set { SetValue(CaretSourceProperty, value); }
        }

        public static readonly DependencyProperty CloseSizeProperty =
            DependencyProperty.Register("CloseSize",
                typeof(int),
                typeof(FilterCaretBtn),
                new UIPropertyMetadata(0)
                );

        public static readonly DependencyProperty OpenSizeProperty =
            DependencyProperty.Register("OpenSize",
                typeof(int),
                typeof(FilterCaretBtn),
                new UIPropertyMetadata(400)
                );
        
        public static readonly DependencyProperty OpenByDefaultProperty =
            DependencyProperty.Register("OpenByDefault",
                typeof(bool?),
                typeof(FilterCaretBtn),
                new FrameworkPropertyMetadata(null)
                {
                    PropertyChangedCallback = OnDataChanged2
                }
                );

        public bool InverseIcons
        {
            get { return (bool)GetValue(InverseIconsProperty); }
            set { SetValue(InverseIconsProperty, value); }
        }

        public static readonly DependencyProperty InverseIconsProperty =
            DependencyProperty.Register("InverseIcons",
                typeof(bool),
                typeof(FilterCaretBtn),
                new UIPropertyMetadata(false)
                );

        public string OpenText
        {
            get { return (string)GetValue(OpenTextProperty); }
            set { SetValue(OpenTextProperty, value); }
        }

        public static readonly DependencyProperty OpenTextProperty =
            DependencyProperty.Register("OpenText",
                typeof(string),
                typeof(FilterCaretBtn),
                new UIPropertyMetadata(User.CurrentUser.CurrentLanguage.GetValue("Label_Filters_Hide", "Hide filters"))
                );

        public string ClosedText
        {
            get { return (string)GetValue(ClosedTextProperty); }
            set { SetValue(ClosedTextProperty, value); }
        }

        public static readonly DependencyProperty ClosedTextProperty =
            DependencyProperty.Register("ClosedText",
                typeof(string),
                typeof(FilterCaretBtn),
                new UIPropertyMetadata(User.CurrentUser.CurrentLanguage.GetValue("Label_Filters_Show", "Show filters"))
                );

        public int CloseSize
        {
            get { return (int)GetValue(CloseSizeProperty); }
            set { SetValue(CloseSizeProperty, value); }
        }

        public int OpenSize
        {
            get { return (int)GetValue(OpenSizeProperty); }
            set { SetValue(OpenSizeProperty, value); }
        }

        /* Overpowers the config */
        public bool? OpenByDefault
        {
            get { return (bool?)GetValue(OpenByDefaultProperty); }
            set { SetValue(OpenByDefaultProperty, value); }
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FilterCaretBtn)d).OnTrackerInstanceChanged(e);
        }

        protected virtual void OnTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((RowDefinition)e.NewValue != null)
            {
                DataContext = (RowDefinition)e.NewValue;

                var heightDescriptor = DependencyPropertyDescriptor.FromProperty(RowDefinition.HeightProperty, typeof(ItemsControl));
                heightDescriptor.AddValueChanged((RowDefinition)e.NewValue, HeightChanged);

                if (OpenByDefault != null)
                    IsOpen = OpenByDefault == true;
                else
                    IsOpen = !ClientConfiguration.IsFilterClosedByDefault;
            }
        }

        private static void OnDataChanged2(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FilterCaretBtn)d).OnTrackerInstanceChanged2(e);
        }

        protected virtual void OnTrackerInstanceChanged2(DependencyPropertyChangedEventArgs e)
        {
            if (CaretSource != null)
                IsOpen = OpenByDefault == true;                        
        }

        private void HeightChanged(object sender, EventArgs e)
        {
            var shouldOpen = CaretSource.Height.Value > 50;

            if (IsOpen != shouldOpen)
            {
                IsOpen = shouldOpen;
            }
        }

        private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            
                IsOpen = !IsOpen;
        }

        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                _isOpen = value;
                FilterCaret.SetFilterIconAndSize(IsOpen, CaretSource, CloseSize, OpenSize);
                lblResize.Content = IsOpen ? OpenText : ClosedText;
                btnResize.Source = IsOpen ? InverseIcons ? FilterCaret.GetDownImage() : FilterCaret.GetUpImage() : InverseIcons ? FilterCaret.GetUpImage() : FilterCaret.GetDownImage();
                PropertyChanged.Raise(this, "IsOpen");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
