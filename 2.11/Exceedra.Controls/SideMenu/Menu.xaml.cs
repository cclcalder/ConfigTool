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

using Model.Entity;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Telerik.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Data;
using System.Linq;

namespace Exceedra.SideMenu
{
    public partial class Menu
    {
        private bool _isShown;

        public Menu()
        {
            InitializeComponent();
            //Theme = SideMenuTheme.Default;
            ClosingType = ClosingType.Auto;
        }

        #region "Dependencies"
        
        public static readonly DependencyProperty EnableNavigationProperty = DependencyProperty.Register(
            "EnableNavigation",
            typeof(bool),
            typeof(Menu));

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
        "State",
        typeof(MenuState),
        typeof(Menu));

        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register(
        "Theme",
        typeof(SideMenuTheme),
        typeof(Menu));

        public static readonly DependencyProperty MenuWidthProperty = DependencyProperty.Register(
        "MenuWidth",
        typeof(double),
        typeof(Menu));

        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register(
        "Menu",
        typeof(ScrollViewer),
        typeof(Menu));

        public static readonly DependencyProperty ShadowBackgroundProperty = DependencyProperty.Register(
        "ShadowBackground",
        typeof(Brush),
        typeof(Menu));

        public static readonly DependencyProperty ButtonBackgroundProperty = DependencyProperty.Register(
        "ButtonBackground",
        typeof(Brush),
        typeof(Menu));

        public static readonly DependencyProperty ButtonHoverProperty = DependencyProperty.Register(
        "ButtonHover",
        typeof(Brush),
        typeof(Menu));

        public static readonly DependencyProperty ShowMenuIconProperty = DependencyProperty.Register("ShowMenuIcon",
           typeof(bool),
           typeof(Menu),
               new FrameworkPropertyMetadata()
               {
                   DefaultValue = true
               });

        public bool ShowMenuIcon
        {
            get { return (bool)GetValue(ShowMenuIconProperty); }
            set { SetValue(ShowMenuIconProperty, value); }
        }


        public static readonly DependencyProperty DisplayNameProperty =
    DependencyProperty.Register("DisplayName", typeof(string),
        typeof(Menu),
        null);

        public static readonly DependencyProperty DisplayInitialsProperty =
            DependencyProperty.Register("DisplayInitials", typeof(string),
                typeof(Menu),
                null);
        
        public static readonly DependencyProperty NavigateCommandProperty =
            DependencyProperty.Register("NavigateCommand", typeof(ICommand),
                typeof(Menu),
                null);

        public static readonly DependencyProperty LogoutCommandProperty =
            DependencyProperty.Register("LogoutCommand", typeof(ICommand),
                typeof(Menu),
                null);

