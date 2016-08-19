using System;
using Model;

namespace ViewModels
{
    public class PromotionDateViewModel : ViewModelBase
    {
        private readonly PromotionDate _promotionDate;

        public PromotionDateViewModel()
        {
        }

        public PromotionDateViewModel(PromotionDate promotionDate)
        {
            _promotionDate = promotionDate;
        }

        public PromotionDateViewModel(PromotionDate promotionDate, bool stopOffset)
        {
            _promotionDate = promotionDate;
            _promotionDate.StopOffset = stopOffset;
        }

        private bool _hasDateChanges;

        public bool HasDateChanges
        {
            get { return _hasDateChanges; }
            set
            {
                _hasDateChanges = value;
                NotifyPropertyChanged(this, vm => vm.HasDateChanges);
            }
        }

        public bool IsEditable
        {
            get { return _promotionDate.IsEditable; }

        }

        public bool IsValid
        {
            get { return StartDate <= EndDate; }
        }

        public string ID
        {
            get { return _promotionDate.ID; }
        }

        public string Description
        {
            get { return _promotionDate.Description; }
        }

        public DateTime StartDate
        {
            get { return _promotionDate.StartDate; }
            set
            {
                // SetStartAndEndDates(value, value.AddDays(_promotionDate.OffsetDays));
                SetStartDate(value);
            }
        }

        public DateTime EndDate
        {
            get { return _promotionDate.EndDate; }
            set
            {
                // var laterOfStartDateAndInputValue = value < StartDate ? StartDate : value;
                // SetStartAndEndDates(StartDate, laterOfStartDateAndInputValue);

                SetStartAndEndDates(StartDate, value);
            }
        }

        public PromotionDate GetModel()
        {
            return _promotionDate;
        }

        public DateTime? EarliestStartDate
        {
            get { return _promotionDate.EarliestStartDate; }
        }

        public DateTime? EarliestEndDate
        {
            get { return _promotionDate.EarliestStartDate; }
        }


        public void SetStartDate(DateTime startDate)
        {
            if (_promotionDate.StopOffset == false)
            {
                SetStartAndEndDates(startDate, startDate.AddDays(_promotionDate.OffsetDays));
            }
            else
            {
                var earliestStartDate = EarliestStartDate.GetValueOrDefault(DateTime.MinValue);

                _promotionDate.StartDate =
                    startDate < earliestStartDate ? earliestStartDate : startDate;

                NotifyPropertyChanged(this,
                    vm => vm.StartDate);

                HasDateChanges = true;
            }
        }

        public void SetStartAndEndDates(DateTime startDate, DateTime endDate)
        {
            var esd = EarliestStartDate.GetValueOrDefault(DateTime.MinValue);
            var earliestStartDate = esd;
            if (earliestStartDate != esd)
            {
                NotifyPropertyChanged(this, vm => vm.EarliestStartDate);
                HasDateChanges = true;
            }

            var eed = EarliestEndDate.GetValueOrDefault(DateTime.MinValue);
            var earliestEndDate = eed;
            if (earliestEndDate != eed)
            {
                NotifyPropertyChanged(this, vm => vm.EarliestEndDate);
                HasDateChanges = true;
            }


            var sd = startDate < earliestStartDate ? earliestStartDate : startDate;
            if (_promotionDate.StartDate != sd)
            {
                _promotionDate.StartDate = sd;
                NotifyPropertyChanged(this, vm => vm.StartDate);
                HasDateChanges = true;
            }


            var ed = endDate < earliestEndDate ? earliestEndDate : endDate;
            if (_promotionDate.EndDate != ed)
            {
                _promotionDate.EndDate = ed;
                NotifyPropertyChanged(this, vm => vm.EndDate);
                HasDateChanges = true;
            }

            //PBI:3766 dont offset the end date, just leave it as it is entered and leave the user to change manually 
        }
    }
}
