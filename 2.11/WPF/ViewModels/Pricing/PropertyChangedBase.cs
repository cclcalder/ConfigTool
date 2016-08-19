using Exceedra.Common.Utilities;

namespace WPF.ViewModels.Pricing
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using Model;

    public abstract class PropertyChangedBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected bool Set<T>(ref T field, T value, Expression<Func<T>> property)
        {
            if (!Equals(field, value))
            {
                field = value;
                RaisePropertyChanged(property);
                return true;
            }

            return false;
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> property)
        {
            PropertyChanged.Raise(this, ((MemberExpression) property.Body).Member.Name);
        }
    }
}