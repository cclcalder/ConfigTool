using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.Entity;
using Exceedra.Common.Utilities;
using Model.DataAccess;
using ViewHelper;
using ViewModels;

namespace WPF.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel()
        {
            LoginWithTwitter = LoginAccess.GetSSOSettings().CanUseTwitterLogin;
            PasswordModalPresenterVis = Visibility.Hidden;

            var passwordPolicy = LoginAccess.GetPasswordPolicy(UserName);
            if (passwordPolicy != null) PasswordValidator = new PasswordValidator(passwordPolicy, UserName);
            else PasswordValidator = new PasswordValidator(XElement.Parse("<Results/>"));
        }

        public bool IsAzureADActive
        {
            get
            {
                return App.AzureADConfigData.IsActive;
            }
        } 

        public bool LoginWithTwitter { get; set; }

        private Visibility _passwordModalPresenterVis;
        public Visibility PasswordModalPresenterVis
        {
            get { return _passwordModalPresenterVis; }
            set
            {
                _passwordModalPresenterVis = value;
                NotifyPropertyChanged(this, vm => vm.PasswordModalPresenterVis);
            }
        }

        #region User Credentials 

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyPropertyChanged(this, vm => vm.UserName);
            }
        }

        private string _newPassword;

        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                _newPassword = value;
                NotifyPropertyChanged(this, vm => vm.NewPassword);
            }
        }

        private string _newPassword2;

        public string NewPassword2
        {
            get { return _newPassword2; }
            set
            {
                _newPassword2 = value;
                NotifyPropertyChanged(this, vm => vm.NewPassword2);
            }
        }

        private string _resetCode;

        public string ResetCode
        {
            get { return _resetCode; }
            set
            {
                _resetCode = value;
                NotifyPropertyChanged(this, vm => vm.ResetCode);
            }
        }

        #endregion

        #region Save new password

        public ICommand SavePasswordCommand
        {
            get { return new ViewCommand(_canSaveNewPassword, SaveNewPassword); }
        }

        private bool _canSaveNewPassword(object obj)
        {
            return UserName != null && UserName.Any() && NewPassword != null && NewPassword.Any() && NewPassword == NewPassword2 && ResetCode != null && ResetCode.Any();
        }

        private void SaveNewPassword(object obj)
        {
            XElement res = new XElement("Dummy");

            string returnPassword = UserSecurity.BcryptEncrypt(NewPassword);

            res = LoginAccess.SaveNewPassword(UserName, ResetCode, returnPassword);
            if (res.MaybeElement("Error") != null)
                MessageBoxShow(res.MaybeElement("Error").Value, null, MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
               MessageBoxShow(res.MaybeElement("Message").Value);
               PasswordModalPresenterVis = Visibility.Hidden;
            }
        }

        private PasswordValidator _passwordValidator;
        public PasswordValidator PasswordValidator
        {
            get { return _passwordValidator; }
            set
            {
                _passwordValidator = value;
                NotifyPropertyChanged(this, vm => vm.PasswordValidator);
            }
        }

        #endregion

        #region Request reset email

        public ICommand ResetPasswordRequestCommand
        {
            get { return new ViewCommand(_canResetPassword, ResetPasswordRequest); }
        }

        private bool _canResetPassword(object obj)
        {
            return UserName != null;
        }

        private void ResetPasswordRequest(object obj)
        {
            //AppEmailer appEmailer = new AppEmailer();

            //DB now sends email itself
            var resetvalues = GetResetValues();
            // var serverDetails = GetServerDetails();

            if (resetvalues != null)// && serverDetails != null)
            {
                //appEmailer.SendEmail(resetvalues, serverDetails, App.GetLoggingConfigHost());                 
                MessageBoxShow(resetvalues.Message);
            }
        }

        public ResetEmailModel GetResetValues()
        {
            var res = LoginAccess.ForgottenPasswordGetDetails(UserName);
            if (res.MaybeElement("Error") != null)
            {
                MessageBoxShow(res.Element("Error").Value);
                return null;
            }

            return new ResetEmailModel(res);
        }

        //public ServerDetailsModel GetServerDetails()
        //{
        //    var res = LoginAccess.GetMailServerDetails();
        //    if (res.MaybeElement("Error") != null)
        //    {
        //        MessageBoxShow(res.Element("Error").Value);
        //        return null;
        //    }

        //    return new ServerDetailsModel(res);
        //}

        #endregion
    }
}
