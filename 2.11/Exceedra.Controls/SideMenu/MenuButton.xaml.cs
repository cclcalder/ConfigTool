//The MIT License(MIT)

//Copyright(c) 2015 Alberto Rodriguez

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Exceedra.Common.Utilities;
using System.Windows.Data;
using System.Globalization;
using System.Linq;

namespace Exceedra.SideMenu
{
    /// <summary>
    /// Interaction logic for MenuButton.xaml
    /// </summary>
    public partial class MenuButton : UserControl, INotifyPropertyChanged
    {
        public bool AreChildrenVisible { get; set; }
        public Brush originalBackgound;

        public MenuButton()
        {
            InitializeComponent();
            Children = new List<MenuButton>();
            Theme = UiTheme.Light;
            AnimationSpeed = TimeSpan.FromMilliseconds(150);

        }

        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register(
        "HoverBackground",
        typeof(SolidColorBrush),
        typeof(MenuButton));

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
        "Image",
        typeof(string),
        typeof(MenuButton));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        "Text",
        typeof(string),
        typeof(MenuButton),
        new FrameworkPropertyMetadata()
        {
            DefaultValue = "menu",
        });

        public static readonly DependencyProperty AbbrProperty = DependencyProperty.Register(
          "Abbr",
          typeof(string),
          typeof(MenuButton),
          new FrameworkPropertyMetadata()
          {
              DefaultValue = "",
          });

        public static readonly DependencyProperty AnimationSpeedProperty = DependencyProperty.Register(
        "AnimationSpeed",
        typeof(TimeSpan),
        typeof(MenuButton));

        public static readonly DependencyProperty UiThemeProperty = DependencyProperty.Register(
        "UiTheme",
        typeof(UiTheme),
        typeof(MenuButton));

        public static readonly DependencyProperty ChildrenProperty = DependencyProperty.Register(
        "Children",
        typeof(List<MenuButton>),
        typeof(MenuButton));

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected",
            typeof(bool),
            typeof(MenuButton),
                    new FrameworkPropertyMetadata()
                    {
                        PropertyChangedCallback = OnDataChanged
                    });

        public static readonly DependencyProperty IsChildProperty = DependencyProperty.Register("IsChild",
            typeof(bool),
            typeof(MenuButton),
                                new FrameworkPropertyMetadata()
                                {
                                    PropertyChangedCallback = OnDataChanged
                                });


        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register("ShowIcon",
            typeof(bool),
            typeof(MenuButton),
                new FrameworkPropertyMetadata()
                {
                    DefaultValue = true
                });


        public MenuButton ParentButton { get; private set; }

        public TimeSpan AnimationSpeed
        {
            get { return (TimeSpan)GetValue(AnimationSpeedProperty); }
            set { SetValue(AnimationSpeedProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        private bool _isTooltipVisible;
        public bool IsTooltipVisible
        {
            get { return _isTooltipVisible; }
            set
            {
                _isTooltipVisible = value;

                if (ChildStack.Children != null)
                {
                    for (int i = 0; i < ChildStack.Children.Count; i++)
                    {
                        var childButton = ChildStack.Children[i] as MenuButton;
                        if (childButton != null) childButton.IsTooltipVisible = value;
                    }
                }

                PropertyChanged.Raise(this, "Tooltip");
            }
        }

        private string _tooltip;
        public string Tooltip
        {
            get
            {
                if (IsTooltipVisible)
                    return _tooltip;

                return null;
            }
            set { _tooltip = value; }
        }

        public string Abbr
        {
            get { return (string)GetValue(AbbrProperty); }
            set
            {
                SetValue(AbbrProperty, value);
            }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public bool ShowIcon
        {
            get { return (bool)GetValue(ShowIconProperty); }
            set { SetValue(ShowIconProperty, value); }
        }

        public bool IsChild
        {
            get { return (bool)GetValue(IsChildProperty); }
            set { SetValue(IsChildProperty, value); }
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MenuButton)d).OnTrackerInstanceChanged(e);
        }

        protected virtual void OnTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            PropertyChanged.Raise(this, "IconColour");
        }

        public SolidColorBrush HoverBackground
        {
            get { return (SolidColorBrush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }

        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public SolidColorBrush IconColour
        {
            get { return IsSelected ? new SolidColorBrush(Colors.Black) : IsChild ? (SolidColorBrush)new BrushConverter().ConvertFrom("#75dbd7") : new SolidColorBrush(Colors.White); }
        }

        public UiTheme Theme
        {
            get { return (UiTheme)GetValue(UiThemeProperty); }
            set
            {
                SetValue(UiThemeProperty, value);
                BitmapImage bm;
                switch (Theme)
                {
                    case UiTheme.Light:
                        bm = new BitmapImage(new Uri(@"Images/Light24.png", UriKind.Relative));
                        break;
                    case UiTheme.Dark:
                        bm = new BitmapImage(new Uri(@"Images/Dark24.png", UriKind.Relative));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                //Chevron.Source = bm;

                Txt.Foreground = Theme == UiTheme.Light
                    ? Brushes.WhiteSmoke
                    : new SolidColorBrush { Color = Color.FromRgb(30, 30, 30) };
            }
        }

        public List<MenuButton> Children
        {
            get { return (List<MenuButton>)GetValue(ChildrenProperty); }
            set
            {
                SetValue(ChildrenProperty, value);
                foreach (var child in Children)
                {
                    child.ParentButton = this;
                }
                DataContext = Children;
                Chevron.Visibility = Children.Any() ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private double ExpandedHeight
        {
            get { return Children.Count * ActualHeight + 5; }
        }

        private void Expand(double units)
        {
            var c = FindName("ChildStack") as StackPanel;
            var animation = new DoubleAnimation
            {
                From = 0,
                To = c.ActualHeight,
                Duration = AnimationSpeed
            };
            c.BeginAnimation(HeightProperty, animation);
        }

        private void Reduce(double units)
        {
            var c = FindName("ChildStack") as StackPanel;
            var animation = new DoubleAnimation
            {
                From = c.ActualHeight,
                To = c.ActualHeight - units,
                Duration = AnimationSpeed
            };
            c.BeginAnimation(HeightProperty, animation);
        }

        private void Open()
        {
            Expand(ExpandedHeight);
            AreChildrenVisible = true;
            Chevron.Icon = FontAwesome.WPF.FontAwesomeIcon.MinusCircle;
        }

        private void Close()
        {
            foreach (var child in Children) child.Close();
            if (!AreChildrenVisible) return;
            var c = FindName("ChildStack") as StackPanel;
            var animation = new DoubleAnimation
            {
                From = c.ActualHeight,
                To = 0,
                Duration = AnimationSpeed
            };
            c.BeginAnimation(HeightProperty, animation);
            AreChildrenVisible = false;
            Chevron.Icon = FontAwesome.WPF.FontAwesomeIcon.PlusCircle;
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenNavigate();
        }

        private void OpenNavigate()
        {
            if (Children.Count == 0)
            {
                CommandHandler.Execute(CommandHandlerParameter);
                return;
            }

            if (AreChildrenVisible)
            {
                var parent = ParentButton;
                while (parent != null)
                {
                    var c = FindName("ChildStack") as StackPanel;
                    parent.Reduce(c.ActualHeight);
                    parent = parent.ParentButton;
                }
                Close();
            }
            else
            {
                var parent = ParentButton;
                while (parent != null)
                {
                    parent.Expand(ExpandedHeight);
                    parent = parent.ParentButton;
                }
                Open();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Tooltip = Text;
            IsTooltipVisible = true;
            Abr.Text = Abbr;
            //SmallSizeButton.ToolTip = Text;

            // Img.Source = Image;

            foreach (var c in Children)
            {
                c.IsChild = true;
                ChildStack.Children.Add(c);
            }
            //ChildStack.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();

            foreach (var child in Children)
            {
                child.ParentButton = this;
            }

            Chevron.Visibility = Children.Any() ? Visibility.Visible : Visibility.Hidden;
        }

        //protected override void OnInitialized(EventArgs e)
        //{
        //    InitializeComponent();
        //    base.OnInitialized(e);
        //}


        public static readonly DependencyProperty CommandHandlerParameterProperty =
               DependencyProperty.Register(
               "CommandHandlerParameter",
               typeof(string),
               typeof(MenuButton));

        public string CommandHandlerParameter
        {
            get { return (string)GetValue(CommandHandlerParameterProperty); }
            set
            {
                if (value != null)
                {
                    SetValue(CommandHandlerParameterProperty, value);
                }
            }
        }

        public static readonly DependencyProperty CommandHandlerProperty =
           DependencyProperty.Register("CommandHandler", typeof(ICommand),
           typeof(MenuButton),
           null);

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CommandHandler
        {
            get { return (ICommand)GetValue(CommandHandlerProperty); }
            set
            {
                if (value != null)
                {
                    SetValue(CommandHandlerProperty, value);
                }
            }
        }

        public static readonly DependencyProperty ExpandMainMenuProperty =
            DependencyProperty.Register("ExpandMainMenu", typeof(Action),
                typeof(MenuButton),
                null);


        public Action ExpandMainMenu
        {
            get { return (Action)GetValue(ExpandMainMenuProperty); }
            set
            {
                if (value != null)
                {
                    SetValue(ExpandMainMenuProperty, value);
                }
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            OpenNavigate();
        }
    }


}