        public static readonly DependencyProperty MenuItemsProperty =
            DependencyProperty.Register("MenuItems", typeof(ObservableCollection<Screen>),
                typeof(Menu),
                new FrameworkPropertyMetadata()
                {
                    PropertyChangedCallback = OnDataChanged,
                });

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Menu)d).OnTrackerInstanceChanged(e);
        }

        protected virtual void OnTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            ScreenStack.Children.Clear();
            foreach (var s in ((ObservableCollection<Screen>)e.NewValue))
            {                
                var button = ConvertToMenuItem(s);
                ScreenStack.Children.Add(button);
            }
        
        }

        private List<MenuButton> GetChildButtons(List<Screen> children)
        {
            var childButtons = new List<MenuButton>();
            if (children == null) return childButtons;
            foreach (var s in children)
            {
                var child = ConvertToMenuItem(s);
                child.IsChild = true;
                childButtons.Add(child);
            }
            return childButtons;
        }

        private MenuButton ConvertToMenuItem(Screen s)
        {
            var bind = new Binding { Source = this, Path = new PropertyPath("EnableNavigation") };
            var button = new MenuButton { Text = s.Label, Abbr = s.Abbreviation , Image = s.Icon, CommandHandlerParameter = s.Uri, CommandHandler = NavigateCommand, Children = GetChildButtons(s.Children), ExpandMainMenu = Show, ShowIcon = ShowMenuIcon };
            button.SetBinding(MenuButton.IsEnabledProperty, bind);
            return button;
        }

       

        #endregion

        public ICommand NavigateCommand
        {
            get { return (ICommand)GetValue(NavigateCommandProperty); }
            set { SetValue(NavigateCommandProperty, value); }
        }
        public ICommand LogoutCommand
        {
            get { return (ICommand)GetValue(LogoutCommandProperty); }
            set { SetValue(LogoutCommandProperty, value); }
        }
        
        public ObservableCollection<Screen> MenuItems
        {
            get { return (ObservableCollection<Screen>)GetValue(MenuItemsProperty); }
            set { SetValue(MenuItemsProperty, value); }
        }

        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        public string DisplayInitials
        {
            get { return (string)GetValue(DisplayInitialsProperty); }
            set { SetValue(DisplayInitialsProperty, value); }
        }

        public bool EnableNavigation
        {
            get { return (bool)GetValue(EnableNavigationProperty); }
            set { SetValue(EnableNavigationProperty, value); }
        }
        

        public ClosingType ClosingType { get; set; }

        public Brush ButtonBackground
        {
            get { return (Brush)GetValue(ButtonBackgroundProperty); }
            set
            {
                SetValue(ButtonBackgroundProperty, value);
                Resources["ButtonBackground"] = value;
            }
        }

        public Brush ButtonHover
        {
            get { return (Brush)GetValue(ButtonHoverProperty); }
            set
            {
                SetValue(ButtonHoverProperty, value);
                Resources["ButtonHover"] = value;
            }
        }

        public Brush ShadowBackground
        {
            get { return (Brush)GetValue(ShadowBackgroundProperty); }
            set
            {
                SetValue(ShadowBackgroundProperty, value);
                //Resources["Shadow"] = value ?? new SolidColorBrush { Color = Colors.Black, Opacity = .2 };
            }
        }

        public double MenuWidth
        {
            get { return (double)GetValue(MenuWidthProperty); }
            set
            {
                SetValue(MenuWidthProperty, value);
            }
        }

        public MenuState State
        {
            get { return (MenuState)GetValue(StateProperty); }
            set
            {
                SetValue(StateProperty, value);
                if (value == MenuState.Visible)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            }
        }

        //public SideMenuTheme Theme
        //{
        //    get { return (SideMenuTheme)GetValue(ThemeProperty); }
        //    set
        //    {
        //        if (value == SideMenuTheme.None) return;
        //        SetValue(ThemeProperty, value);
        //        SolidColorBrush buttonBackground;
        //        SolidColorBrush buttonHoverBackground;
        //        SolidColorBrush background;
        //        switch (value)
        //        {
        //            case SideMenuTheme.Default:
        //                background = new SolidColorBrush { Color = Color.FromArgb(205, 20, 20, 20) };
        //                buttonBackground = new SolidColorBrush { Color = Color.FromArgb(50, 30, 30, 30) };
        //                buttonHoverBackground = new SolidColorBrush { Color = Color.FromArgb(50, 70, 70, 70) };
        //                break;
        //            case SideMenuTheme.Primary:
        //                background = new SolidColorBrush { Color = Color.FromArgb(205, 24, 57, 85) };
        //                buttonBackground = new SolidColorBrush { Color = Color.FromArgb(50, 35, 85, 126) };
        //                buttonHoverBackground = new SolidColorBrush { Color = Color.FromArgb(50, 45, 110, 163) };
        //                break;
        //            case SideMenuTheme.Success:
        //                background = new SolidColorBrush { Color = Color.FromArgb(205, 55, 109, 55) };
        //                buttonBackground = new SolidColorBrush { Color = Color.FromArgb(50, 65, 129, 65) };
        //                buttonHoverBackground = new SolidColorBrush { Color = Color.FromArgb(50, 87, 172, 87) };
        //                break;
        //            case SideMenuTheme.Warning:
        //                background = new SolidColorBrush { Color = Color.FromArgb(205, 150, 108, 49) };
        //                buttonBackground = new SolidColorBrush { Color = Color.FromArgb(50, 179, 129, 58) };
        //                buttonHoverBackground = new SolidColorBrush { Color = Color.FromArgb(50, 216, 155, 70) };
        //                break;
        //            case SideMenuTheme.Danger:
        //                background = new SolidColorBrush { Color = Color.FromArgb(205, 135, 52, 49) };
        //                buttonBackground = new SolidColorBrush { Color = Color.FromArgb(50, 179, 69, 65) };
        //                buttonHoverBackground = new SolidColorBrush { Color = Color.FromArgb(50, 238, 92, 86) };
        //                break;
        //            default:
        //                throw new ArgumentOutOfRangeException("SideMenuTheme", value, null);
        //        }
        //        Resources["ButtonHover"] = buttonBackground;
        //        Resources["ButtonBackground"] = buttonHoverBackground;
        //        if (MenuScroller != null) MenuScroller.Background = background;
        //    }
        //}

        public void AssertButtonSelection(string uri)
        {
            foreach (var s in ScreenStack.ChildrenOfType<MenuButton>())
            {
                s.IsSelected = s.CommandHandlerParameter == uri;
            }
        }

        public void Toggle()
        {
            if (_isShown)
            { 
                State = MenuState.Hidden;
            }
            else
            { 
                State = MenuState.Visible;
            }
        }

        public void Show()
        {
            ExpandButton.Visibility = Visibility.Collapsed;
            
            var animation = new DoubleAnimation
            {
                From = 50,
                To = 260,
                Duration = TimeSpan.FromMilliseconds(100)
            };
            MenuGrid.BeginAnimation(WidthProperty, animation);

            _isShown = true;
            var p = Parent as Panel;
            ShadowColumn.Width = new GridLength(10000);

            ShowTooltips(false);
        }

        public void Hide()
        {
            //Removing this animation causes issues but I do not know why :(
            var animation = new DoubleAnimation
            {
                To = 0,//-MenuWidth
                Duration = TimeSpan.FromMilliseconds(100)
            };
            RenderTransform.BeginAnimation(TranslateTransform.XProperty, animation);

            var animation2 = new DoubleAnimation
            {
                From = 260,
                To = 50,
                Duration = TimeSpan.FromMilliseconds(100),                                           
            };
            animation2.Completed += (s, e) => { ExpandButton.Visibility = Visibility.Visible; };
            MenuGrid.BeginAnimation(WidthProperty, animation2);

            _isShown = false;
            ShadowColumn.Width = new GridLength(0);

            ShowTooltips(true);
        }

        private void ShowTooltips(bool showTooltips)
        {
            for (int i = 0; i < ScreenStack.Children.Count; i++)
            {
                var menuButton = ScreenStack.Children[i] as MenuButton;
                if (menuButton != null) menuButton.IsTooltipVisible = showTooltips;
            }
        }

        public override void OnApplyTemplate()
        {
            Panel.SetZIndex(this, int.MaxValue);
            RenderTransform = new TranslateTransform(-MenuWidth, 0);
            //MenuColumn.Width = new GridLength(MenuWidth);

            //this is a little hack to fire propertu changes.
            //wpf so complex, it could be much simple...
            State = State;
            //Theme = Theme;
            ShadowBackground = ShadowBackground;
            ButtonBackground = ButtonBackground;
            ButtonHover = ButtonHover;
        }

        private void ShadowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ClosingType == ClosingType.Auto)
            {         
                State = MenuState.Hidden;
            }
        }

        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            if (LogoutCommand != null)
                LogoutCommand.Execute(null);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Toggle();
        }
    }
}