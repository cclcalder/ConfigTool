using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;
using Exceedra.Common;
using Exceedra.Common.Mvvm;

namespace ViewModels
{
    using System;
    using System.ComponentModel;

    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public string MyIdx { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            CommandManager.InvalidateRequerySuggested();
        }

        protected bool Set<T>(ref T field, T value, string propertyName)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool Set(ref string field, string value, string propertyName)
        {
            if (field == value) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected static void MessageBoxShow(string message, string title = null,
                                             MessageBoxButton button = MessageBoxButton.OK,
                                             MessageBoxImage image = MessageBoxImage.Information)
        {

            Application.Current.Dispatcher.Invoke((Action)delegate {

                switch (image)
                {
                    case MessageBoxImage.Error:
                        Messages.Instance.PutError(message);
                        break;
                    case MessageBoxImage.Warning:
                        Messages.Instance.PutWarning(message);
                        break;
                    default:
                        Messages.Instance.PutInfo(message);
                        break;
                }

            });

           
        }

        protected virtual void NotifyPropertyChanged<TViewModel>(TViewModel sender, params Expression<Func<TViewModel, object>>[] properties)
    where TViewModel : BaseViewModel
        {
            NotifyPropertyChanged(sender, properties.Select(ReflectionUtils.GetPropertyName).ToArray());
        }

        private void NotifyPropertyChanged(BaseViewModel viewModel, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                VerifyPropertyName(propertyName);

                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    var e = new PropertyChangedEventArgs(propertyName);
                    handler(viewModel, e);
                }
            }
        }

        [Conditional("DEBUG")]
        public void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (ThrowOnInvalidProppertyName)
                {
                    throw new Exception(msg);
                }

                Debug.Fail(msg);
            }
        }

        protected bool ThrowOnInvalidProppertyName { get; private set; }
    }
}