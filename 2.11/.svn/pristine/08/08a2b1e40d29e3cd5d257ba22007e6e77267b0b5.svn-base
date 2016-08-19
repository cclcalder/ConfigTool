using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Schedule.Model;
using Telerik.Windows.Controls;

namespace Exceedra.Schedule.ViewModels
{
    public class ScheduleViewModel : ViewModelBase
    {
        public ScheduleViewModel(XElement xmlIn)
        {
            TimelineItems = LoadSchedule(xmlIn);
            FilterVisibleItems();
        }

        public ScheduleViewModel() { }

        private List<ScheduleItem> LoadSchedule(XElement xmlIn)
        {
            /* Special case for wacky schedule page xml */
            var nodes = xmlIn.Descendants("RootItem");
            if (!nodes.Any())
                nodes = xmlIn.Descendants("Item");

            var scheduleItems = nodes.Select(root => new ScheduleItem(root)).Where(s => s.Idx != "-1" && !String.IsNullOrWhiteSpace(s.Name)).ToList();
            var sellOutItems = nodes.Select(root => new ScheduleItem(root, true)).Where(s => s.Idx != "-1" && !String.IsNullOrWhiteSpace(s.Name) && s.HasSellOut).ToList();

            if (sellOutItems != null)
            {
                var ids = sellOutItems.Select(t => t.Idx).ToList();

                scheduleItems.Where(t => ids.Contains(t.Idx)).Do(t =>
                {
                    t.SellOutStartDate = null;
                    t.SellOutEndDate = null;
                });

                scheduleItems.AddRange(sellOutItems);
            }

            if (scheduleItems.Any())
            {
                var minStartDate = scheduleItems.Where(item => item.StartDate != new DateTime()).Min(item => item.StartDate);
                minStartDate = minStartDate > DateTime.MinValue.AddDays(7) ? minStartDate.AddDays(-7) : DateTime.MinValue;

                var maxStartDate = scheduleItems.Max(item => item.EndDate);
                maxStartDate = maxStartDate < DateTime.MaxValue.AddDays(-7) ? maxStartDate.AddDays(7) : DateTime.MinValue;

                StartDate = minStartDate;
                EndDate = maxStartDate;
            }

            return scheduleItems;
        }

        private void FilterVisibleItems()
        {
            VisibleTimelineItems = TimelineItems.Where(t => MatchesFilter(t)).ToList();
        }

        public void SwitchVisibleItems(int flag)
        {
            switch (flag)
            {
                case 1: // buy in
                    VisibleTimelineItems = TimelineItems.Where(t => MatchesFilter(t)).Where(t => t.HasSellOut == false).ToList();
                    break;
                case 2: //sell out
                    VisibleTimelineItems = TimelineItems.Where(t => MatchesFilter(t)).Where(t => t.HasSellOut == true).ToList();
                    break;

                default: // all
                    FilterVisibleItems();
                    break;
            }

        }

        private bool MatchesFilter(ScheduleItem item)
        {
            return Filter == _filterWatermark
                || String.IsNullOrWhiteSpace(Filter)
                || item.Name.Contains(Filter)
                || item.ScheduleType.Contains(Filter)
                || (item.TooltipContent1 != null && item.TooltipContent1.Contains(Filter))
                || (item.TooltipContent2 != null && item.TooltipContent2.Contains(Filter))
                || (item.TooltipContent3 != null && item.TooltipContent3.Contains(Filter))
                || item.Status.Contains(Filter)
                || (item.Status != null && item.Status.Contains(Filter))
                || item.Category.Contains(Filter);
        }

        #region Properties

        private List<ScheduleItem> _timelineItems;
        public List<ScheduleItem> TimelineItems
        {
            get { return _timelineItems; }
            set
            {
                _timelineItems = value;
                OnPropertyChanged("TimelineItems");
            }
        }

        private List<ScheduleItem> _visibleTimelineItems;
        public List<ScheduleItem> VisibleTimelineItems
        {
            get { return _visibleTimelineItems; }
            set
            {
                _visibleTimelineItems = value;
                OnPropertyChanged("VisibleTimelineItems");
            }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged("StartDate");
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged("EndDate");
            }
        }

        public DateTime VisibleDateTo
        {
            get { return StartDate.AddMonths(3); }
        }

        public string Title { get; set; }


        private string _filterWatermark = "Filter...";
        private string _filter = "Filter...";
        public string Filter
        {
            get { return _filter; }
            set
            {
                if (value != _filter)
                {
                    _filter = value;
                    FilterVisibleItems();
                }
            }
        }

        #endregion

    }
}