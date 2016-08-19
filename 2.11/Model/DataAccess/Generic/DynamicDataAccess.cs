using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Model.DataAccess.Converters;
using System.IO;
using System.Text;

namespace Model.DataAccess.Generic
{
    public static class DynamicDataAccess
    {
        #region Dynamic XML

        /* Simple method to show message aswell */
        public static bool SaveDynamicData(string proc, XElement args)
        {
            return MessageConverter.DisplayMessage(GetDynamicData(proc, args));
        }

        public static Task<XElement> GetDynamicDataAsync(string proc, XElement arguments, bool forceReload = true, string cacheOverride = null)
        {
            if (cacheOverride == null)
                cacheOverride = GetCacheString(proc, arguments);

            var xmlCache = XmlCache.GetItem(cacheOverride ?? proc);
            if (!forceReload && xmlCache != null)
                return FromResult((XElement)xmlCache.obj);

            return WebServiceProxy.CallAsync(proc, arguments).ContinueWith(t => GetDynamicDataContinuation(cacheOverride ?? proc, t));
        }

        public static XElement GetDynamicData(string proc, XElement arguments, bool forceReload = true, string cacheOverride = null)
        {
            if (cacheOverride == null)
                cacheOverride = GetCacheString(proc, arguments);

            var xmlCache = XmlCache.GetItem(cacheOverride ?? proc);
            if (!forceReload && xmlCache != null)
                return (XElement)xmlCache.obj;

            var res = WebServiceProxy.Call(proc, arguments, DisplayErrors.No);

            //res = ConvertXmlToStandard(res);
            XmlCache.Upsert(cacheOverride, res);

            return res;
        }

        private static XElement GetDynamicDataContinuation(string cacheItem, Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return XElement.Parse("<Result></Result>");

            //var res = ConvertXmlToStandard(task.Result);
            var res = task.Result;
            XmlCache.Upsert(cacheItem, res);

            return res;
        }

        #endregion 

        #region Dynamic Construction

        #region Single

        public static T GetGenericItem<T>(string proc, XElement arguments, bool forceReload = true, string cacheOverride = null) where T : new()
        {
            if (cacheOverride == null)
                cacheOverride = GetCacheString(proc, arguments);

            var xmlCache = XmlCache.GetItem(cacheOverride ?? proc);

            var result = (!forceReload && xmlCache != null) ? (XElement)xmlCache.obj : WebServiceProxy.Call(proc, arguments, DisplayErrors.No);

            return GetGenericItemContinuation<T>(cacheOverride ?? proc, result);
        }

        private static T GetGenericItemContinuation<T>(string cacheItem, XElement result) where T : new()
        {
            //result = ConvertXmlToStandard(result);

            XmlCache.Upsert(cacheItem, result);

            return (T)Activator.CreateInstance(typeof(T), result);
        }

        public static Task<T> GetGenericItemAsync<T>(string proc, XElement arguments, bool forceReload = true, string cacheOverride = null) where T : new()
        {
            if (cacheOverride == null)
                cacheOverride = GetCacheString(proc, arguments);

            var xmlCache = XmlCache.GetItem(cacheOverride ?? proc);
            if (!forceReload && xmlCache != null)
                return FromResult(GetGenericItemContinuation<T>(cacheOverride ?? proc, ((XElement)xmlCache.obj)));

            return WebServiceProxy.CallAsync(proc, arguments).ContinueWith(t => GetGenericItemAsyncContinuation<T>(cacheOverride ?? proc, t));
        }

        private static T GetGenericItemAsyncContinuation<T>(string cacheItem, Task<XElement> task) where T : new()
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return (T)Activator.CreateInstance(typeof(T));

            //var res = ConvertXmlToStandard(task.Result);
            var res = task.Result;
            XmlCache.Upsert(cacheItem, res);

            return (T)Activator.CreateInstance(typeof(T), res);
        }

        #endregion

        #region Enumerable

        public static IEnumerable<T> GetGenericEnumerable<T>(string proc, XElement arguments, bool forceReload = true, string cacheOverride = null) where T : new()
        {
            if (cacheOverride == null)
                cacheOverride = GetCacheString(proc, arguments);

            var xmlCache = XmlCache.GetItem(cacheOverride ?? proc);

            var result = (!forceReload && xmlCache != null) ? (XElement)xmlCache.obj : WebServiceProxy.Call(proc, arguments);

            return GetGenericEnumerableContinuation<T>(cacheOverride ?? proc, result);
        }

