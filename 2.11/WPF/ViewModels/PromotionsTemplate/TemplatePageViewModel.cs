namespace WPF.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Automation;
    using System.Windows.Controls;
    using System.Windows.Input;
    using ViewHelper;

 
    public class TemplatePageViewModel: INotifyPropertyChanged
    {
        private static readonly Action NoOp = () => { };
        private static readonly Func<bool> AllGood = () => true;

        private readonly string _name;
        private readonly TemplateViewModelBase _parentViewModel;
        private Action _afterNavigateAway = NoOp;
        private Action _beforeNavigateAway = NoOp;
        private Action _beforeNavigateBackTo = NoOp;
        private Action _beforeNavigateInTo = NoOp;
        private Func<bool> _canAttemptNavigate = AllGood;
        private ICommand _goToCommand;
        private Page _pageView;
        private Action _reset = NoOp;
        private Func<bool> _validate = AllGood;
        private bool _valid;

        public bool HasChanges { get; set; }
        public bool ForceReload { get; set; }

        private bool _clear;
        private string _errorMessage;

        public TemplatePageViewModel(TemplateViewModelBase parentViewModel, string name)
        {
            _parentViewModel = parentViewModel;
            _name = name;
            ThrowOnInvalidProppertyName = true;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Title { get; set; }

        public string LastSavedDate { get; set; }
        public bool Visited { get; set; }
         

        public ICommand GoToCommand
        {
            get
            {
               Valid = CanAttemptNavigate();
                
                return _goToCommand ?? (_goToCommand = new ViewCommand(
                                                           _ =>
                                                           IsCurrent || (AllPreviousVisited()),
                                                           _ => _parentViewModel.CurrentPageIndex = Index));
            }
        }

        public bool IsCurrent
        {
            get { return _parentViewModel.CurrentOffset(this) == 0; }
        }

        private bool IsNext
        {
            get { return _parentViewModel.CurrentOffset(this) == 1; }
        }

        //public bool Clear
        //{
        //    get { return !Valid; }
        //    //get { return _clear; }
        //    //set { _clear = value;
        //    //NotifyPropertyChanged(this, "Clear");
        //    //}
        //}


        public bool Valid
        {
            get { return _valid; }
            set
            {
                _valid = value;
                NotifyPropertyChanged(this, "Valid");
                //if (_valid == true)
                //{
                //    State = ToggleState.On;
                //}
                //else
                //{
                //    State = ToggleState.Off;
                //}
                
            }
        }

        private bool _reloadData;
        public bool ReloadData
        {
            get { return _reloadData; }
            set
            {
                _reloadData = value;
                NotifyPropertyChanged(this, "ReloadData");
               
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyPropertyChanged(this, "ErrorMessage");
            }
        }

        private ToggleState _state;
        public ToggleState State
        {
            get { return _state; }
            set
            {
                _state = value;
                NotifyPropertyChanged(this, "State");

            }
        }

        //private bool _hasChanged;
        //public bool HasChanged
        //{
        //    get { return _hasChanged; }
        //    set
        //    {
        //        _hasChanged = value;
        //        NotifyPropertyChanged(this, "HasChanged");

        //    }
        //}


        /// <summary>
        /// Navigate away command buttons become available for click - client-side validation
        /// </summary>
        public Func<bool> CanAttemptNavigate
        {
            get { return _canAttemptNavigate; }
            set { _canAttemptNavigate = value ?? AllGood; }
        }

        /// <summary>
        /// Server-side validation
        /// </summary>
        public Func<bool> Validate
        {
            get { return _validate; }
            set { _validate = value ?? AllGood;
            //Valid = _validate();
            }
        }

        public Type PageViewType { get; set; }

        public Page PageView
        {
            get { return _pageView ?? (_pageView = Activator.CreateInstance(PageViewType, _parentViewModel) as Page); }
        }

        public int Index
        {
            get { return _parentViewModel.PageList.IndexOf(this); }
        }

        public Action BeforeNavigateInTo
        {
            get { return _beforeNavigateInTo; }
            set { _beforeNavigateInTo = value ?? NoOp; }
        }

        public Action BeforeNavigateBackTo
        {
            get { return _beforeNavigateBackTo; }
            set { _beforeNavigateBackTo = value ?? NoOp; }
        }

        public Action BeforeNavigateAway
        {
            get { return _beforeNavigateAway; }
            set { _beforeNavigateAway = value ?? NoOp; }
        }

        public Action AfterNavigateAway
        {
            get { return _afterNavigateAway; }
            set { _afterNavigateAway = value ?? NoOp; }
        }

        public Action Reset
        {
            get { return _reset; }
            set { _reset = value ?? NoOp; }
        }

        private bool AllPreviousVisited()
        {
            return _parentViewModel.PageList
                .Where((p, index) => index < Index)
                .All(p => p.Visited);
        }

        private void NotifyPropertyChanged(TemplatePageViewModel viewModel, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                VerifyPropertyName(propertyName);

                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    var e = new PropertyChangedEventArgs(propertyName);
                    handler(viewModel, e);
                }
            }
        }

        # region Methods

        [Conditional("DEBUG")]
        public void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (ThrowOnInvalidProppertyName)
                {
                    throw new Exception(msg);
                }

                Debug.Fail(msg);
            }
        }
        protected bool ThrowOnInvalidProppertyName { get; private set; }
        # endregion
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
 }