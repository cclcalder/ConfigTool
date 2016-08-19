using System;
using Model;

namespace ViewModels
{
    public class TemplateDateViewModel : ViewModelBase
    {
        private readonly PromotionDate _promotionDate;

        public TemplateDateViewModel() { }

        public TemplateDateViewModel(PromotionDate promotionDate)
        {
            _promotionDate = promotionDate;
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
            set { SetStartAndEndDates(value, value.AddDays(_promotionDate.OffsetDays)); }
        }

        public DateTime EndDate
        {
            get { return _promotionDate.EndDate; }
            set
            {
                // var laterOfStartDateAndInputValue = value < StartDate ? StartDate : value;
                SetStartAndEndDates(StartDate, value);
            }
        }

        public bool IsValid
        {
            get { return StartDate <= EndDate; }
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
            get { return _promotionDate.EarliestEndDate; }
        }

        private void SetStartAndEndDates(DateTime startDate, DateTime endDate)
        {
            // CorrectDates(startDate, endDate);

            _promotionDate.StartDate = startDate;
            _promotionDate.EndDate = endDate;

            NotifyPropertyChanged(this,
                vm => vm.StartDate,
                vm => vm.EndDate,
                vm => vm.EarliestStartDate,
                vm => vm.EarliestEndDate);

            HasDateChanges = true;
        }

        /// <summary>
        /// Changes the start date to earliest start date if first is earlier than second. Works analogical with the end date.
        /// </summary>
        /// <param name="startDate">If you don't specify this value, currently assigned _promotionDate.StartDate will be taken</param>
        /// <param name="endDate">If you don't specify this value, currently assigned _promotionDate.EndDate will be taken</param>
        public void CorrectDates(DateTime startDate = new DateTime(), DateTime endDate = new DateTime())
        {
            if (startDate == DateTime.MinValue) startDate = StartDate;
            if (endDate == DateTime.MinValue) endDate = EndDate;

            var earliestStartDate = EarliestStartDate.GetValueOrDefault(DateTime.MinValue);
            var earliestEndDate = EarliestEndDate.GetValueOrDefault(DateTime.MinValue);

            _promotionDate.StartDate =
                startDate < earliestStartDate ? earliestStartDate : startDate;

            _promotionDate.EndDate =
                endDate < earliestEndDate ? earliestEndDate : endDate;
        }
    }
}