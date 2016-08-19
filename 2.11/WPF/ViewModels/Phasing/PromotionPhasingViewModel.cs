using Exceedra.Controls.DynamicGrid.ViewModels;
using System.Linq;
using System.Xml.Linq;
using ViewModels;

namespace WPF.ViewModels.Phasing
{
    public class PromotionPhasingViewModel : ViewModelBase
    {
        public PromotionPhasingViewModel()
        {
            Load();
        }

        private void Load()
        {
            LoadWeekly();
            //LoadDaily();
        }

        private void LoadWeekly()
        {
            var xml = new XElement("Rows");

            var fullRow1 = @"
<Row Idx=""10"" Code=""WEEK 1"" Name=""WEEK 1"" IsDisplayed=""1""><Columns><Column Idx=""01"" Width=""100"" Name=""Week Split"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.1"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""02"" Width=""100"" Name=""Week Volume"" IsEditable=""1"" IsDisplayed=""1"" Value=""1000"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""03"" Width=""150"" Name=""Daily Profile"" IsEditable=""1"" IsDisplayed=""1"" Value=""-"" Format=""N2"" ControlType=""Dropdown"" Alignment=""Center""/><Column Idx=""04"" Width=""30"" Name=""M"" IsEditable=""1"" IsDisplayed=""1"" Value=""0"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""05"" Width=""30"" Name=""T"" IsEditable=""1"" IsDisplayed=""1"" Value=""0"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""06"" Width=""30"" Name=""W"" IsEditable=""1"" IsDisplayed=""1"" Value=""0"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""07"" Width=""30"" Name=""T"" IsEditable=""1"" IsDisplayed=""1"" Value=""0"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""08"" Width=""30"" Name=""F"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.2"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""09"" Width=""30"" Name=""S"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.4"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""10"" Width=""30"" Name=""S"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.4"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""11"" Name=""M Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""0"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""12"" Name=""T Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""0"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""13"" Name=""W Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""0"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""14"" Name=""T Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""0"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""15"" Name=""F Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""200"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""16"" Name=""S Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""400"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""17"" Name=""S Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""400"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/></Columns></Row>";

            var fullRow2 = @"<Row Idx=""10"" Code=""WEEK 2"" Name=""WEEK 2"" IsDisplayed=""1""><Columns><Column Idx=""01"" Width=""100"" Name=""Week Split"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.4"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""02"" Width=""100"" Name=""Week Volume"" IsEditable=""1"" IsDisplayed=""1"" Value=""4000"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""03"" Width=""150"" Name=""Daily Profile"" IsEditable=""1"" IsDisplayed=""1"" Value=""-"" Format=""N2"" ControlType=""Dropdown"" Alignment=""Center""/><Column Idx=""04"" Width=""30"" Name=""M"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.07"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""05"" Width=""30"" Name=""T"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.08"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""06"" Width=""30"" Name=""W"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.1"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""07"" Width=""30"" Name=""T"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.25"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""08"" Width=""30"" Name=""F"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.2"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""09"" Width=""30"" Name=""S"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.2"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""10"" Width=""30"" Name=""S"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.1"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""11"" Name=""M Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""280"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""12"" Name=""T Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""320"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""13"" Name=""W Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""400"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""14"" Name=""T Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""1000"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""15"" Name=""F Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""800"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""16"" Name=""S Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""800"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""17"" Name=""S Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""400"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/></Columns></Row>";

            var fullRow3 = @"<Row Idx=""10"" Code=""WEEK 3"" Name=""WEEK 3"" IsDisplayed=""1""><Columns><Column Idx=""01"" Width=""100"" Name=""Week Split"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.4"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""02"" Width=""100"" Name=""Week Volume"" IsEditable=""1"" IsDisplayed=""1"" Value=""4000"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""03"" Width=""150"" Name=""Daily Profile"" IsEditable=""1"" IsDisplayed=""1"" Value=""-"" Format=""N2"" ControlType=""Dropdown"" Alignment=""Center""/><Column Idx=""04"" Width=""30"" Name=""M"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.07"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""05"" Width=""30"" Name=""T"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.08"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""06"" Width=""30"" Name=""W"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.1"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""07"" Width=""30"" Name=""T"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.25"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""08"" Width=""30"" Name=""F"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.2"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""09"" Width=""30"" Name=""S"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.2"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""10"" Width=""30"" Name=""S"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.1"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""11"" Name=""M Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""280"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""12"" Name=""T Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""320"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""13"" Name=""W Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""400"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""14"" Name=""T Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""1000"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""15"" Name=""F Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""800"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""16"" Name=""S Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""800"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""17"" Name=""S Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""400"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/></Columns></Row>";

            var fullRow4 = @"<Row Idx=""10"" Code=""WEEK 4"" Name=""WEEK 4"" IsDisplayed=""1""><Columns><Column Idx=""01"" Width=""100"" Name=""Week Split"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.1"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""02"" Width=""100"" Name=""Week Volume"" IsEditable=""1"" IsDisplayed=""1"" Value=""1000"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""03"" Width=""150"" Name=""Daily Profile"" IsEditable=""1"" IsDisplayed=""1"" Value=""-"" Format=""N2"" ControlType=""Dropdown"" Alignment=""Center""/><Column Idx=""04"" Width=""30"" Name=""M"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.25"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""05"" Width=""30"" Name=""T"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.5"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""06"" Width=""30"" Name=""W"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.1"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""07"" Width=""30"" Name=""T"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.15"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""08"" Width=""30"" Name=""F"" IsEditable=""1"" IsDisplayed=""1"" Value=""0"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""09"" Width=""30"" Name=""S"" IsEditable=""1"" IsDisplayed=""1"" Value=""0"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""10"" Width=""30"" Name=""S"" IsEditable=""1"" IsDisplayed=""1"" Value=""0"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""11"" Name=""M Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""250"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""12"" Name=""T Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""500"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""13"" Name=""W Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""100"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""14"" Name=""T Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""150"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""15"" Name=""F Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""0"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""16"" Name=""S Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""0"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/><Column Idx=""17"" Name=""S Vol."" IsEditable=""1"" IsDisplayed=""1"" Value=""0"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/></Columns></Row>";
//            var splitRow = @"<Row Idx=""10"" Code=""Splits"" Name=""Splits"" IsDisplayed=""1""><Columns>
//<Column Idx=""01"" Width=""210"" Name=""Week 1"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.1"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/>
//<Column Idx=""02"" Width=""210"" Name=""Week 2"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.4"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/>
//<Column Idx=""03"" Width=""210"" Name=""Week 3"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.4"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/>
//<Column Idx=""04"" Width=""210"" Name=""Week 4"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.1"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/>
//</Columns></Row>";

            //            var volumeRow = @"<Row Idx=""20"" Code=""Volume"" Name=""Volume"" IsDisplayed=""1""><Columns>
            //<Column Idx=""01"" Width=""210"" Name=""Week 1"" IsEditable=""1"" IsDisplayed=""1"" Value=""1000"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/>
            //<Column Idx=""02"" Width=""210"" Name=""Week 2"" IsEditable=""1"" IsDisplayed=""1"" Value=""4000"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/>
            //<Column Idx=""03"" Width=""210"" Name=""Week 3"" IsEditable=""1"" IsDisplayed=""1"" Value=""4000"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/>
            //<Column Idx=""04"" Width=""210"" Name=""Week 4"" IsEditable=""1"" IsDisplayed=""1"" Value=""1000"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Center""/>
            //</Columns></Row>";

            //            var dailySplitRow = @"<Row Idx=""30"" Code=""DailyProfiles"" Name=""Daily Profiles"" IsDisplayed=""1""><Columns>
            //<Column Idx=""01"" Width=""100"" Name=""Week 1"" IsEditable=""1"" IsDisplayed=""1"" Value=""-"" Format=""N2"" ControlType=""dropdown"" Alignment=""Right""/>
            //<Column Idx=""02"" Width=""100"" Name=""Week 2"" IsEditable=""1"" IsDisplayed=""1"" Value=""-"" Format=""N2"" ControlType=""dropdown"" Alignment=""Right""/>
            //<Column Idx=""03"" Width=""100"" Name=""Week 3"" IsEditable=""1"" IsDisplayed=""1"" Value=""-"" Format=""N2"" ControlType=""dropdown"" Alignment=""Right""/>
            //<Column Idx=""04"" Width=""100"" Name=""Week 4"" IsEditable=""1"" IsDisplayed=""1"" Value=""-"" Format=""N2"" ControlType=""dropdown"" Alignment=""Right""/>
            //</Columns></Row>";

            xml.Add(XElement.Parse(fullRow1));
            xml.Add(XElement.Parse(fullRow2));
            xml.Add(XElement.Parse(fullRow3));
            xml.Add(XElement.Parse(fullRow4));

            //xml.Add(XElement.Parse(volumeRow));
            //xml.Add(XElement.Parse(dailySplitRow));

            var weeklyVM = RecordViewModel.LoadWithNewXml(xml);

            weeklyVM.Records.SelectMany(r => r.Properties).Where(p => p.Value == "0.00").Do(p => p.BackgroundColour = "#eeCCCCCC");

            int i = 0;
            weeklyVM.Records.SelectMany(r => r.Properties.Where(p => p.ControlType.ToLower() == "dropdown")).Do(p =>
            {
                p.Values = new System.Collections.ObjectModel.ObservableCollection<Exceedra.DynamicGrid.Models.Option>
                {
                    new Exceedra.DynamicGrid.Models.Option { IsSelected = i == 0, Item_Idx = "1", Item_Name = "Phase-In Profile", Background = "#33FF1111" },
                    new Exceedra.DynamicGrid.Models.Option { IsSelected = i == 1 || i == 2, Item_Idx = "2", Item_Name = "Mid Promo Profile", Background = "#3311FF11" },
                    new Exceedra.DynamicGrid.Models.Option { IsSelected = i == 3, Item_Idx = "3", Item_Name = "Phase-Out Profile", Background = "#331111FF" }
                };
                i++;
            });

            WeeklyPhasingGrid = weeklyVM;
        }