        private static IEnumerable<T> GetGenericEnumerableContinuation<T>(string cacheItem, XElement result) where T : new()
        {
            //result = ConvertXmlToStandard(result);
            XmlCache.Upsert(cacheItem, result);

            if (result == null)
                return new List<T> { (T)Activator.CreateInstance(typeof(T)) };

            return result.Elements().Select(n => (T)Activator.CreateInstance(typeof(T), n));
        }

        public static Task<IEnumerable<T>> GetGenericEnumerableAsync<T>(string proc, XElement arguments, bool forceReload = true, string cacheOverride = null) where T : new()
        {
            if (cacheOverride == null)
                cacheOverride = GetCacheString(proc, arguments);

            var xmlCache = XmlCache.GetItem(cacheOverride ?? proc);
            if (!forceReload && xmlCache != null)
                return FromResult(GetGenericEnumerableContinuation<T>(cacheOverride ?? proc, (XElement)xmlCache.obj));

            return WebServiceProxy.CallAsync(proc, arguments).ContinueWith(t => GetGenericEnumerableAsyncContinuation<T>(proc, t));
        }

        private static IEnumerable<T> GetGenericEnumerableAsyncContinuation<T>(string cacheItem, Task<XElement> task) where T : new()
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<T> { (T) Activator.CreateInstance(typeof (T)) };

            //var res = ConvertXmlToStandard(task.Result);
            var res = task.Result;
            XmlCache.Upsert(cacheItem, res);

            var result = res.Elements().Select(n => (T)Activator.CreateInstance(typeof(T), n)).ToList();
            return result;
        }

        #endregion

        #endregion

        public static Task<T> FromResult<T>(T value)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(value);
            return tcs.Task;
        }

        #region Result Conversion
        /* This is done to help the app use consistent xml without forcing the DB to change all it's outputs. */

        private static XElement ConvertXmlToStandard(XElement input)
        {
            try
            {
                //return XElement.Parse(input.ToString().Replace("ID>", "Idx>").Replace("Item_Idx>", "Idx>").Replace("Parent>", "ParentIdx>").Replace("Parent_Idx>", "ParentIdx>"));

                /* Warning: The below throws consistent out of memory exceptions when dealing with 30mb+ responses. 
                 * This means we cannot normalise that incoming data and may cause issues.
                 * IF this does cause an issue, we will need to work with the DB to ensure all incoming data is normalised to the below standards.
                 */
                var asString = input.ToString();
                var stringBuilder = new StringBuilder(asString);
                stringBuilder = stringBuilder.Replace("ID>", "Idx>");
                stringBuilder = stringBuilder.Replace("Item_Idx>", "Idx>");
                stringBuilder = stringBuilder.Replace("Parent>", "ParentIdx>");
                stringBuilder = stringBuilder.Replace("Parent_Idx>", "ParentIdx>");

                XElement xmlValue;
                using (StringReader stringReader = new StringReader(stringBuilder.ToString()))
                {
                    xmlValue = XElement.Load(stringReader);
                }

                return xmlValue;
            }
            catch (Exception e)
            {
                return input;
            }
        }

        #endregion

        #region RoBs Special Case
        /*Since RoBs all use the same proc, we need to use a different string for caching.
         *Here we check if we have an AppType Idx and if so, concat it to the cache string
         */

        public static string GetCacheString(string proc, XElement[] additionalInputs)
        {
            var specialCase = additionalInputs.Any() ? additionalInputs.FirstOrDefault(t => t != null && t.Name == "AppType_Idx") : null;
            if (specialCase != null)
                return proc + specialCase.Value;

            return proc;
        }
        
        public static string GetCacheString(string proc, XElement additionalInputs = null)
        {
            var nodeList = additionalInputs != null ? additionalInputs.DescendantsAndSelf().ToList() : null;

            var specialCaseNodes = new List<string> { "AppType_Idx" , "Screen_Code", "IsStatusMassUpdate", "IncludeTemplateStatuses", "IncludePromotionStatuses", "ReturnAsList", "ScreenKey", "Seasonal_Idx" };

            /* Adding special case values is required when shared procs are used */
            if (nodeList != null)
            {
                foreach (var special in specialCaseNodes)
                {
                    var specialCase = nodeList.FirstOrDefault(node => node.Name == special);
                    if (specialCase != null)
                        proc += specialCase.Value;
                }
            }

            proc += Model.User.CurrentUser.ID + Model.User.CurrentUser.SalesOrganisationID;

            return proc;
        }
        #endregion

    }
}