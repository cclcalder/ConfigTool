using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exceedra.Web.Models
{
    public static class StaticData
    {
       

        #region "rob grid"
        public const string RobGrid = @"<Results>
  <Grid_Title>ROB Filters</Grid_Title>
  <RowsLimitedAt>500</RowsLimitedAt>
  <RowsAvailable>4</RowsAvailable>
  <RootItem>
    <Item_Idx>2</Item_Idx>
    <Item_Type>ROBGrid</Item_Type>
    <Item_RowSortOrder>1</Item_RowSortOrder>
    <Attributes>
      <Attribute>
        <ColumnCode>IsSelected</ColumnCode>
        <HeaderText />
        <Value>false</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>1</IsEditable>
        <ControlType>Checkbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>-1</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>ROB_Idx</ColumnCode>
        <HeaderText>ROB_Idx Not Displayed To User</HeaderText>
        <Value>2</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>0</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>0</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>ROB_Name</ColumnCode>
        <HeaderText>Name</HeaderText>
        <Value>(EA-53) Growth Incentive 2015 - Poundland</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Hyperlink</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>0</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>SubType</ColumnCode>
        <HeaderText>SubType</HeaderText>
        <Value>Growth Incentives</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>1</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>Customer_Name</ColumnCode>
        <HeaderText>Customer</HeaderText>
        <Value> Poundland</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>2</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>Status_Name</ColumnCode>
        <HeaderText>Status</HeaderText>
        <Value>Planned</Value>
        <Format />
        <ForeColour />
        <BorderColour>#8B668B</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>3</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>StartDate</ColumnCode>
        <HeaderText>Start</HeaderText>
        <Value>2015-01-01</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>4</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>EndDate</ColumnCode>
        <HeaderText>End</HeaderText>
        <Value>2016-01-01</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>5</ColumnSortOrder>
      </Attribute>
    </Attributes>
  </RootItem>
  <RootItem>
    <Item_Idx>3</Item_Idx>
    <Item_Type>ROBGrid</Item_Type>
    <Item_RowSortOrder>2</Item_RowSortOrder>
    <Attributes>
      <Attribute>
        <ColumnCode>IsSelected</ColumnCode>
        <HeaderText />
        <Value>false</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>1</IsEditable>
        <ControlType>Checkbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>-1</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>ROB_Idx</ColumnCode>
        <HeaderText>ROB_Idx Not Displayed To User</HeaderText>
        <Value>3</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>0</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>0</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>ROB_Name</ColumnCode>
        <HeaderText>Name</HeaderText>
        <Value>(EA-54) Growth Incentive 2015 - Tesco</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Hyperlink</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>0</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>SubType</ColumnCode>
        <HeaderText>SubType</HeaderText>
        <Value>Growth Incentives</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>1</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>Customer_Name</ColumnCode>
        <HeaderText>Customer</HeaderText>
        <Value> Tesco</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>2</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>Status_Name</ColumnCode>
        <HeaderText>Status</HeaderText>
        <Value>Planned</Value>
        <Format />
        <ForeColour />
        <BorderColour>#8B668B</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>3</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>StartDate</ColumnCode>
        <HeaderText>Start</HeaderText>
        <Value>2015-01-01</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>4</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>EndDate</ColumnCode>
        <HeaderText>End</HeaderText>
        <Value>2016-01-01</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>5</ColumnSortOrder>
      </Attribute>
    </Attributes>
  </RootItem>
  <RootItem>
    <Item_Idx>5</Item_Idx>
    <Item_Type>ROBGrid</Item_Type>
    <Item_RowSortOrder>3</Item_RowSortOrder>
    <Attributes>
      <Attribute>
        <ColumnCode>IsSelected</ColumnCode>
        <HeaderText />
        <Value>false</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>1</IsEditable>
        <ControlType>Checkbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>-1</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>ROB_Idx</ColumnCode>
        <HeaderText>ROB_Idx Not Displayed To User</HeaderText>
        <Value>5</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>0</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>0</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>ROB_Name</ColumnCode>
        <HeaderText>Name</HeaderText>
        <Value>(EA-56) Growth Incentive 2015 - Morrisons</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Hyperlink</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>0</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>SubType</ColumnCode>
        <HeaderText>SubType</HeaderText>
        <Value>Growth Incentives</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>1</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>Customer_Name</ColumnCode>
        <HeaderText>Customer</HeaderText>
        <Value> Morrisons</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>2</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>Status_Name</ColumnCode>
        <HeaderText>Status</HeaderText>
        <Value>Planned</Value>
        <Format />
        <ForeColour />
        <BorderColour>#8B668B</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>3</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>StartDate</ColumnCode>
        <HeaderText>Start</HeaderText>
        <Value>2015-01-01</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>4</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>EndDate</ColumnCode>
        <HeaderText>End</HeaderText>
        <Value>2016-01-01</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>5</ColumnSortOrder>
      </Attribute>
    </Attributes>
  </RootItem>
  <RootItem>
    <Item_Idx>8</Item_Idx>
    <Item_Type>ROBGrid</Item_Type>
    <Item_RowSortOrder>4</Item_RowSortOrder>
    <Attributes>
      <Attribute>
        <ColumnCode>IsSelected</ColumnCode>
        <HeaderText />
        <Value>false</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>1</IsEditable>
        <ControlType>Checkbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>-1</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>ROB_Idx</ColumnCode>
        <HeaderText>ROB_Idx Not Displayed To User</HeaderText>
        <Value>8</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>0</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>0</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>ROB_Name</ColumnCode>
        <HeaderText>Name</HeaderText>
        <Value>(EA-59) Growth Incentive 2015 - Booker</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Hyperlink</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>0</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>SubType</ColumnCode>
        <HeaderText>SubType</HeaderText>
        <Value>Growth Incentives</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>1</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>Customer_Name</ColumnCode>
        <HeaderText>Customer</HeaderText>
        <Value> Booker</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>2</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>Status_Name</ColumnCode>
        <HeaderText>Status</HeaderText>
        <Value>Planned</Value>
        <Format />
        <ForeColour />
        <BorderColour>#8B668B</BorderColour>
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>3</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>StartDate</ColumnCode>
        <HeaderText>Start</HeaderText>
        <Value>2015-01-01</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>4</ColumnSortOrder>
      </Attribute>
      <Attribute>
        <ColumnCode>EndDate</ColumnCode>
        <HeaderText>End</HeaderText>
        <Value>2016-01-01</Value>
        <Format />
        <ForeColour />
        <BorderColour />
        <IsDisplayed>1</IsDisplayed>
        <IsEditable>0</IsEditable>
        <ControlType>Textbox</ControlType>
        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
        <ColumnSortOrder>5</ColumnSortOrder>
      </Attribute>
    </Attributes>
  </RootItem>
</Results>";
    }
#endregion
}