using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Exceedra.Test
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class EditableTextBlock
    {
        #region Constructor

        public EditableTextBlock()
        {
            InitializeComponent();
            base.Focusable = true;
            base.FocusVisualStyle = null;
        }

        #endregion Constructor

        #region Member Variables

        // We keep the old text when we go into editmode
        // in case the user aborts with the escape key
        private string oldText;

        #endregion Member Variables

        #region Properties

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(EditableTextBlock),
            new PropertyMetadata(""));

        public string CellBackground
        {
            get { return (string)GetValue(CellBackgroundProperty); }
            set { SetValue(CellBackgroundProperty, value); }
        }

        public static readonly DependencyProperty CellBackgroundProperty =
            DependencyProperty.Register(
            "CellBackground",
            typeof(string),
            typeof(EditableTextBlock),
            new PropertyMetadata("#00FFFFFF"));

        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register(
            "IsEditable",
            typeof(bool),
            typeof(EditableTextBlock),
            new PropertyMetadata(false));

        public bool IsInEditMode
        {
            get
            {
                if (IsEditable)
                    return (bool)GetValue(IsInEditModeProperty);
                else
                    return false;
            }
            set
            {
                if (IsEditable && IsInEditMode != value)
                {
                    if (value) oldText = Text;
                    SetValue(IsInEditModeProperty, value);
                }
            }
        }

        public static readonly DependencyProperty IsInEditModeProperty =
            DependencyProperty.Register(
            "IsInEditMode",
            typeof(bool),
            typeof(EditableTextBlock),
            new PropertyMetadata(false));

        public HorizontalAlignment Alignment
        {
            get { return (HorizontalAlignment)GetValue(AlignmentProperty); }
            set { SetValue(AlignmentProperty, value); }
        }
        public static readonly DependencyProperty AlignmentProperty =
            DependencyProperty.Register(
            "Alignment",
            typeof(HorizontalAlignment),
            typeof(EditableTextBlock),
            new PropertyMetadata(HorizontalAlignment.Left));

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register(
            "TextAlignment",
            typeof(TextAlignment),
            typeof(EditableTextBlock),
            new PropertyMetadata(TextAlignment.Left));

        #endregion Properties

        #region Event Handlers

        // Invoked when we enter edit mode.
        void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox txt = sender as TextBox;

            // Give the TextBox input focus
            txt.Focus();

            txt.SelectAll();
        }

        // Invoked when we exit edit mode.
        void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {            
            this.IsInEditMode = false;
        }

        // Invoked when the user edits the annotation.
        void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //this.IsInEditMode = false;
                //e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                this.IsInEditMode = false;
                Text = oldText;
                e.Handled = true;
            }
        }


        private ICommand _clickCommand;
        public ICommand ClickCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler(ToEditMode));
            }
        }

        public void ToEditMode(object o)
        {
            IsInEditMode = true;
            txtBox.Focus();
        }


        #endregion Event Handlers

        private void ToEditMode(object sender, RoutedEventArgs e)
        {
            ToEditMode(sender);
        }
    }

    public class CommandHandler : ICommand
    {
        #region Member Fields

        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;
        private readonly Window _windowOverride;

        #endregion

        #region Constructors

        public CommandHandler(Action<object> execute)
            : this(null, execute)
        {
        }

        public CommandHandler(Predicate<object> canExecute, Action<object> execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = execute;
            _canExecute = canExecute;
        }

        public CommandHandler(Action<object> execute, Window w)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = execute;
            _windowOverride = w;
        }

        #endregion // Constructors

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(_windowOverride ?? parameter);
        }

        #endregion

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

    }
}
