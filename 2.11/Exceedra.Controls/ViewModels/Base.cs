using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Exceedra.Common;

namespace Exceedra.Controls.ViewModels
{
    [Serializable]
  public  class Base :  INotifyPropertyChanged
    {
        # region Properties

        protected bool ThrowOnInvalidProppertyName { get; private set; }

        # endregion

        protected Base()
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
            where TViewModel : Base
        {
            NotifyPropertyChanged(sender, properties.Select(ReflectionUtils.GetPropertyName).ToArray());
        }

        private void NotifyPropertyChanged(Base viewModel, params string[] propertyNames)
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

       
       
    }
}