        private void LoadDaily()
        {
            var xml = new XElement("Rows");

            var dailySplitRow = @"<Row Idx=""30"" Code=""DailyProfiles"" Name=""Daily Profiles"" IsDisplayed=""1""><Columns>
<Column Idx=""01"" Width=""210"" Name=""Week 1"" IsEditable=""1"" IsDisplayed=""1"" Value=""-"" Format=""N2"" ControlType=""dropdown"" Alignment=""Right""/>
<Column Idx=""02"" Width=""210"" Name=""Week 2"" IsEditable=""1"" IsDisplayed=""1"" Value=""-"" Format=""N2"" ControlType=""dropdown"" Alignment=""Right""/>
<Column Idx=""03"" Width=""210"" Name=""Week 3"" IsEditable=""1"" IsDisplayed=""1"" Value=""-"" Format=""N2"" ControlType=""dropdown"" Alignment=""Right""/>
<Column Idx=""04"" Width=""210"" Name=""Week 4"" IsEditable=""1"" IsDisplayed=""1"" Value=""-"" Format=""N2"" ControlType=""dropdown"" Alignment=""Right""/>
</Columns></Row>";

            xml.Add(XElement.Parse(dailySplitRow));

            var dailyVM = RecordViewModel.LoadWithNewXml(xml);

            dailyVM.Records.SelectMany(r => r.Properties.Where(p => p.ControlType.ToLower() == "dropdown")).Do(p =>
            {
                p.Values = new System.Collections.ObjectModel.ObservableCollection<Exceedra.DynamicGrid.Models.Option>
                {
                    new Exceedra.DynamicGrid.Models.Option { IsSelected = true, Item_Idx = "1", Item_Name = "Phase-In Profile", Background = "#33FF1111" },
                    new Exceedra.DynamicGrid.Models.Option { IsSelected = false, Item_Idx = "2", Item_Name = "Mid Promo Profile", Background = "#3311FF11" },
                    new Exceedra.DynamicGrid.Models.Option { IsSelected = false, Item_Idx = "3", Item_Name = "Phase-Out Profile", Background = "#331111FF" }
                };
            });

            DailyProfileGrid = dailyVM;

            //Load the split and volumes grid:

            var xml2 = new XElement("Rows");

            var dailySplitRow2 = @"<Row Idx=""10"" Code=""DailySplits"" Name=""Daily Splits"" IsDisplayed=""1"">
<Columns>
<Column Idx=""01"" Width=""30"" Name=""M"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.14"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Right""/>
<Column Idx=""02"" Width=""30"" Name=""T"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.14"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Right""/>
<Column Idx=""03"" Width=""30"" Name=""W"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.14"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Right""/>
<Column Idx=""04"" Width=""30"" Name=""T"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.14"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Right""/>
<Column Idx=""05"" Width=""30"" Name=""F"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.14"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Right""/>
<Column Idx=""06"" Width=""30"" Name=""S"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.14"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Right""/>
<Column Idx=""07"" Width=""30"" Name=""S"" IsEditable=""1"" IsDisplayed=""1"" Value=""0.14"" Format=""N2"" ControlType=""TreeGridCell"" Alignment=""Right""/>
</Columns></Row>";

            xml2.Add(XElement.Parse(dailySplitRow2));

            var dailyVM2 = RecordViewModel.LoadWithNewXml(xml2);

            DailyPhasingGrid = dailyVM2;
        }

        private RecordViewModel _weeklyPhasingGrid;
        public RecordViewModel WeeklyPhasingGrid
        {
            get { return _weeklyPhasingGrid; }
            set
            {
                _weeklyPhasingGrid = value;
                NotifyPropertyChanged(this, vm => vm.WeeklyPhasingGrid);
            }
        }

        private RecordViewModel _dailyProfileGrid;
        public RecordViewModel DailyProfileGrid
        {
            get { return _dailyProfileGrid; }
            set
            {
                _dailyProfileGrid = value;
                NotifyPropertyChanged(this, vm => vm.DailyProfileGrid);
            }
        }

        private RecordViewModel _dailyPhasingGrid;
        public RecordViewModel DailyPhasingGrid
        {
            get { return _dailyPhasingGrid; }
            set
            {
                _dailyPhasingGrid = value;
                NotifyPropertyChanged(this, vm => vm.DailyPhasingGrid);
            }
        }
    }
}
