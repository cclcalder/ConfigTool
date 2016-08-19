using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using System.Xml.Serialization;
using Exceedra.Common;
using Exceedra.Common.Utilities;
using Model;
using Model.Annotations;
using Model.DataAccess;
using Model.DataAccess.Generic;
using Model.Language;
using Model.DataAccess.Listings;

namespace Model
{
    /// <summary>
    /// Created for unit testing purposes
    /// </summary>
    public interface IUser
    {
        User LogIn(string username, string password);
    }

    //public class LoginUser
    //{
    //    public LoginUser()
    //    {
    //    }

    //    public LoginUser(XElement el)
    //    {
    //        ID = el.GetValue<string>("User_Idx");
    //        Session = el.GetValue<string>("Session_ID");
    //    }

    //    public string ID { get; set; }
    //    public string Session { get; set; }


    //}


    public class User : IUser, INotifyPropertyChanged
    {
        /// <summary>
        /// Creates a new User instance.
        /// </summary>
        public User()
        {
        }

        [XmlIgnore]
        public  CurrentLanguageSet CurrentLanguage { get; set; }

        public string Session { get; set; }

        private string _accent = "#FF548ED5";

        public string Accent
        {
            get
            {
                return _accent;
            }
            set
            {
                _accent = value;
                OnPropertyChanged("Accent");
            }
        }

        public string Message { get; set; }
         

        public string Salt { get; set; }

        private static string version { get; set; }

        public  int SalesOrganisationID { get; set; }

        public string LoginMessage { get; set; }

        public User(XElement el)
        {
            ID = el.GetValue<string>("User_Idx");
            Session = el.GetValue<string>("Session_ID");
            SalesOrganisationID = el.GetValue<int>("SalesOrg_Idx");
            Message = el.GetValue<string>("Error");
            LoginMessage = el.Element("Msg").MaybeValue();
            Logging = true;// el.GetValue<string>("IsVerboseLogging") == "1";
        }

      

        #region ID

        /// <summary>
        /// Gets or sets the Id of this User.
        /// </summary>
        public string ID { get; set; }

        public RememberMe RememberMe { get; set; }

        #endregion

        #region DisplayName

        /// <summary>
        /// Gets or sets the DisplayName of this User.
        /// </summary>
        public string DisplayName { get; set; }

        #endregion

   

        public string LanguageCode { get; set; }

        /// <summary>
        /// Password storage
        /// </summary>

        /// <summary>
        /// username storage
        /// </summary>
        public string Hash1 { get; set; }

        public static void Logout()
        {
            ClearRememberedUser();
            CurrentUser = null;
            //clear our proc/results cache
            WebServiceProxy.cachedRequests.Clear();
            XmlCache.Clear();
            ListingsAccess.ClearListingsCache();
           // XmlCache.Clear();
        }

        private static User _CurrentUser;

        public static User CurrentUser
        {
            get
            {
                if (_CurrentUser == null)
                    _CurrentUser = GetRememberedUser(version);

                return _CurrentUser;
            }
            set { _CurrentUser = value; }

            }

        private static string GetFileName()
        {
            return version + ".wpf.xml";
        }

        public static void Remember(string v)
        {
            version = v;
            if (CurrentUser == null)
                throw new ApplicationException("Cannot Remember a null user");

            var storage =
                IsolatedStorageFile.GetStore(
                    IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null);
                // IsolatedStorageFile.GetUserStoreForApplication();
            using (var file = storage.CreateFile(GetFileName()))
            {
                var ser = new XmlSerializer(typeof (User));
                ser.Serialize(file, CurrentUser);
            }
        }

       

        public static User GetRememberedUser(string v)
        {
            version = v;
            var storage =
                IsolatedStorageFile.GetStore(
                    IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null);
            if (!storage.FileExists(GetFileName()))
                return null;

            try
            {
                using (var file = storage.OpenFile(GetFileName(), System.IO.FileMode.Open))
                {
                    var ser = new XmlSerializer(typeof (User));
                    return ser.Deserialize(file) as User;
                }
            }
            catch (Exception)
            {
                // any problem during reading file shall be considered as not having the file
                return null;
            }
        }

        public static void ClearRememberedUser()
        {

            

            if (CurrentUser == null)
                throw new ApplicationException("Cannot Remember a null user");

            var storage =
                IsolatedStorageFile.GetStore(
                    IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null);
            // IsolatedStorageFile.GetUserStoreForApplication();

            //RememberMe rem = RememberMe.Dont;
            if (CurrentUser.RememberMe == RememberMe.Automatic)
            {
                CurrentUser.RememberMe = RememberMe.Name;
            }

          
            using (var file = storage.CreateFile(GetFileName()))
            {
                var ser = new XmlSerializer(typeof(User));
                ser.Serialize(file, CurrentUser);
            }
 
        }


        public static void DeleteRememberUserFile()
        { 
            
            var storage =
                IsolatedStorageFile.GetStore(
                    IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null);
 
             
               if (storage.FileExists(GetFileName()))
                            storage.DeleteFile(GetFileName());
        }

        public string Description
        {
            get { return "{0} {1:yyyy/MM/dd}".FormatWith(DisplayName, DateTime.Now); }
        }

        public bool Logging { get; set; }


        //standard login using uername and password
        public static User Login(string username, string password)
        {
            return LoginAccess.GetLoginUserID_Salt(username, password);
        }

        public User LogIn(string username, string password)
        {
            return Login(username, password);
        }

        //Auto login feature to alloe logging in with only the user ID
        public static User Login(string userId)
        {
            var u = LoginAccess.GetUser(userId);

            if (u != null)
            {
                u.Hash1 = UserSecurity.Encrypt(userId);
            }

            return u;
        }

         
        public void HashData(string username)
        {
            Hash1 = UserSecurity.Encrypt(username);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum RememberMe
    {
        Dont = 0,
        Name = 1,
        Automatic = 2
    }
}
