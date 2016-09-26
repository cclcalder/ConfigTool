using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool
{
    //this should be in the setup controller
    class DatabaseConnect
    {
        private static void BuildConnectionString(string dataSource, string userName, string userPassword)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["databaseConnect"];
            SqlConnectionStringBuilder builder;
            //LinqToSQLDataClassDataContext db;

            if (settings != null)
            {
                //retrieve 'default'
                string connection = settings.ConnectionString;
                builder = new SqlConnectionStringBuilder(connection);

                // Supply the additional values.
                builder.DataSource = dataSource;
                builder.UserID = userName;
                builder.Password = userPassword;

                //var db = new LinqToSQLDataClassDataContext(builder.ConnectionString);
            }
        }
    }
}
