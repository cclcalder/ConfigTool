 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
 
using System.Xml.Linq;

namespace Exceedra.Common.Utilities
{
  public  class DynamicDataView : INotifyPropertyChanged
    {

#region "Dummy"
  


      public static string DummyData = @"
<Results>
  <RootItem>
    <Item_Type>Scenario</Item_Type>
    <Item_Idx>1</Item_Idx>
    <Item_RowSortOrder>1</Item_RowSortOrder>
    <Attributes>
      <Attribute>
        <ColumnCode>Scen_Name</ColumnCode>
        <HeaderText>Name</HeaderText>
        <Value>(LIVE) - Live</Value>
        <Format />
        <ForeColour>#0000FF</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>1</IsHyperlink>
        <TotalsAggregationMethod>COUNT</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>Author</ColumnCode>
        <HeaderText>Author</HeaderText>
        <Value>Test User, Exceedra</Value>
        <Format />
        <ForeColour>#006633</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>COUNT</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>Start_Day</ColumnCode>
        <HeaderText>Start_Day</HeaderText>
        <Value>2014-02-14</Value>
        <Format>LongDate</Format>
        <ForeColour>#660000</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>End_Day</ColumnCode>
        <HeaderText>End_Day</HeaderText>
        <Value>2015-02-14</Value>
        <Format>ShortDate</Format>
        <ForeColour>#66FFFF</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>Create_Day</ColumnCode>
        <HeaderText>Create_Day</HeaderText>
        <Value>2014-07-17</Value>
        <Format>dd-mmm-yyyy</Format>
        <ForeColour>#990000</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>Status_Name</ColumnCode>
        <HeaderText>Status</HeaderText>
        <Value>Closed</Value>
        <Format />
        <ForeColour>#FFFFFF</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>Num_Customers</ColumnCode>
        <HeaderText>Number Of Customers</HeaderText>
        <Value>31</Value>
        <Format>N0</Format>
        <ForeColour>#996600</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>SUM</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
       <ColumnCode>Num_Products</ColumnCode>
        <HeaderText>Number Of Products</HeaderText>
        <Value>47</Value>
        <Format>N2</Format>
        <ForeColour>#99CC00</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>SUM</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>Num_Promotions</ColumnCode>
        <HeaderText>Number Of Promotions</HeaderText>
        <Value>0</Value>
        <Format>N4</Format>
        <ForeColour>#99CC00</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>AVG</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>Num_Terms</ColumnCode>
        <HeaderText>Number Of Terms</HeaderText>
        <Value>4</Value>
        <Format>N8</Format>
        <ForeColour>#9999CC</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>MIN</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>CalcTotalItems</ColumnCode>
        <HeaderText>Total Linked Items</HeaderText>
        <Value>=Num_Customers + Num_Products + Num_Promotions + Num_Terms</Value>
        <Format>N8</Format>
        <ForeColour>#FFFFFF</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>MAX</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>CalcTwiceTotalItems</ColumnCode>
        <HeaderText>Twice Linked Items</HeaderText>
        <Value>=CalcTotalItems * 2</Value>
        <Format>N8</Format>
        <ForeColour>#FFFFFF</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>MAX</TotalsAggregationMethod>
      </Attribute>
    </Attributes>
  </RootItem>
  <RootItem>
    <Item_Idx>3</Item_Idx>
    <RowSortOrder>3</RowSortOrder>
    <Attributes>
      <Attribute>
        <ColumnCode>Scen_Name</ColumnCode>
        <HeaderText>Name</HeaderText>
        <Value>(PVB-8) - Budget 2014</Value>
        <Format />
        <ForeColour>#0000FF</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>1</IsHyperlink>
        <TotalsAggregationMethod>COUNT</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>Author</ColumnCode>
        <HeaderText>Author</HeaderText>
        <Value>Van Bosch, Peter</Value>
        <Format />
        <ForeColour>#006633</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>COUNT</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>Start_Day</ColumnCode>
        <HeaderText>Start_Day</HeaderText>
        <Value>2014-01-01</Value>
        <Format>LongDate</Format>
        <ForeColour>#660000</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>End_Day</ColumnCode>
        <HeaderText>End_Day</HeaderText>
        <Value>2014-12-31</Value>
        <Format>ShortDate</Format>
        <ForeColour>#66FFFF</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>Create_Day</ColumnCode>
        <HeaderText>Create_Day</HeaderText>
        <Value>2014-07-17</Value>
        <Format>dd-mmm-yyyy</Format>
        <ForeColour>#990000</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>Status_Name</ColumnCode>
        <HeaderText>Status</HeaderText>
        <Value>Active Budget</Value>
        <Format />
        <ForeColour>#FFFFFF</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>Num_Customers</ColumnCode>
        <HeaderText>Number Of Customers</HeaderText>
        <Value>0</Value>
        <Format>N0</Format>
        <ForeColour>#996600</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>SUM</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>Num_Products</ColumnCode>
        <HeaderText>Number Of Products</HeaderText>
        <Value>0</Value>
        <Format>N2</Format>
        <ForeColour>#99CC00</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>SUM</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>Num_Promotions</ColumnCode>
        <HeaderText>Number Of Promotions</HeaderText>
        <Value>0</Value>
        <Format>N4</Format>
        <ForeColour>#99CC00</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>AVG</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>Num_Terms</ColumnCode>
        <HeaderText>Number Of Terms</HeaderText>
        <Value>0</Value>
        <Format>N8</Format>
        <ForeColour>#9999CC</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>MIN</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>CalcTotalItems</ColumnCode>
        <HeaderText>Total Linked Items</HeaderText>
        <Value>=Num_Customers + Num_Products + Num_Promotions + Num_Terms</Value>
        <Format>N8</Format>
        <ForeColour>#FFFFFF</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>MAX</TotalsAggregationMethod>
      </Attribute>
      <Attribute>
        <ColumnCode>CalcTwiceTotalItems</ColumnCode>
        <HeaderText>Twice Linked Items</HeaderText>
        <Value>=CalcTotalItems * 2</Value>
        <Format>N8</Format>
        <ForeColour>#FFFFFF</ForeColour>
        <BorderColour>#FFFFFF</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <IsHyperlink>0</IsHyperlink>
        <TotalsAggregationMethod>MAX</TotalsAggregationMethod>
      </Attribute>
    </Attributes>
  </RootItem>
</Results>
";

#endregion

