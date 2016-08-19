using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Exceedra.Common.Utilities;

namespace Exceedra.ChangePasswordPanel
{
    /// <summary>
    /// Interaction logic for LoggingPanel.xaml
    /// </summary>
    public partial class ChangePasswordPanel
    {
        public ChangePasswordPanel()
        {
            InitializeComponent();
        }

        #region properties

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
            "Password", typeof(string), typeof(ChangePasswordPanel), new PropertyMetadata(default(string)));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty ConfirmPasswordProperty = DependencyProperty.Register(
            "ConfirmPassword", typeof(string), typeof(ChangePasswordPanel), new PropertyMetadata(default(string)));

        public string ConfirmPassword
        {
            get { return (string)GetValue(ConfirmPasswordProperty); }
            set { SetValue(ConfirmPasswordProperty, value); }
        }

        public static readonly DependencyProperty PasswordValidatorProperty = DependencyProperty.Register(
            "PasswordValidator", typeof(PasswordValidator), typeof(ChangePasswordPanel), new PropertyMetadata(default(PasswordValidator)));

        public PasswordValidator PasswordValidator
        {
            get { return (PasswordValidator)GetValue(PasswordValidatorProperty); }
            set { SetValue(PasswordValidatorProperty, value); }
        }

        public static readonly DependencyProperty PoliciesValidationResultProperty = DependencyProperty.Register(
            "PoliciesValidationResult", typeof(ObservableCollection<PasswordPolicy>), typeof(ChangePasswordPanel), new PropertyMetadata(default(ObservableCollection<PasswordPolicy>)));

        public ObservableCollection<PasswordPolicy> PoliciesValidationResult
        {
            get { return (ObservableCollection<PasswordPolicy>)GetValue(PoliciesValidationResultProperty); }
            set { SetValue(PoliciesValidationResultProperty, value); }
        }

        #endregion

        #region commands

        public static readonly DependencyProperty SavePasswordCommandProperty = DependencyProperty.Register(
            "SavePasswordCommand", typeof(ICommand), typeof(ChangePasswordPanel), new PropertyMetadata(default(ICommand)));

        public ICommand SavePasswordCommand
        {
            get { return (ICommand)GetValue(SavePasswordCommandProperty); }
            set { SetValue(SavePasswordCommandProperty, value); }
        }

        #endregion

        #region events

        public static readonly DependencyProperty CancelButtonVisibilityProperty = DependencyProperty.Register(
            "CancelButtonVisibility", typeof(Visibility), typeof(ChangePasswordPanel), new PropertyMetadata(Visibility.Collapsed));

        public Visibility CancelButtonVisibility
        {
            get { return (Visibility)GetValue(CancelButtonVisibilityProperty); }
            set { SetValue(CancelButtonVisibilityProperty, value); }
        }

