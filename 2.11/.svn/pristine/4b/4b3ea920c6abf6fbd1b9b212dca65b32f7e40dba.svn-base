using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Coder.UI.WPF;
using Exceedra.Common;


namespace ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using Exceedra.Common.Mvvm;

    public abstract class ViewModelBase : DependencyObject, INotifyPropertyChanged
    {
        # region Properties
        public string MyIdx { get; set; }

        protected bool ThrowOnInvalidProppertyName { get; private set; }

        # endregion

        protected ViewModelBase()
        {
            ThrowOnInvalidProppertyName = true;
        }

        # region Methods

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

        # endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected virtual void NotifyPropertyChanged<TViewModel>(TViewModel sender, params Expression<Func<TViewModel, object>>[] properties)
            where TViewModel : ViewModelBase
        {
            NotifyPropertyChanged(sender, properties.Select(ReflectionUtils.GetPropertyName).ToArray());
        }

        private void NotifyPropertyChanged(ViewModelBase viewModel, params string[] propertyNames)
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

        protected void Populate<TViewModel, TElement>(TViewModel vm, Task<IList<TElement>> task,
            ObservableCollection<TElement> collection, Action populated,
            params Expression<Func<TViewModel, object>>[] propertyExpressions)
            where TViewModel : ViewModelBase
        {
            Populate(vm, task, collection, e => e, populated, propertyExpressions);
        }

        protected void Populate<TViewModel, TEntity, TEntityViewModel>(TViewModel vm, Task<IList<TEntity>> task, 
            ObservableCollection<TEntityViewModel> collection, Func<IList<TEntity>, IList<TEntityViewModel>> createViewModelList, Action populated, 
            params Expression<Func<TViewModel, object>>[] propertyExpressions)
            where TViewModel : ViewModelBase
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    Populate(vm, task, collection, createViewModelList, populated, propertyExpressions)));
                return;
            }

            if (task.IsFaulted || task.IsCanceled) return;

            collection.Clear();
            if (collection is BulkObservableCollection<TEntity>)
                ((BulkObservableCollection<TEntityViewModel>) collection).AddBulkRange(createViewModelList(task.Result));
            else
                collection.AddRange(createViewModelList(task.Result));

            if (populated != null)
                populated();

            NotifyPropertyChanged(vm, propertyExpressions);
        }

        protected void Populate<TViewModel, TElement>(TViewModel vm, Task<IList<TElement>> task,
    List<TElement> collection, Action populated,
    params Expression<Func<TViewModel, object>>[] propertyExpressions)
    where TViewModel : ViewModelBase
        {
            Populate(vm, task, collection, e => e, populated, propertyExpressions);
        }

        protected void Populate<TViewModel, TEntity, TEntityViewModel>(TViewModel vm, Task<IList<TEntity>> task,
            List<TEntityViewModel> collection, Func<IList<TEntity>, IList<TEntityViewModel>> createViewModelList, Action populated,
            params Expression<Func<TViewModel, object>>[] propertyExpressions)
            where TViewModel : ViewModelBase
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    Populate(vm, task, collection, createViewModelList, populated, propertyExpressions)));
                return;
            }

            if (task.IsFaulted || task.IsCanceled) return;

            collection = new List<TEntityViewModel>();
            if (collection is List<TEntity>)
                ((List<TEntityViewModel>)collection).AddRange(createViewModelList(task.Result));
            else
                collection.AddRange(createViewModelList(task.Result));

            if (populated != null)
                populated();

            NotifyPropertyChanged(vm, propertyExpressions);
        }

        protected static void MessageBoxShow(string message, string title = null,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxImage image = MessageBoxImage.Information)
        {
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
        }
    }
}