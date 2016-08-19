using System.ServiceModel.Channels;
using System.Windows;

namespace Model.DataAccess
{
    using System;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Entity;

    public interface IClientConfigurationAccess
    {
        ClientConfiguration GetClientConfiguration();
        Task<ClientConfiguration> GetClientConfigurationAsync();
    }

    public class ClientConfigurationAccess : IClientConfigurationAccess
    {
        private static ClientConfiguration _cached;

        public ClientConfiguration GetClientConfiguration()
        {
            if (_cached == null )
            {
                try
                {
                    string arguments = "<GetSysConfig><User_Idx>{0}</User_Idx><Session_ID>{1}</Session_ID></GetSysConfig>".FormatWith(User.CurrentUser.ID, User.CurrentUser.Session);
                    var clientConfigXml = WebServiceProxy.Call(StoredProcedure.GetClientConfiguration,
                                                               XElement.Parse(arguments));

                     

                    return _cached = ClientConfiguration.FromXml(clientConfigXml);
                }
                catch (ExceedraDataException ex)
                {                    
                    return null;
                }
            }

            return _cached;
        }

        public ClientConfiguration GetClientConfiguration(bool forceReload)
        {
            if (forceReload) _cached = null;
            return GetClientConfiguration();
        }

        public Task<ClientConfiguration> GetClientConfigurationAsync()
        {
            if (_cached == null)
            {
                string arguments = "<GetSysConfig><User_Idx>{0}</User_Idx><Session_ID>{1}</Session_ID></GetSysConfig>".FormatWith(User.CurrentUser.ID, User.CurrentUser.Session);
                return WebServiceProxy.CallAsync(StoredProcedure.GetClientConfiguration, XElement.Parse(arguments))
                    .ContinueWith(t =>
                    {
                        var x = ClientConfiguration.FromXml(t.Result);
                        return  _cached = t.IsFaulted || t.Result == null  ? null  : x;
                    });
            }

            return CachedAsTask();
        }

        private static Task<ClientConfiguration> CachedAsTask()
        {
            var tcs = new TaskCompletionSource<ClientConfiguration>();
            tcs.SetResult(_cached);
            return tcs.Task;
        }
    }
}