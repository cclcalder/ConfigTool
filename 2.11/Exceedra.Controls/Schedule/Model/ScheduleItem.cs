using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Xml.Linq;
using Exceedra.Common;
using Model;

namespace Exceedra.Schedule.Model
{
    public class ScheduleItem
    {
        public ScheduleItem()
        {
            Colour = new SolidColorBrush(Colors.Gray);
        }

        public ScheduleItem(XElement root, bool swapDates = false)
            : this()
        {
            LoadAttributes(root, swapDates);
        }

        private void LoadAttributes(XElement root, bool swapDates = false)
        {
            Idx = root.Element("Idx").MaybeValue() ?? root.Element("Item_Idx").MaybeValue();
            ScheduleType = root.Element("Item_Type").MaybeValue() ?? root.Element("Item_Type_Name").MaybeValue();
            AppTypeIdx = root.Element("Item_Type_Idx").MaybeValue();

            if (root.Descendants("Attribute").Any())
                LoadFromDynamicGridXml(root);
            else /* Special case for wacky schedule page xml */
            {
                LoadForScheduleScreen(root);
            }

            if (swapDates)
            {
                if (SellOutStartDate != null) StartDate = SellOutStartDate.Value;
                if (SellOutEndDate != null) EndDate = SellOutEndDate.Value;
                Name += " - SellOut";
            }

        }

        private void LoadFromDynamicGridXml(XElement xmlIn)
        {
            foreach (var attribute in xmlIn.Descendants("Attribute"))
            {
                if (attribute.Element("Value") == null) continue;

                var value = attribute.Element("Value").MaybeValue().Trim();

                SetProperty(attribute.Element("ColumnCode").MaybeValue(), value, attribute);
            }
        }

        private void LoadForScheduleScreen(XElement xmlIn)
        {
            foreach (var item in xmlIn.Descendants())
            {
                if (item.MaybeValue() == null) continue;

                var value = item.MaybeValue().Trim();

                SetProperty(item.Name.ToString(), value);
            }
        }

        private void SetProperty(string propertyName, string value, XElement attribute = null)
        {
            switch (propertyName)
            {
                case "Name":
                case "ROB_Name":
                case "Fund_Name":
                case "Scen_Name":
                case "Promo_Name":
                    Name = value;
                    break;
                case "StartDate":
                case "Start_Date":
                case "Start_Day":
                case "BuyIn_Start":
                    try
                    {
                        StartDate = Convert.ToDateTime(value);
                    }
                    catch (Exception) { }
                    break;
                case "SellOutStartDate":
                    try
                    {
                        SellOutStartDate = Convert.ToDateTime(value);
                    }
                    catch (Exception) { }
                    break;
                case "EndDate":
                case "End_Date":
                case "End_Day":
                case "BuyIn_End":
                    try
                    {
                        EndDate = Convert.ToDateTime(value);
                    }
                    catch (Exception) { }
                    break;
                case "SellOutEndDate":
                    try
                    {
                        SellOutEndDate = Convert.ToDateTime(value);
                    }
                    catch (Exception) { }
                    break;
                case "TooltipContent1":
                case "Customer_Name":
                case "CustomerName":
                    TooltipContent1 = value;
                    Category = value;
                    break;
                case "TooltipContent2":
                case "SubType":
                    TooltipContent2 = value;
                    break;
                case "TooltipContent3":
                    TooltipContent3 = value;
                    break;
                case "Colour":
                case "StatusColour":
                    Colour = (SolidColorBrush)new BrushConverter().ConvertFromString(value);
                    break;
                case "Status":
                case "StatusName":
                    //Colour = (SolidColorBrush)new BrushConverter().ConvertFromString(attribute.Element("BorderColour").MaybeValue());
                    Status = value;
                    break;
                //Special Cases
                case "Status_Name":
                    TooltipContent3 = value;
                    Status = value;
                    Colour = (SolidColorBrush)new BrushConverter().ConvertFromString(attribute.Element("BorderColour").MaybeValue());
                    break;
                case "IsEditable":
                    CanEditItem = (value == "1");
                    break;
            }
        }

        public string Status { get; set; }

        public string Name { get; set; }

        public string ScheduleType { get; set; }

        public string AppTypeIdx { get; set; }

        public string Idx { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime? SellOutStartDate { get; set; }

        public DateTime? SellOutEndDate { get; set; }

        public bool IsSelected { get; set; }

        public bool CanEditItem { get; set; }

        public bool HasSellOut
        {
            get { return SellOutDuration.TotalDays > 0; }
        }

        public TimeSpan Duration
        {
            get
            {
                List<TimeSpan> timeSpans = new List<TimeSpan>
                {
                    new TimeSpan(1,0,0,0),
                    EndDate - StartDate
                };

                return timeSpans.Max();
            }
        }

        public TimeSpan SellOutDuration
        {
            get
            {
                if (SellOutStartDate != null && SellOutEndDate != null)
                {
                    List<TimeSpan> timeSpans = new List<TimeSpan>
                    {
                        new TimeSpan(0,0,0,0),
                        SellOutEndDate.Value - SellOutStartDate.Value
                    };

                    return timeSpans.Max();
                }
                else
                {
                    return new TimeSpan(0, 0, 0, 0);
                }
            }
        }

        public string DurationString
        {
            get { return Duration.Days + " Day" + (Duration.Days != 1 ? "s" : ""); }
        }

        public string TooltipContent1 { get; set; }
        public string TooltipContent2 { get; set; }
        public string TooltipContent3 { get; set; }

        public SolidColorBrush Colour { get; set; }

        public string Category { get; set; }

    }
}