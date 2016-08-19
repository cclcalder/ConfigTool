using System;
using System.ComponentModel;
using System.Dynamic;
using Exceedra.Common.Utilities;

namespace Model
{
    public class DynamicViewModel<T> : DynamicObject, INotifyPropertyChanged
        where T : class
    {
        private readonly T _model;

        public DynamicViewModel(T model)
        {
            if (model == null) throw new ArgumentNullException("model");
            _model = model;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var propInfo = typeof(T).GetProperty(binder.Name);
            if (propInfo == null)
            {
                result = null;
                return false;
            }
            result = propInfo.GetValue(_model);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var propInfo = typeof(T).GetProperty(binder.Name);
            if (propInfo == null)
            {
                return false;
            }
            if (!Equals(value, propInfo.GetValue(_model)))
            {
                propInfo.SetValue(_model, value);
                PropertyChanged.Raise(this, binder.Name);
            }
            return true;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public T GetModel()
        {
            return _model;
        }
    }

    public static class DynamicViewModel
    {
        public static DynamicViewModel<T> Create<T>(T model)
            where T : class
        {
            return new DynamicViewModel<T>(model);
        }
    }
}