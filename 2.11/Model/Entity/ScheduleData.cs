using Exceedra.Common.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model
{    
    public class ScheduleData
    {
        public ScheduleData()
        {
        }


        // <Item>
        //  <Idx>2</Idx>
        //  <Item_Type_Idx>0</Item_Type_Idx>
        //  <Name>(AB-1) AB-1 WalMart Only £1</Name>
        //  <CustomerName>WalMart</CustomerName>
        //  <StartDate>2013-02-11</StartDate>
        //  <EndDate>2013-03-03</EndDate>
        //  <StatusName>Complete</StatusName>
        //  <StatusColour>#AFEEEE</StatusColour>
        //</Item>


        public ScheduleData(XElement el)
        {
            ID = el.GetValue<string>("Idx");            
            Name = el.GetValue<string>("Name");
            Comments = el.GetValue<string>("Comments");
            CustomerName = el.GetValue<string>("CustomerName");
           // CustomerColor = el.Element("Customer").GetValue<string>("Colour");
            StatusName = el.GetValue<string>("StatusName");
            StatusColor = el.GetValue<string>("StatusColour");

            StartDate = el.GetValue<string>("StartDate");
            EndDate = el.GetValue<string>("EndDate");

            ItemTypeID = el.GetValue<string>("Item_Type_Idx");
            ItemType = el.GetValue<string>("Item_Type_Name");
            
            //if (el.Element("Attributes") != null)
            //{
            //    ExtraDataList = el.Element("Attributes").Elements().Select(a => new ScheduleExtraData(a)).ToList();
            //}

            //if (el.Element("ItemType") != null)
            //{
            //    ItemType = el.Element("Item_Type_Idx").Value;
            //}
        }

        #region ID
        /// <summary>
        /// Gets or sets the Id of this PromotionData.
        /// </summary>
        public string ID { get; set; }
        #endregion

        #region Name
        /// <summary>
        /// Gets or sets the Name of this PromotionData.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region Dates
        /// <summary>
        /// Gets or sets the Name of this PromotionData.
        /// </summary>
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        #endregion

        #region CustomerName
        /// <summary>
        /// Gets or sets the CustomerName of this PromotionData.
        /// </summary>
        public string CustomerName { get; set; }
        #endregion

        #region CustomerColor
        /// <summary>
        /// Gets or sets the CustomerColor of this PromotionData.
        /// </summary>
        public string CustomerColor { get; set; }
        #endregion

        #region StatusName
        /// <summary>
        /// Gets or sets the StatusName of this PromotionData.
        /// </summary>
        public string StatusName { get; set; }
        #endregion

        public string ItemType { get; set; }
        public string ItemTypeID { get; set; }

        #region StatusColor
        /// <summary>
        /// Gets or sets the StatusColor of this PromotionData.
        /// </summary>
        public string StatusColor { get; set; }
        #endregion

        //#region ExtraData  
        ///// <summary>
        ///// Gets or sets the Attributes of this PromotionData.
        ///// </summary>
        //private List<ScheduleExtraData> _extraDataList;
        //public List<ScheduleExtraData> ExtraDataList
        //{
        //    get
        //    {
        //        return _extraDataList;
        //    }
        //    set
        //    {
        //        if (_extraDataList != value)
        //        {
        //            _extraDataList = value.ToList();
        //        }
        //    }
        //}
        //#endregion       

        #region Comments
        /// <summary>
        /// Gets or sets the Comments of this PromotionData.
        /// </summary>
        public string Comments { get; set; }
        #endregion

        public bool IsSelected { get; set; }
    }

  
        [Serializable()]
        public class ScheduleExtraData
        {
            public ScheduleExtraData()
            {

            }
            public ScheduleExtraData(XElement el)
            {
                Name = el.GetValue<string>("Name");
                _value = el.GetValue<string>("Value");
                Format = el.GetValue<string>("Format");

                // formating the value
                _value = this.InvariantFormatValue(el.GetValue<string>("Value"));
            }

            private string FormatValue(string value)
            {
                if (!string.IsNullOrWhiteSpace(Format))
                {
                    decimal d;
                    if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out d))
                    {
                        if (Format.StartsWith("P"))
                        {
                            return (d / 100).ToString(Format);
                        }
                        else
                        {
                            return d.ToString(Format);
                        }
                    }
                    DateTime date;
                    if (DateTime.TryParse(value, out date))
                    {
                        return date.ToString(Format);
                    }
                }

                return value;
            }

            private string FormatValue(decimal value)
            {
                var result = (value).ToString(Format, CultureInfo.CurrentCulture.NumberFormat);
                return result;
            }

            private string InvariantFormatValue(string value)
            {

                if (value == null)
                {
                    //Trace.TraceError(ex.Message);

                    Messages.Instance.PutError(string.Format("An error was found in your XML formatting, the promotion attributed with '{0}'  may load incorrectly", this.Name));

                    return null;
                }

                decimal d;
                DateTime dateValue = DateTime.MinValue;
                string returnValue = value;
                if (decimal.TryParse(value.Replace("%", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                {
                    returnValue = FormatValue(d);
                }
                else if (DateTime.TryParse(value, out dateValue))
                {
                    returnValue = FormatValue(dateValue.ToString());
                }
                return returnValue;
            }

            #region Name
            /// <summary>
            /// Gets or sets the Text of this PromotionExtraData.
            /// </summary>
            public string Name { get; set; }
            #endregion

            #region Value

            private string _value;

            /// <summary>
            /// Gets or sets the Value of this PromotionExtraData.
            /// </summary>
            public string Value
            {
                get { return _value; }
                set { _value = FormatValue(value); }
            }

            #endregion

            public IComparable SortValue
            {
                get
                {
                    decimal d;
                    if (decimal.TryParse(Value, out d))
                    {
                        return d;
                    }
                    DateTime date;
                    if (DateTime.TryParse(Value, out date))
                    {
                        return date;
                    }
                    if (Value == "Not Set") return null;
                    return Value;

                }
            }

            #region Format
            /// <summary>
            /// Gets or sets the Format of this PromotionExtraData.
            /// </summary>
            public string Format { get; set; }
            #endregion

        }
    }
 
