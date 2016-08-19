using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Exceedra.DB.DBContext;

namespace Exceedra.DB.Composite
{
   public  class AppUser : Dim_User
    {
       public Dim_User Login(string u, string p)
       { 
           var db = new ExceedraDBDataContext();
           var res = db.Dim_Users.SingleOrDefault(t => t.User_LoginName == u && t.User_LoginPassword == p);

           return res;
       }


       public object Login(XElement u)
       {
           var db = new ExceedraDBDataContext();
           var res = db.Procast_SP_LOGIN_User_Login(u, 0, false);

           return res;
       }
    }
}