        public event RoutedEventHandler CancelClick;
        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (CancelClick != null)
            {
                CancelClick(this, e);
                Clean();
            }
        }

        public event RoutedEventHandler SaveClick;
        private void SavePasswordButton_OnClick(object sender, RoutedEventArgs e)
        {
            // invoke the command
            // http://stackoverflow.com/questions/20565719/wpf-button-click-and-command-doesnt-work-together-mvvm
            var btn = sender as Button;
            if (btn != null && btn.Command.CanExecute(btn.CommandParameter))
                btn.Command.Execute(btn.CommandParameter);

            // invoke the click
            if (SaveClick != null)
                SaveClick(this, e);

            Clean();
        }

        #endregion

        #region labels

        public static readonly DependencyProperty NewPasswordLabelProperty = DependencyProperty.Register(
            "NewPasswordLabel", typeof(string), typeof(ChangePasswordPanel), new PropertyMetadata("New password"));

        public string NewPasswordLabel
        {
            get { return (string)GetValue(NewPasswordLabelProperty); }
            set { SetValue(NewPasswordLabelProperty, value); }
        }

        public static readonly DependencyProperty ConfirmNewPasswordLabelProperty = DependencyProperty.Register(
            "ConfirmNewPasswordLabel", typeof(string), typeof(ChangePasswordPanel), new PropertyMetadata("Confirm new password"));

        public string ConfirmNewPasswordLabel
        {
            get { return (string)GetValue(ConfirmNewPasswordLabelProperty); }
            set { SetValue(ConfirmNewPasswordLabelProperty, value); }
        }

        public static readonly DependencyProperty PasswordDoNotMatchLabelProperty = DependencyProperty.Register(
            "PasswordDoNotMatchLabel", typeof(string), typeof(ChangePasswordPanel), new PropertyMetadata("Passwords don't match"));

        public string PasswordDoNotMatchLabel
        {
            get { return (string)GetValue(PasswordDoNotMatchLabelProperty); }
            set { SetValue(PasswordDoNotMatchLabelProperty, value); }
        }

        public static readonly DependencyProperty CancelLabelProperty = DependencyProperty.Register(
            "CancelLabel", typeof(string), typeof(ChangePasswordPanel), new PropertyMetadata("Cancel"));

        public string CancelLabel
        {
            get { return (string)GetValue(CancelLabelProperty); }
            set { SetValue(CancelLabelProperty, value); }
        }

        public static readonly DependencyProperty SavePasswordLabelProperty = DependencyProperty.Register(
            "SavePasswordLabel", typeof(string), typeof(ChangePasswordPanel), new PropertyMetadata("Save password"));

        public string SavePasswordLabel
        {
            get { return (string)GetValue(SavePasswordLabelProperty); }
            set { SetValue(SavePasswordLabelProperty, value); }
        }

        #endregion

        #region passwords change events

        private void NewPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = NewPassword.Password;
            PoliciesValidationResult = PasswordValidator.Validate(Password).ToObservableCollection();

            // disable Save Password if policies not met
            if (!PasswordValidator.IsValid)
                SavePasswordButton.IsEnabled = false;
            else SavePasswordButton.IsEnabled = true;

            PoliciesValidationGrid.Visibility = Visibility.Visible;

            if (NewPassword.Password == NewPassword2.Password)
                PasswordDontMatchWarning.Visibility = Visibility.Collapsed;
        }

        private void NewPassword2_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ConfirmPassword = NewPassword2.Password;

            if (NewPassword.Password != NewPassword2.Password)
                PasswordDontMatchWarning.Visibility = Visibility.Visible;
            else PasswordDontMatchWarning.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region revealing / hiding password boxes

        private void RevealNewPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RevealNewPassword();
        }

        private void RevealNewPassword_MouseUp(object sender, MouseButtonEventArgs e)
        {
            HideNewPassword();
        }

        private void RevealNewPassword_MouseLeave(object sender, MouseEventArgs e)
        {
            HideNewPassword();
        }

        private void RevealNewPassword()
        {
            NewPassword.Visibility = Visibility.Collapsed;
            NewPasswordRevealed.Visibility = Visibility.Visible;

            NewPasswordRevealed.Text = NewPassword.Password;
        }

        private void HideNewPassword()
        {
            NewPassword.Visibility = Visibility.Visible;
            NewPasswordRevealed.Visibility = Visibility.Collapsed;

            NewPassword.Focus();
        }

        private void RevealNewPassword2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RevealNewPassword2();
        }

        private void RevealNewPassword2_MouseUp(object sender, MouseButtonEventArgs e)
        {
            HideNewPassword2();
        }

        private void RevealNewPassword2_MouseLeave(object sender, MouseEventArgs e)
        {
            HideNewPassword2();
        }

        private void RevealNewPassword2()
        {
            NewPassword2.Visibility = Visibility.Collapsed;
            NewPassword2Revealed.Visibility = Visibility.Visible;

            NewPassword2Revealed.Text = NewPassword2.Password;
        }

        private void HideNewPassword2()
        {
            NewPassword2.Visibility = Visibility.Visible;
            NewPassword2Revealed.Visibility = Visibility.Collapsed;

            NewPassword2.Focus();
        }

        private void Clean()
        {
            NewPassword.Password = string.Empty;
            NewPassword2.Password = string.Empty;
            PoliciesValidationGrid.Visibility = Visibility.Collapsed;
        }

        #endregion
    }
}
