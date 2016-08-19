using Exceedra.Common;
using Exceedra.Common.Utilities;
using Exceedra.Controls.Messages;

namespace WPF.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using Model;
    using Model.DataAccess;
    using Model.Entity;
    using Exceedra.Common.Mvvm;
    using ViewHelper;
    using global::ViewModels;
    using Model.Utilities;

    public class ApplyPhasingViewModel : ViewModelBase
    {
        public static readonly DependencyProperty SelectedWeekProfileProperty =
            DependencyProperty.Register("SelectedWeekProfile", typeof(PhasingProfile), typeof(ApplyPhasingViewModel),
                                        new PropertyMetadata(default(PhasingProfile), OnSelectedWeekProfileChanged));

        public static readonly DependencyProperty SelectedFirstWeekDayProfileProperty =
            DependencyProperty.Register("SelectedFirstWeekDayProfile", typeof(PhasingProfile), typeof(ApplyPhasingViewModel),
                                        new PropertyMetadata(default(PhasingProfile), OnSelectedFirstWeekDayProfileChanged));

        public static readonly DependencyProperty SelectedOtherWeeksDayProfileProperty =
            DependencyProperty.Register("SelectedOtherWeeksDayProfile", typeof(PhasingProfile), typeof(ApplyPhasingViewModel),
                                        new PropertyMetadata(default(PhasingProfile), OnSelectedOtherWeeksDayProfileChanged));

        public static readonly DependencyProperty SelectedFinalWeekDayProfileProperty =
            DependencyProperty.Register("SelectedFinalWeekDayProfile", typeof(PhasingProfile), typeof(ApplyPhasingViewModel),
                                        new PropertyMetadata(default(PhasingProfile), OnSelectedFinalWeekDayProfileChanged));

        public static readonly DependencyProperty SelectedPostPromoProfileProperty =
            DependencyProperty.Register("SelectedPostPromoProfile", typeof(PhasingProfile), typeof(ApplyPhasingViewModel),
                                        new PropertyMetadata(default(PhasingProfile), OnSelectedPostPromoProfileChanged));

        private bool _isApplying { get; set; }
        private bool IsApplying
        {
            get { return _isApplying; }
            set { _isApplying = value; _applyCommand.RaiseCanExecuteChanged(); _removeCommand.RaiseCanExecuteChanged(); }
        }
        private readonly ActionCommand _applyCommand;
        private readonly ActionCommand _removeCommand;

        private readonly RangeObservableCollection<DayPhasingProfile> _dayPhasingProfiles;

        private readonly IList<PromotionPhasingViewModel> _promotions;

        private readonly RangeObservableCollection<WeekPhasingProfile> _weekPhasingProfiles;

        public ApplyPhasingViewModel(Dictionary<string, string> promotionIdxs)
        {
            _promotions = promotionIdxs.Select(p => new PromotionPhasingViewModel { ID = p.Key, Name = p.Value, Selected = true }).ToList();

            var access = new PhasingAccess();
            access.GetPromoPhasingsAsync(_promotions.Select(p => p.ID))
                .ContinueWith(AssignPromoPhasings, App.Scheduler);

            _dayPhasingProfiles = new RangeObservableCollection<DayPhasingProfile>(access.GetDayProfiles(_promotions.Select(p => p.ID)));
            _weekPhasingProfiles = new RangeObservableCollection<WeekPhasingProfile>(access.GetWeekProfiles(_promotions.Select(p => p.ID)));

            _isApplying = false;

            _applyCommand = new ActionCommand(Apply, CanApply);
            _removeCommand = new ActionCommand(Remove, CanRemove);
        }

        private void AssignPromoPhasings(Task<IEnumerable<PromoPhasing>> task)
        {
            foreach (var phasing in task.Result)
            {
                var promotion = _promotions.FirstOrDefault(p => p.ID == phasing.PromotionID);
                if (promotion != null)
                {
                    promotion.DayAPhaseID = phasing.DayAPhaseID;
                    promotion.DayBPhaseID = phasing.DayBPhaseID;
                    promotion.DayCPhaseID = phasing.DayCPhaseID;
                    promotion.WeekPhaseID = phasing.WeekPhaseID;
                    promotion.PostPromoPhaseID = phasing.PostPromoPhaseID;
                }
            }

            SetSelectedDayPhase(SelectedFirstWeekDayProfileProperty, p => p.DayAPhaseID);
            SetSelectedDayPhase(SelectedOtherWeeksDayProfileProperty, p => p.DayBPhaseID);
            SetSelectedDayPhase(SelectedFinalWeekDayProfileProperty, p => p.DayCPhaseID);

            SetSelectedWeekPhase(SelectedWeekProfileProperty, p => p.WeekPhaseID);
            SetSelectedWeekPhase(SelectedPostPromoProfileProperty, p => p.PostPromoPhaseID);
        }

        private void SetSelectedDayPhase(DependencyProperty property, Func<PromotionPhasingViewModel, string> idSelector)
        {
            var promoPhases = _promotions.Select(idSelector).Distinct().ToList();
            if (promoPhases.Count == 1 && _dayPhasingProfiles != null)
            {
                SetValue(property, _dayPhasingProfiles.FirstOrDefault(p => p.ID == promoPhases[0]));
            }
        }

        private void SetSelectedWeekPhase(DependencyProperty property, Func<PromotionPhasingViewModel, string> idSelector)
        {
            var promoPhases = _promotions.Select(idSelector).Distinct().ToList();
            if (promoPhases.Count == 1 && _weekPhasingProfiles != null)
            {
                SetValue(property, _weekPhasingProfiles.FirstOrDefault(p => p.ID == promoPhases[0]));
            }
        }

        public ICommand ApplyCommand
        {
            get { return _applyCommand; }
        }

        public IList<WeekPhasingProfile> WeekPhasingProfiles
        {
            get { return _weekPhasingProfiles; }
        }

        public IList<DayPhasingProfile> DayPhasingProfiles
        {
            get { return _dayPhasingProfiles; }
        }

        public IList<PromotionPhasingViewModel> Promotions
        {
            get { return _promotions; }
        }

        public PhasingProfile SelectedWeekProfile
        {
            get { return (PhasingProfile)GetValue(SelectedWeekProfileProperty); }
            set
            {
                SetValue(SelectedWeekProfileProperty, value);
            }
        }

        public PhasingProfile SelectedFirstWeekDayProfile
        {
            get { return (PhasingProfile)GetValue(SelectedFirstWeekDayProfileProperty); }
            set { SetValue(SelectedFirstWeekDayProfileProperty, value); }
        }

        public PhasingProfile SelectedOtherWeeksDayProfile
        {
            get { return (PhasingProfile)GetValue(SelectedOtherWeeksDayProfileProperty); }
            set { SetValue(SelectedOtherWeeksDayProfileProperty, value); }
        }

        public PhasingProfile SelectedFinalWeekDayProfile
        {
            get { return (PhasingProfile)GetValue(SelectedFinalWeekDayProfileProperty); }
            set { SetValue(SelectedFinalWeekDayProfileProperty, value); }
        }

        public PhasingProfile SelectedPostPromoProfile
        {
            get { return (PhasingProfile)GetValue(SelectedPostPromoProfileProperty); }
            set
            {
                SetValue(SelectedPostPromoProfileProperty, value);
            }
        }

        public ActionCommand RemoveCommand
        {
            get { return _removeCommand; }
        }

        private void Apply()
        {
            var access = new PhasingAccess();

            IsApplying = true;

            access.ValidatePhasingAsync(_promotions.Where(p => p.Selected).Select(p => p.ID), SelectedWeekProfile.GetID(),
                                        SelectedFirstWeekDayProfile.GetID(), SelectedOtherWeeksDayProfile.GetID(),
                                        SelectedFinalWeekDayProfile.GetID(), SelectedPostPromoProfile.GetID())
                .ContinueWith(t => CheckValidationResult(t, access), App.Scheduler);
        }

        private void CheckValidationResult(Task<ValidationResult> t, PhasingAccess access)
        {
            if (t.IsFaulted) { IsApplying = false; return; }
            if (t.Result.Status == ValidationStatus.Error)
            {
                if (!string.IsNullOrWhiteSpace(t.Result.Message))
                {
                    CustomMessageBox.Show(t.Result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                IsApplying = false;
                return;
            }
            bool cont = true;
            if (t.Result.Status == ValidationStatus.Warning)
            {
                cont =
                    CustomMessageBox.Show(t.Result.Message, "Warning",
                                    MessageBoxButton.OKCancel,
                                    MessageBoxImage.Warning) ==
                    MessageBoxResult.OK;
                IsApplying = false;
            }
            if (cont)
            {
                ApplyPhasing(access);
            }
        }

        private void ApplyPhasing(PhasingAccess access)
        {
            access.ApplyPhasingAsync(_promotions.Where(p => p.Selected).Select(p => p.ID), SelectedWeekProfile.GetID(),
                                     SelectedFirstWeekDayProfile.GetID(), SelectedOtherWeeksDayProfile.GetID(),
                                     SelectedFinalWeekDayProfile.GetID(), SelectedPostPromoProfile.GetID())
                .ContinueWith(ApplyPhasingContinuation);

        }

        private void ApplyPhasingContinuation(Task<string> task)
        {
            Messages.Instance.PutInfo(task.Result);
            IsApplying = false;
        }

        private bool CanApply()
        {
            var canApply = SelectedWeekProfile != null
                           && _promotions.Any(p => p.Selected);

            if (App.Configuration.IsPhasingDailyActive)
            {
                canApply = canApply
                           && SelectedFirstWeekDayProfile != null
                           && SelectedOtherWeeksDayProfile != null
                           && SelectedFinalWeekDayProfile != null;
            }

            if (App.Configuration.IsPhasingPostActive)
            {
                canApply = canApply
                           && SelectedPostPromoProfile != null;
            }

            if (_isApplying) return false;

            return canApply;
        }

        private bool CanRemove()
        {
            return !_isApplying;
        }

        private void Remove()
        {
            var access = new PhasingAccess();
            access.RemovePhasingAsync(_promotions.Where(p => p.Selected).Select(p => p.ID))
                .ContinueWith(t =>
                {
                    SelectedFirstWeekDayProfile = null;
                    SelectedFinalWeekDayProfile = null;
                    SelectedOtherWeeksDayProfile = null;
                    SelectedPostPromoProfile = null;
                    SelectedWeekProfile = null;
                    Messages.Instance.PutInfo(t.Result);
                }, App.Scheduler);
        }

        private static void OnSelectedWeekProfileChanged(DependencyObject dependencyObject,
                                                     DependencyPropertyChangedEventArgs
                                                         dependencyPropertyChangedEventArgs)
        {
            SetSelectedPromosPhaseId(dependencyObject, vm => vm.SelectedWeekProfile.GetID(), (pvm, id) => pvm.WeekPhaseID = id);
        }

        private static void OnSelectedFirstWeekDayProfileChanged(DependencyObject dependencyObject,
                                                     DependencyPropertyChangedEventArgs
                                                         dependencyPropertyChangedEventArgs)
        {
            SetSelectedPromosPhaseId(dependencyObject, vm => vm.SelectedFirstWeekDayProfile.GetID(), (pvm, id) => pvm.DayAPhaseID = id);
        }

        private static void OnSelectedOtherWeeksDayProfileChanged(DependencyObject dependencyObject,
                                                     DependencyPropertyChangedEventArgs
                                                         dependencyPropertyChangedEventArgs)
        {
            SetSelectedPromosPhaseId(dependencyObject, vm => vm.SelectedOtherWeeksDayProfile.GetID(), (pvm, id) => pvm.DayBPhaseID = id);
        }

        private static void OnSelectedFinalWeekDayProfileChanged(DependencyObject dependencyObject,
                                                     DependencyPropertyChangedEventArgs
                                                         dependencyPropertyChangedEventArgs)
        {
            SetSelectedPromosPhaseId(dependencyObject, vm => vm.SelectedFinalWeekDayProfile.GetID(), (pvm, id) => pvm.DayCPhaseID = id);
        }

        private static void OnSelectedPostPromoProfileChanged(DependencyObject dependencyObject,
                                                     DependencyPropertyChangedEventArgs
                                                         dependencyPropertyChangedEventArgs)
        {
            SetSelectedPromosPhaseId(dependencyObject, vm => vm.SelectedPostPromoProfile.GetID(), (pvm, id) => pvm.PostPromoPhaseID = id);
        }

        private static void SetSelectedPromosPhaseId(DependencyObject dependencyObject, Func<ApplyPhasingViewModel, string> getId, Action<PromotionPhasingViewModel, string> setId)
        {
            var viewModel = ((ApplyPhasingViewModel)dependencyObject);
            viewModel._applyCommand.RaiseCanExecuteChanged();
            viewModel._removeCommand.RaiseCanExecuteChanged();
            foreach (var promo in viewModel._promotions.Where(p => p.Selected))
            {
                setId(promo, getId(viewModel));
            }
        }

        public Visibility DailyVisibility
        {
            get { return App.Configuration.IsPhasingDailyActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility PostVisibility
        {
            get { return App.Configuration.IsPhasingPostActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public void RefreshProfiles()
        {
            var access = new PhasingAccess();
            access.GetDayProfilesAsync(_promotions.Select(p => p.ID))
                  .ContinueWith(RefreshDayProfilesContinuation);

            access.GetWeekProfilesAsync(_promotions.Select(p => p.ID))
                  .ContinueWith(RefreshWeekProfilesContinuation);
        }

        readonly IEqualityComparer<PhasingProfile> _phasingProfileComparer = new PhasingProfile.EqualityComparer();

        private void RefreshWeekProfilesContinuation(Task<IEnumerable<WeekPhasingProfile>> t)
        {
            if (t.IsFaulted || t.IsCanceled) return;

            var refreshed = t.Result.ToList();

            var toAdd = refreshed.Except(_weekPhasingProfiles, _phasingProfileComparer).Cast<WeekPhasingProfile>();
            var toRemove = _weekPhasingProfiles.Except(refreshed, _phasingProfileComparer).Cast<WeekPhasingProfile>().ToList();

            _weekPhasingProfiles.AddRange(toAdd);
            foreach (var remove in toRemove)
            {
                _weekPhasingProfiles.Remove(remove);
            }
        }

        private void RefreshDayProfilesContinuation(Task<IEnumerable<DayPhasingProfile>> t)
        {
            if (t.IsFaulted || t.IsCanceled) return;
            foreach (
                var dayPhasingProfile in
                    t.Result.Where(
                        dayPhasingProfile =>
                        _dayPhasingProfiles.All(dpp => dpp.ID != dayPhasingProfile.ID)))
            {
                _dayPhasingProfiles.Add(dayPhasingProfile);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            _applyCommand.RaiseCanExecuteChanged();
        }
    }

    internal static class PhasingProfileExtensions
    {
        public static string GetID(this PhasingProfile phasingProfile)
        {
            return phasingProfile != null ? phasingProfile.ID : null;
        }
    }
}