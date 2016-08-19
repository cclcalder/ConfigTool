using System.Windows;
using Exceedra.Common.Utilities;

namespace ViewHelper
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows.Input;

    public class ViewCommand : ICommand
    {
        #region Member Fields

        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;
        private readonly Window _windowOverride;

        #endregion

        #region Constructors

        public ViewCommand(Action<object> execute)
            : this(null, execute)
        {
        }

        public ViewCommand(Predicate<object> canExecute, Action<object> execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = execute;
            _canExecute = canExecute;
        }

        public ViewCommand(Action<object> execute, Window w)
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

        [DebuggerStepThrough]
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

    public class ActionCommand<T> : ICommand
    {
        private readonly Func<T, bool> _canExecute;
        private readonly Action<T> _execute;

        public ActionCommand(Action<T> execute) : this(execute, _ => true)
        {
        }

        public ActionCommand(Action<T> execute, Func<T, bool> canExecute) : this(execute, canExecute, _ => { })
        {
        }

        public ActionCommand(Action<T> execute, Func<T, bool> canExecute,
                             Action<PropertyChangedEventHandler> addPropertyChanged)
        {
            if (execute == null) throw new ArgumentNullException("execute");
            if (canExecute == null) throw new ArgumentNullException("canExecute");
            if (addPropertyChanged == null) throw new ArgumentNullException("addPropertyChanged");
            _execute = execute;
            _canExecute = canExecute;

            addPropertyChanged(OwnerPropertyChanged);

            CommandManager.RequerySuggested += CommandManagerOnRequerySuggested;
        }

        private void CommandManagerOnRequerySuggested(object sender, EventArgs eventArgs)
        {
            CanExecuteChanged.Raise(this);
        }

        #region ICommand Members

        public void Execute(object parameter)
        {
            _execute((T) parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute((T) parameter);
        }

        public event EventHandler CanExecuteChanged;

        #endregion

        private void OwnerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CanExecuteChanged.Raise(this);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged.Raise(this);
        }
    }

    public class ActionCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        public ActionCommand(Action execute) : this(execute, () => true)
        {
        }

        public ActionCommand(Action execute, Func<bool> canExecute) : this(execute, canExecute, _ => { })
        {
        }

        public ActionCommand(Action execute, Func<bool> canExecute,
                             Action<PropertyChangedEventHandler> addPropertyChanged)
        {
            if (execute == null) throw new ArgumentNullException("execute");
            if (canExecute == null) throw new ArgumentNullException("canExecute");
            if (addPropertyChanged == null) throw new ArgumentNullException("addPropertyChanged");
            _execute = execute;
            _canExecute = canExecute;

            addPropertyChanged(OwnerPropertyChanged);
        }

        #region ICommand Members

        public void Execute(object parameter)
        {
            _execute();
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public event EventHandler CanExecuteChanged;

        #endregion

        private void OwnerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CanExecuteChanged.Raise(this);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged.Raise(this);
        }
    }

    //internal static class EventHandlerEx
    //{
    //    public static void Raise(this EventHandler handler, object sender)
    //    {
    //        if (handler != null)
    //        {
    //            handler(sender, EventArgs.Empty);
    //        }
    //    }
    //}
}