      public DynamicDataView(XElement xmlSP = null)
        {

 

            if (xmlSP == null)
            {
                xmlSP = XElement.Parse(DummyData);
            }

            if (xmlSP.GetElement("RootItem") != null)
            {
                var root = xmlSP.GetElement("RootItem");

                Item_Type = root.GetValue<string>("Item_Type");
                Item_Idx = root.GetValue<string>("Item_Idx");
                Item_RowSortOrder = root.GetValue<int>("Item_RowSortOrder");

                if (root.GetElement("Attributes") != null)
                {

                    var a = new List<DynamicDataAttribute>();

                    foreach (var attr in root.Element("Attributes").Elements())
                    {
                        a.Add(new DynamicDataAttribute{
                            ColumnCode = attr.GetValue<string>("ColumnCode"),
                            HeaderText = attr.GetValue<string>("HeaderText"),
                            _innerValue = attr.GetValue<string>("Value"),
                            StringFormat = FixNull(attr.GetValue<string>("Format"),""),
                            ForeColour = FixNull(attr.GetValue<string>("ForeColour"),"#000000"),
                            BorderColour = FixNull(attr.GetValue<string>("BorderColour"), "#ffffff"),

                            IsDisplayed = attr.GetValue<int>("IsDisplayed") == 1 ? true : false,
                            IsEditable = attr.GetValue<int>("IsEditable") == 1 ? true : false,
                            IsHyperlink = attr.GetValue<int>("IsHyperlink") == 1 ? true : false,

                            TotalsAggregationMethod = attr.GetValue<string>("TotalsAggregationMethod")
                            }
                        );
                  
                    }

                    Attributes = a;
                }
            }
        }
   
  public string FixNull(string In, string Default)
  {
      if (string.IsNullOrWhiteSpace(In))
      {
        In = Default;
      }

      return In;
  }

      public string Item_Type { get; set; }
      public string Item_Idx { get; set; }
      public int Item_RowSortOrder { get; set; }

      private List<DynamicDataAttribute> _attributes;
      public List<DynamicDataAttribute> Attributes { get { return _attributes; } 
          set {
              _attributes = value;
              PropertyChanged.Raise(this, "Attributes");      
                }
      }

      public event PropertyChangedEventHandler PropertyChanged;
    }


  public class DynamicDataAttribute
  {

      public string ColumnCode { get; set; }

      public string HeaderText { get; set; }


      public string _innerValue;

      /// <summary>
      /// Gets or sets the Value of this PromotionExtraData.
      /// Values can be either actual or calculated
      /// values that start with '=' are calculated
      /// values are formatted using the StringFormat value
      /// </summary>
      public string Value
      {
          get { return _innerValue; }
          set { _innerValue = FormatValue(value); }
      }

      private string FormatValue(string value)
      {
          if (!string.IsNullOrWhiteSpace(StringFormat))
          {
              decimal d;
              if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out d))
              {
                  if (StringFormat.StartsWith("P"))
                  {
                      return (d / 100).ToString(StringFormat);
                  }
                  else
                  {
                      return d.ToString(StringFormat);
                  }
              }
              DateTime date;
              if (DateTime.TryParse(value, out date))
              {
                  if (StringFormat == "LongDate")
                  {
                      return date.ToLongDateString();
                  }
                  if (StringFormat == "ShortDate")
                  {
                      return date.ToShortDateString();
                  }




                  return date.ToString(StringFormat);
              }
          }

          return value;
      }



      public string ForeColour { get; set; }

      public string BorderColour { get; set; }

      public bool IsDisplayed { get; set; }

      public bool IsEditable { get; set; }

      public bool IsHyperlink { get; set; }

      public string StringFormat { get; set; }




      public string TotalsAggregationMethod { get; set; }

  }
}

