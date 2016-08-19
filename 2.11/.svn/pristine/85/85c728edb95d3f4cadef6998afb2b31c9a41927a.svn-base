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

namespace Coder.WPF.UI
{
    /// <summary>
    /// Interaction logic for ClearableTextBox.xaml
    /// </summary>
    public partial class ClearableTextBox : UserControl
    {
        public ClearableTextBox()
        {
            InitializeComponent();
        }

        #region Text property

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ClearableTextBox), new UIPropertyMetadata("", OnTextPropertyChanged));

        #endregion

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = (ClearableTextBox)d;
            me.HasText = !String.IsNullOrWhiteSpace(me.Text);
        }

        #region HasText
        private static readonly DependencyPropertyKey HasTextPropertyKey
        = DependencyProperty.RegisterReadOnly("HasText", typeof(bool), typeof(ClearableTextBox), new FrameworkPropertyMetadata((bool)false));

        public static readonly DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;

        public bool HasText
        {
            get { return (bool)GetValue(HasTextProperty); }
            protected set { SetValue(HasTextPropertyKey, value); }
        }

        #endregion

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtSearch.Text = String.Empty;
        }


    }
}
