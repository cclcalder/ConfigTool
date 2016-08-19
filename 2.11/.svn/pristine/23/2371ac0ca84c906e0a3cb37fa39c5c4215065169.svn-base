using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Exceedra.TreeGrid.Controls
{
    /// <summary>
    /// Interaction logic for TextBlockWithIcon.xaml
    /// </summary>
    public partial class TextBlockWithIcon : UserControl
    {
        public TextBlockWithIcon()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty InnerIconProperty = DependencyProperty.Register("InnerIcon", typeof(int), typeof(TextBlockWithIcon),
            new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true, PropertyChangedCallback = OnIconChanged });

        public int InnerIcon
        {
            get { return (int)GetValue(InnerIconProperty); }
            set { SetValue(InnerIconProperty, value); }
        }

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextBlockWithIcon)d).OnIconInstanceChanged(e);
        }

        protected virtual void OnIconInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            Icon.Icon = (FontAwesome.WPF.FontAwesomeIcon)(int)e.NewValue;            
        }
        //NavigationService.RemoveBackEntry


        public static readonly DependencyProperty InnerTextBlockProperty = DependencyProperty.Register("InnerTextBlock", typeof(TextBlock), typeof(TextBlockWithIcon),
            new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true, PropertyChangedCallback = OnDataChanged });

        public TextBlock InnerTextBlock
        {
            get { return (TextBlock)GetValue(InnerTextBlockProperty); }
            set { SetValue(InnerTextBlockProperty, value); }
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextBlockWithIcon)d).OnTrackerInstanceChanged(e);
        }

        protected virtual void OnTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((TextBlock)e.NewValue != null)
            {
                var block = (TextBlock)e.NewValue;
                InnerText.Text = block.Text;
                InnerText.TextAlignment = block.TextAlignment;
                InnerText.TextWrapping = block.TextWrapping;
                InnerText.FontSize = block.FontSize;
                InnerText.Tag = block.Tag;
                InnerText.FontWeight = block.FontWeight;
                InnerText.Foreground = block.Foreground;
            }
        }

        public void SwitchIcon()
        {
            InnerIcon = InnerIcon == (int)FontAwesome.WPF.FontAwesomeIcon.Plus ? (int)FontAwesome.WPF.FontAwesomeIcon.Minus : (int)FontAwesome.WPF.FontAwesomeIcon.Plus;
            Icon.Foreground = InnerIcon == (int)FontAwesome.WPF.FontAwesomeIcon.Plus ? new BrushConverter().ConvertFromString("#5cb85c") as Brush : new BrushConverter().ConvertFromString("#d9534f") as Brush;
        }

    }
}
