using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.DataAccess;
using Model.Entity;

namespace SalesPlannerWeb
{
    public class WebConfiguration
    {
        private static ClientConfiguration _configuration;
        public static ClientConfiguration Configuration
        {
            get
            {
                return _configuration ?? (_configuration = new ClientConfigurationAccess().GetClientConfiguration());
            }
            set { _configuration = value; }
        }
    }
}