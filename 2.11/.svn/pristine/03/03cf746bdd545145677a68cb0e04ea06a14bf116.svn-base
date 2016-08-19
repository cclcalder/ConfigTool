using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Model;
using Model.DataAccess;
using Model.Entity;

namespace WPF.ViewModels
{
    class DesignTimeClientConfigurationAccess : IClientConfigurationAccess
    {
        public ClientConfiguration GetClientConfiguration()
        {
            return new ClientConfiguration(Feature.All(), RobScreens(), Screens(), "Promotions");
        }

        public Task<ClientConfiguration> GetClientConfigurationAsync()
        {
            var tcs = new TaskCompletionSource<ClientConfiguration>();
            tcs.SetResult(GetClientConfiguration());
            return tcs.Task;
        }

        private static IEnumerable<ROBScreen> RobScreens()
        {
            return App.Configuration.GetScreens().Where(r => r.RobAppType != null).Select(rob => new ROBScreen { AppTypeID = rob.RobAppType, Create = true, Title = "ROB" });
            //yield return new ROBScreen { AppTypeID = "300", Create = true, Title = "Three" };
            //yield return new ROBScreen { AppTypeID = "400", Create = true, Title = "Four" };
            //yield return new ROBScreen { AppTypeID = "500", Create = true, Title = "Five" };
            //yield return new ROBScreen { AppTypeID = "600", Create = true, Title = "Six" };
            //yield return new ROBScreen { AppTypeID = "700", Create = true, Title = "Seven" };
        }

        private static IEnumerable<Screen> Screens()
        {
            return new ObservableCollection<Screen>();
        }
    }
}