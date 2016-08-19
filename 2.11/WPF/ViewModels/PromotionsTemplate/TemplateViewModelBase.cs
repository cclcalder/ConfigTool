using System.Windows;
using System.Windows.Automation;

namespace WPF.ViewModels 
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using ViewHelper;
    using global::ViewModels;
    using  WPF.ViewModels.PromoTemplates;


    public abstract class TemplateViewModelBase : ViewModelBase
    {
        private ICommand _backCommand;
        private ICommand _cancelCommand;
        private int _currentPageIndex = -1;
        private ICommand _nextCommand;
        private List<TemplatePageViewModel> _pageList;

        public List<TemplatePageViewModel> PageList
        {
            get { return _pageList ?? (_pageList = new List<TemplatePageViewModel>(ConstructPageList())); }
            set
            {
                _pageList = value;
                NotifyPropertyChanged(this, vm=>vm.PageList);
            }
        }

        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set { NavigateToPageIndex(value, null); }
        }

        public ICommand NextCommand
        {
            get
            {
                return _nextCommand ?? (_nextCommand = new ViewCommand(
                                                           _ =>
                                                           CurrentPageIndex < PageList.Count - 1 &&
                                                           PageList[CurrentPageIndex].CanAttemptNavigate(),
                                                           _ => CurrentPageIndex++));
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new ViewCommand(
                                                           _ => CurrentPageIndex > 0,
                                                           _ => CurrentPageIndex--));
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new ViewCommand(
                                                               _ => true,
                                                               _ => OnCancel()));
            }
        }

        public void NavigateToPageIndex(int value, Action callback)
        {
            if (_currentPageIndex != value)
            {
                if (_currentPageIndex == -1)
                {
                    _currentPageIndex = value;
                    NavigateToPage(callback);
                    return;
                }

                int oldIndex = _currentPageIndex;
                int newIndex = value;
                TemplatePageViewModel oldPage = PageList[oldIndex];
                TemplatePageViewModel newPage = PageList[newIndex];

                 //Backwards Jim <-
                if (_currentPageIndex > value)
                {
                    _currentPageIndex = newIndex;
                    
                    //FFS
                    newPage.BeforeNavigateBackTo();

                    NavigateToPage(callback);
                    OnPageChanged(oldIndex, newIndex);
                }
                else if (oldPage.Validate())// Warp factor 9 -> that way
                {
                    oldPage.BeforeNavigateAway();
                    // only go forth and pillage if the save was completed nad state is OK
                    if (oldPage.State == ToggleState.On ||  oldPage.Valid == true)
                    {
                        oldPage.Visited = true;
                        _currentPageIndex = newIndex;

                        NavigateToPage(callback);
                        oldPage.AfterNavigateAway(); 
                        OnPageChanged(oldIndex, newIndex);
                    }
                }
                
            }
        }

        private void NavigateToPage(Action callback)
        {
            TemplatePageViewModel TemplatePageViewModel = PageList[_currentPageIndex];
            //if (!TemplatePageViewModel.Visited)
            TemplatePageViewModel.BeforeNavigateInTo();

            var templateIdx = ((PromotionTemplateViewModel)TemplatePageViewModel.PageView.DataContext).CurrentTemplate.Id;
            ThisWindow = ThisWindow ?? Application.Current.Windows.OfType<Window>().SingleOrDefault(x => (string)x.Tag == "template::" + templateIdx);
            
            //If we are not viewing in a popup, do the normal nav
            if (ThisWindow == null)
            {
                if (callback != null)
                {
                    App.Navigator.NavigateTo(TemplatePageViewModel.PageView, callback);
                }
                else
                {
                    App.Navigator.NavigateTo(TemplatePageViewModel.PageView);
                }
            }
            else
            {
                ThisWindow.Content = TemplatePageViewModel.PageView;
                if (callback != null) callback();
                App.Navigator.EnableNavigation(true);
            }
        }

        /* When navigating using popup windows we hold the Window here.
         * We must only set this property once, as when making a new promo the Idx changes and will stop us from identifying 
         * the parent window.
         */
        private Window ThisWindow { get; set; }

        /// <summary>
        /// Occurs after a page change has been completed and all Before and After delegates 
        /// on the page modesl have been fired.
        /// </summary>
        /// <param name="oldPageIndex"></param>
        /// <param name="newPageIndex"></param>
        protected virtual void OnPageChanged(int oldPageIndex, int newPageIndex)
        {
        }

        protected abstract IEnumerable<TemplatePageViewModel> ConstructPageList();

        protected abstract void OnCancel();

        /// <summary>
        /// Get offset from current page of <paramref name="page"/>.
        /// </summary>
        /// <param name="page">Page to calculate offset for.</param>
        /// <returns>0 if page is current, negative if page has passed, positive if page is yet to come.</returns>
        public int CurrentOffset(TemplatePageViewModel page)
        {
            return PageList.IndexOf(page) - CurrentPageIndex;
        }

        protected void ResetLaterPages(int pageIndex)
        {
            foreach (TemplatePageViewModel TemplatePageViewModel in PageList.Skip(pageIndex + 1))
            {
                TemplatePageViewModel.ForceReload = true;
            }
        }

        public void GoTo(TemplatePageViewModel page)
        {
            int targetIndex = GetPageIndex(page);
            CurrentPageIndex = targetIndex;
        }

        protected int GetPageIndex(TemplatePageViewModel page)
        {
            return PageList.IndexOf(page);
        }

        protected void ResetLaterPages(TemplatePageViewModel pageViewModel)
        {
            ResetLaterPages(PageList.IndexOf(pageViewModel));
        }
    }
}