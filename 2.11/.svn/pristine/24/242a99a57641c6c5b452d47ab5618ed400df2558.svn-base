using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Model.Entity.Demand;
using Model.Entity.Funds;
using Model.Entity.Generic;

namespace Model.DataAccess.Generic
{
    public static class ComboBoxAccess
    {
        #region Comboboxes
        public static Task<List<ComboboxItem>> GetComboboxItems(string proc, XElement arguments)
        {
            return WebServiceProxy.CallAsync(proc, arguments).ContinueWith(t => GetComboboxItemsContinuation(t));
        }

        private static List<ComboboxItem> GetComboboxItemsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<ComboboxItem>() { new ComboboxItem(XElement.Parse("<Results><Name>No Items</Name><Idx>1</Idx><IsSelected>1</IsSelected></Results>")) };

            return task.Result.Elements().Select(n => new ComboboxItem(n)).ToList();
        }
        #endregion

        #region ComboboxGeneric

        public static IEnumerable<T> GetComboboxGenericItems<T>(string proc, XElement arguments) where T : new()
        {
            //return FromResult<XElement>(XElement.Parse("<Items><Item><Idx>1</Idx><Name>Fixed</Name><IsSelected>1</IsSelected><MultiplicationFactor>1</MultiplicationFactor></Item><Item><Idx>2</Idx><Name>Rate Per Case</Name><IsSelected>0</IsSelected><Format>C2</Format><MultiplicationFactor>5000</MultiplicationFactor></Item><Item><Idx>2</Idx><Name>Half Rate Per Case</Name><IsSelected>0</IsSelected><Format>C2</Format><MultiplicationFactor>2500</MultiplicationFactor></Item></Items>")).ContinueWith(t => GetComboboxGenericItemsContinuation<T>(t));

            return GetComboboxGenericItemsContinuation<T>(WebServiceProxy.Call(proc, arguments));
        }

        private static IEnumerable<T> GetComboboxGenericItemsContinuation<T>(XElement task) where T : new()
        {
            if (task == null)
                return new List<T>() { (T)Activator.CreateInstance(typeof(T), "<Name>No Items</Name><Idx>1</Idx><IsSelected>1</IsSelected>") };

            return task.Elements().Select(n => (T)Activator.CreateInstance(typeof(T), n)).ToList();
        }

        public static Task<IEnumerable<T>> GetComboboxGenericItemsAsync<T>(string proc, XElement arguments) where T : new()
        {
            //return FromResult<XElement>(XElement.Parse("<Items><Item><Idx>1</Idx><Name>Fixed</Name><IsSelected>1</IsSelected><MultiplicationFactor>1</MultiplicationFactor></Item><Item><Idx>2</Idx><Name>Rate Per Case</Name><IsSelected>0</IsSelected><Format>C2</Format><MultiplicationFactor>5000</MultiplicationFactor></Item><Item><Idx>2</Idx><Name>Half Rate Per Case</Name><IsSelected>0</IsSelected><Format>C2</Format><MultiplicationFactor>2500</MultiplicationFactor></Item></Items>")).ContinueWith(t => GetComboboxGenericItemsContinuation<T>(t));

            return WebServiceProxy.CallAsync(proc, arguments).ContinueWith(t => GetComboboxGenericItemsAsyncContinuation<T>(t));
        }

        private static IEnumerable<T> GetComboboxGenericItemsAsyncContinuation<T>(Task<XElement> task) where T : new()
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<T>() { (T)Activator.CreateInstance(typeof(T), "<Name>No Items</Name><Idx>1</Idx><IsSelected>1</IsSelected>") };

            return task.Result.Elements().Select(n => (T)Activator.CreateInstance(typeof(T), n)).ToList();
        }

        public static Task<T> FromResult<T>(T value)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(value);
            return tcs.Task;
        }

        #endregion
    }
}