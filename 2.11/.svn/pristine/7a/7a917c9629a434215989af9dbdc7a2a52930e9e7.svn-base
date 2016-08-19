using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using Exceedra.Chart.Converters;
using Exceedra.Chart.Model;
using Exceedra.Common;
using Exceedra.Controls.ViewModels;
using Telerik.Charting;
using Telerik.Windows.Controls.ChartView;
using System.Collections.Generic;

namespace Exceedra.Chart.ViewModels
{
    public class RecordViewModel : Base
    {
        public RecordViewModel()
        {
            IsLoading = true;
            Chart = new Model.Chart();
        }

        public RecordViewModel(bool error)
        {
            IsLoading = false;
            Chart = new Model.Chart();
        }

        public RecordViewModel(XElement res)
        {
            IsLoading = true;
            Chart = Chart ?? new Model.Chart();
            Chart = LoadChartAndSeries(res);
            IsLoading = false;
        }

        public static RecordViewModel LoadWithData(XElement res)
        {
            var instance = new RecordViewModel();
            instance.Init(res);
            instance.IsLoading = false;
            return instance;
        }

        public void Init(XElement res, bool footer = false)
        {
            //XElement dummXElement = XElement.Parse("<Results><ChartType>Categorical</ChartType><xAxisType>Categorical</xAxisType><yAxisType>Linear</yAxisType><RightClickMenu_IsExportEnabled>1</RightClickMenu_IsExportEnabled><RightClickMenu_IsCategoryEnabled>0</RightClickMenu_IsCategoryEnabled><RightClickMenu_IsLinearEnabled>0</RightClickMenu_IsLinearEnabled><yAxisTitle>Contribution</yAxisTitle><xAxisTitle>Month</xAxisTitle><xAxisMin>201501</xAxisMin><xAxisMax>201601</xAxisMax><yAxisMin>-452807</yAxisMin><yAxisMax>6212064</yAxisMax><Series><SeriesType>Bar</SeriesType><Datapoints><Datapoint><Name/><Tooltip_Header1>January 2015</Tooltip_Header1><Tooltip_Header2>AP_Contribution</Tooltip_Header2><Tooltip_Header3/><Tooltip_Color/><X>201501.00</X><Y>1356708.40</Y></Datapoint><Datapoint><Name/><Tooltip_Header1>February 2015</Tooltip_Header1><Tooltip_Header2>AP_Contribution</Tooltip_Header2><Tooltip_Header3/><Tooltip_Color/><X>201502.00</X><Y>2768089.05</Y></Datapoint>     </Datapoints></Series><Series><SeriesType>Bar</SeriesType><Datapoints><Datapoint><Name/><Tooltip_Header1>January 2015</Tooltip_Header1><Tooltip_Header2>AP_Contribution</Tooltip_Header2><Tooltip_Header3/><Tooltip_Color/><X>201501.00</X><Y>2356708.40</Y></Datapoint><Datapoint><Name/><Tooltip_Header1>February 2015</Tooltip_Header1><Tooltip_Header2>AP_Contribution</Tooltip_Header2><Tooltip_Header3/><Tooltip_Color/><X>201502.00</X><Y>1768089.05</Y></Datapoint>     </Datapoints></Series><Series><SeriesType>Line</SeriesType><Datapoints><Datapoint><Name/><Tooltip_Header1/><Tooltip_Header2/><Tooltip_Header3/><Tooltip_Color/><X>201501.00</X><Y>5656708.40</Y></Datapoint><Datapoint><Name/><Tooltip_Header1/><Tooltip_Header2/><Tooltip_Header3/><Tooltip_Color/><X>201502.00</X><Y>2768089.05</Y></Datapoint></Datapoints></Series></Results>");

            Chart = Chart ?? new Model.Chart();
            Chart = LoadChartAndSeries(res);
        }

        private Model.Chart _chart;
        public Model.Chart Chart
        {
            get { return _chart; }
            set { _chart = value; NotifyPropertyChanged(this, vm => vm.Chart); }
        }

        private Model.Chart LoadChartAndSeries(XElement res)
        //DummyData
        {
            if (res == null)
                return null;
            /* RANGE Xml */
            //res = XElement.Parse("<Results><ChartType>Range</ChartType><xAxisType>Categorical</xAxisType><yAxisType>Linear</yAxisType><yAxisTitle>Test Values Test</yAxisTitle><xAxisTitle>Test Categories Test</xAxisTitle><MultiSeries><Series><SeriesType>Range</SeriesType><SeriesName>Start Node</SeriesName><Datapoints><Datapoint><Name>TEST 1</Name><Tooltip_Header1>Head 1</Tooltip_Header1><Tooltip_Header2>Head 2</Tooltip_Header2><Tooltip_Color>#8B668B</Tooltip_Color><X>Start</X><Y1>1</Y1><Y2>3</Y2></Datapoint></Datapoints></Series><Series><SeriesType>Range</SeriesType><SeriesName>Positive Nodes</SeriesName><Datapoints><Datapoint><Name>TEST 2</Name><Tooltip_Header1>Head 1</Tooltip_Header1><Tooltip_Header2>Head 2</Tooltip_Header2><Tooltip_Color>#8B668B</Tooltip_Color><X>One</X><Y1>3</Y1><Y2>5</Y2></Datapoint><Datapoint><Name>TEST 4</Name><Tooltip_Header1>Head 1</Tooltip_Header1><Tooltip_Header2>Head 2</Tooltip_Header2><Tooltip_Color>#8B668B</Tooltip_Color><X>Two</X><Y1>0</Y1><Y2>0</Y2></Datapoint><Datapoint><Name>TEST 6</Name><Tooltip_Header1>Head 1</Tooltip_Header1><Tooltip_Header2>Head 2</Tooltip_Header2><Tooltip_Color>#8B668B</Tooltip_Color><X>Three</X><Y1>0</Y1><Y2>1</Y2></Datapoint><Datapoint><Name>TEST 1</Name><Tooltip_Header1>Head 1</Tooltip_Header1><Tooltip_Header2>Head 2</Tooltip_Header2><Tooltip_Color>#8B668B</Tooltip_Color><X>Four</X><Y1>1</Y1><Y2>3</Y2></Datapoint><Datapoint><Name>TEST 2</Name><Tooltip_Header1>Head 1</Tooltip_Header1><Tooltip_Header2>Head 2</Tooltip_Header2><Tooltip_Color>#8B668B</Tooltip_Color><X>Five</X><Y1>3</Y1><Y2>5</Y2></Datapoint><Datapoint><Name>TEST 4</Name><Tooltip_Header1>Head 1</Tooltip_Header1><Tooltip_Header2>Head 2</Tooltip_Header2><Tooltip_Color>#8B668B</Tooltip_Color><X>Six</X><Y1>4</Y1><Y2>9</Y2></Datapoint><Datapoint><Name>TEST 6</Name><Tooltip_Header1>Head 1</Tooltip_Header1><Tooltip_Header2>Head 2</Tooltip_Header2><Tooltip_Color>#8B668B</Tooltip_Color><X>Seven</X><Y1>0</Y1><Y2>1</Y2></Datapoint><Datapoint><Name>TEST 1</Name><Tooltip_Header1>Head 1</Tooltip_Header1><Tooltip_Header2>Head 2</Tooltip_Header2><Tooltip_Color>#8B668B</Tooltip_Color><X>Eight</X><Y1>1</Y1><Y2>3</Y2></Datapoint><Datapoint><Name>TEST 2</Name><Tooltip_Header1>Head 1</Tooltip_Header1><Tooltip_Header2>Head 2</Tooltip_Header2><Tooltip_Color>#8B668B</Tooltip_Color><X>Nine</X><Y1>3</Y1><Y2>5</Y2></Datapoint><Datapoint><Name>TEST 4</Name><Tooltip_Header1>Head 1</Tooltip_Header1><Tooltip_Header2>Head 2</Tooltip_Header2><Tooltip_Color>#8B668B</Tooltip_Color><X>Ten</X><Y1>4</Y1><Y2>9</Y2></Datapoint></Datapoints></Series><Series>	<SeriesType>Range</SeriesType><SeriesName>Negative Nodes</SeriesName><Datapoints><Datapoint><Name>TEST 3</Name><Tooltip_Header1>Head 1</Tooltip_Header1><Tooltip_Header2>Head 2</Tooltip_Header2><Tooltip_Color>#8B668B</Tooltip_Color><X>Eleven</X><Y1>4</Y1><Y2>5</Y2></Datapoint><Datapoint><Name>TEST 5</Name><Tooltip_Header1>Head 1</Tooltip_Header1><Tooltip_Header2>Head 2</Tooltip_Header2><Tooltip_Color>#8B668B</Tooltip_Color><X>Twelve</X><Y1>0</Y1><Y2>9</Y2></Datapoint><Datapoint><Name>TEST 3</Name><Tooltip_Header1>Head 1</Tooltip_Header1><Tooltip_Header2>Head 2</Tooltip_Header2><Tooltip_Color>#8B668B</Tooltip_Color><X>Thirteen</X><Y1>4</Y1><Y2>5</Y2></Datapoint><Datapoint><Name>TEST 5</Name><Tooltip_Header1>Head 1</Tooltip_Header1><Tooltip_Header2>Head 2</Tooltip_Header2><Tooltip_Color>#8B668B</Tooltip_Color><X>Fourteen</X><Y1>0</Y1><Y2>9</Y2></Datapoint><Datapoint><Name>TEST 3</Name><Tooltip_Header1>Head 1</Tooltip_Header1><Tooltip_Header2>Head 2</Tooltip_Header2><Tooltip_Color>#8B668B</Tooltip_Color><X>Fifteen</X><Y1>4</Y1><Y2>5</Y2></Datapoint><Datapoint><Name>TEST 5</Name><Tooltip_Header1>Head 1</Tooltip_Header1><Tooltip_Header2>Head 2</Tooltip_Header2><Tooltip_Color>#8B668B</Tooltip_Color><X>Sixteen</X><Y1>0</Y1><Y2>9</Y2></Datapoint></Datapoints></Series><Series><SeriesType>Range</SeriesType><SeriesName>End Node</SeriesName><Datapoints><Datapoint><Name>TEST 6</Name><Tooltip_Header1>Head 1</Tooltip_Header1><Tooltip_Header2>Head 2</Tooltip_Header2><Tooltip_Color>#8B668B</Tooltip_Color><X>End</X><Y1>0</Y1><Y2>1</Y2></Datapoint></Datapoints></Series></MultiSeries></Results>");

            /* PIE Xml */
            //res = XElement.Parse("<Results><ChartType>Pie</ChartType><Series><SeriesType>Pie</SeriesType><SeriesName>Positive Nodes</SeriesName><Datapoints><Datapoint><Name>SegmentOne</Name><Y>5</Y></Datapoint><Datapoint><Name>SegmentTwo</Name><Y>9</Y></Datapoint><Datapoint><Name>SegmentThree</Name><Y>20</Y></Datapoint><Datapoint><Name>SegmentFour</Name><Y>5</Y></Datapoint></Datapoints></Series></Results>");

            /* Basic with null elements */
            //res = XElement.Parse("<Results><Title>Previous 10 Days</Title><ChartType>Categorical</ChartType><xAxisType>Categorical</xAxisType><yAxisType>Linear</yAxisType><xAxisTitle>Date</xAxisTitle><yAxisTitle>EPOS Volume</yAxisTitle><MultiSeries><Series><SeriesType>Line</SeriesType><SeriesName>Tesco Volume</SeriesName><Datapoints><Datapoint><Tooltip_Header1>Tesco Volume</Tooltip_Header1><Tooltip_Header2 /><Tooltip_Header3 /><Tooltip_Color>#1B141B</Tooltip_Color><X>20160130</X><Y>44099</Y></Datapoint><Datapoint><Tooltip_Header1>Tesco Volume</Tooltip_Header1><Tooltip_Header2 /><Tooltip_Header3 /><Tooltip_Color>#1B141B</Tooltip_Color><X>20160131</X><Y>43877</Y></Datapoint><Datapoint><Tooltip_Header1>Tesco Volume</Tooltip_Header1><Tooltip_Header2 /><Tooltip_Header3 /><Tooltip_Color>#1B141B</Tooltip_Color><X>20160201</X><Y>41951</Y></Datapoint><Datapoint><Tooltip_Header1>Tesco Volume</Tooltip_Header1><Tooltip_Header2 /><Tooltip_Header3 /><Tooltip_Color>#1B141B</Tooltip_Color><X>20160202</X><Y>37605</Y></Datapoint><Datapoint><Tooltip_Header1>Tesco Volume</Tooltip_Header1><Tooltip_Header2 /><Tooltip_Header3 /><Tooltip_Color>#1B141B</Tooltip_Color><X>20160203</X><Y>35046</Y></Datapoint><Datapoint><Tooltip_Header1>Tesco Volume</Tooltip_Header1><Tooltip_Header2 /><Tooltip_Header3 /><Tooltip_Color>#1B141B</Tooltip_Color><X>20160204</X><Y>36018</Y></Datapoint><Datapoint><Tooltip_Header1>Tesco Volume</Tooltip_Header1><Tooltip_Header2 /><Tooltip_Header3 /><Tooltip_Color>#1B141B</Tooltip_Color><X>20160205</X></Datapoint><Datapoint><Tooltip_Header1>Tesco Volume</Tooltip_Header1><Tooltip_Header2 /><Tooltip_Header3 /><Tooltip_Color>#1B141B</Tooltip_Color><X>20160206</X><Y>44089</Y></Datapoint><Datapoint><Tooltip_Header1>Tesco Volume</Tooltip_Header1><Tooltip_Header2 /><Tooltip_Header3 /><Tooltip_Color>#1B141B</Tooltip_Color><X>20160207</X><Y>42705</Y></Datapoint><Datapoint><Tooltip_Header1>Tesco Volume</Tooltip_Header1><Tooltip_Header2 /><Tooltip_Header3 /><Tooltip_Color>#1B141B</Tooltip_Color><X>20160208</X><Y>41781</Y></Datapoint></Datapoints></Series></MultiSeries></Results>");


            /* Multi-series for testing custome colouring. Data valid for both categorical and linear */
            //res = XElement.Parse("<Results><Title>Volume</Title><ChartType>Linear</ChartType><xAxisType>Linear</xAxisType><yAxisType>Linear</yAxisType><RightClickMenu_IsExportEnabled>0</RightClickMenu_IsExportEnabled><RightClickMenu_IsCategoryEnabled>0</RightClickMenu_IsCategoryEnabled><RightClickMenu_IsLinearEnabled>0</RightClickMenu_IsLinearEnabled><yAxisTitle>Volume</yAxisTitle><xAxisTitle>Month</xAxisTitle><xAxisMin>201601</xAxisMin><xAxisMax>201612</xAxisMax><yAxisMin>0</yAxisMin><yAxisMax>3473070</yAxisMax><IsLegendVisible>1</IsLegendVisible><MultiSeries><Series><SeriesType>Point</SeriesType><SeriesName>Total Vol</SeriesName><SeriesBrush>#0000BB</SeriesBrush><Datapoints><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201601</Tooltip_Header3><Tooltip_Color /><X>201601</X><Y>982759.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201602</Tooltip_Header3><Tooltip_Color /><X>201602</X><Y>1277587.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201603</Tooltip_Header3><Tooltip_Color /><X>201603</X><Y>1547845.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201604</Tooltip_Header3><Tooltip_Color /><X>201604</X><Y>2456897.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201605</Tooltip_Header3><Tooltip_Color /><X>201605</X><Y>2702587.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201606</Tooltip_Header3><Tooltip_Color /><X>201606</X><Y>2948277.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201607</Tooltip_Header3><Tooltip_Color /><X>201607</X><Y>0.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201608</Tooltip_Header3><Tooltip_Color /><X>201608</X><Y>0.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201609</Tooltip_Header3><Tooltip_Color /><X>201609</X><Y>0.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201610</Tooltip_Header3><Tooltip_Color /><X>201610</X><Y>0.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201611</Tooltip_Header3><Tooltip_Color /><X>201611</X><Y>0.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201612</Tooltip_Header3><Tooltip_Color /><X>201612</X><Y>0.00</Y></Datapoint></Datapoints></Series><Series><SeriesType>Point</SeriesType><SeriesName>Incr Vol</SeriesName><SeriesBrush>#BB0000</SeriesBrush><Datapoints><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201601</Tooltip_Header3><Tooltip_Color /><X>201601</X><Y>226035.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201602</Tooltip_Header3><Tooltip_Color /><X>201602</X><Y>293845.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201603</Tooltip_Header3><Tooltip_Color /><X>201603</X><Y>356004.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201604</Tooltip_Header3><Tooltip_Color /><X>201604</X><Y>614224.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201605</Tooltip_Header3><Tooltip_Color /><X>201605</X><Y>702673.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201606</Tooltip_Header3><Tooltip_Color /><X>201606</X><Y>737069.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201607</Tooltip_Header3><Tooltip_Color /><X>201607</X><Y>0.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201608</Tooltip_Header3><Tooltip_Color /><X>201608</X><Y>0.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201609</Tooltip_Header3><Tooltip_Color /><X>201609</X><Y>0.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201610</Tooltip_Header3><Tooltip_Color /><X>201610</X><Y>0.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201611</Tooltip_Header3><Tooltip_Color /><X>201611</X><Y>0.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201612</Tooltip_Header3><Tooltip_Color /><X>201612</X><Y>0.00</Y></Datapoint></Datapoints></Series><Series><SeriesType>Point</SeriesType><SeriesName>Total Vol</SeriesName><Datapoints><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201601</Tooltip_Header3><Tooltip_Color /><X>201601</X><Y>0.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201602</Tooltip_Header3><Tooltip_Color /><X>201602</X><Y>0.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201603</Tooltip_Header3><Tooltip_Color /><X>201603</X><Y>0.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201604</Tooltip_Header3><Tooltip_Color /><X>201604</X><Y>0.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201605</Tooltip_Header3><Tooltip_Color /><X>201605</X><Y>0.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201606</Tooltip_Header3><Tooltip_Color /><X>201606</X><Y>0.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201607</Tooltip_Header3><Tooltip_Color /><X>201607</X><Y>3193966.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201608</Tooltip_Header3><Tooltip_Color /><X>201608</X><Y>2948277.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201609</Tooltip_Header3><Tooltip_Color /><X>201609</X><Y>2211207.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201610</Tooltip_Header3><Tooltip_Color /><X>201610</X><Y>1842673.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201611</Tooltip_Header3><Tooltip_Color /><X>201611</X><Y>1474138.00</Y></Datapoint><Datapoint><Name>Total Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201612</Tooltip_Header3><Tooltip_Color /><X>201612</X><Y>982759.00</Y></Datapoint></Datapoints></Series><Series><SeriesType>Point</SeriesType><SeriesName>Incr Vol</SeriesName><Datapoints><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201601</Tooltip_Header3><Tooltip_Color /><X>201601</X><Y>0.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201602</Tooltip_Header3><Tooltip_Color /><X>201602</X><Y>0.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201603</Tooltip_Header3><Tooltip_Color /><X>201603</X><Y>0.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201604</Tooltip_Header3><Tooltip_Color /><X>201604</X><Y>0.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201605</Tooltip_Header3><Tooltip_Color /><X>201605</X><Y>0.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201606</Tooltip_Header3><Tooltip_Color /><X>201606</X><Y>0.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201607</Tooltip_Header3><Tooltip_Color /><X>201607</X><Y>830431.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201608</Tooltip_Header3><Tooltip_Color /><X>201608</X><Y>707586.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201609</Tooltip_Header3><Tooltip_Color /><X>201609</X><Y>508578.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201610</Tooltip_Header3><Tooltip_Color /><X>201610</X><Y>423815.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201611</Tooltip_Header3><Tooltip_Color /><X>201611</X><Y>339052.00</Y></Datapoint><Datapoint><Name>Incr Vol</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201612</Tooltip_Header3><Tooltip_Color /><X>201612</X><Y>226035.00</Y></Datapoint></Datapoints></Series><Series><SeriesType>Line</SeriesType><SeriesName>Total Vol LY</SeriesName><Datapoints><Datapoint><Name>Total Vol LY</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201601</Tooltip_Header3><Tooltip_Color /><X>201601</X><Y>1148354.00</Y></Datapoint><Datapoint><Name>Total Vol LY</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201602</Tooltip_Header3><Tooltip_Color /><X>201602</X><Y>1492860.00</Y></Datapoint><Datapoint><Name>Total Vol LY</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201603</Tooltip_Header3><Tooltip_Color /><X>201603</X><Y>1808657.00</Y></Datapoint><Datapoint><Name>Total Vol LY</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201604</Tooltip_Header3><Tooltip_Color /><X>201604</X><Y>1996229.00</Y></Datapoint><Datapoint><Name>Total Vol LY</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201605</Tooltip_Header3><Tooltip_Color /><X>201605</X><Y>2043156.00</Y></Datapoint><Datapoint><Name>Total Vol LY</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201606</Tooltip_Header3><Tooltip_Color /><X>201606</X><Y>2764009.00</Y></Datapoint><Datapoint><Name>Total Vol LY</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201607</Tooltip_Header3><Tooltip_Color /><X>201607</X><Y>3219518.00</Y></Datapoint><Datapoint><Name>Total Vol LY</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201608</Tooltip_Header3><Tooltip_Color /><X>201608</X><Y>3473070.00</Y></Datapoint><Datapoint><Name>Total Vol LY</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201609</Tooltip_Header3><Tooltip_Color /><X>201609</X><Y>2583796.00</Y></Datapoint><Datapoint><Name>Total Vol LY</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201610</Tooltip_Header3><Tooltip_Color /><X>201610</X><Y>2153163.00</Y></Datapoint><Datapoint><Name>Total Vol LY</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201611</Tooltip_Header3><Tooltip_Color /><X>201611</X><Y>1722531.00</Y></Datapoint><Datapoint><Name>Total Vol LY</Name><Tooltip_Header1 /><Tooltip_Header2 /><Tooltip_Header3>201612</Tooltip_Header3><Tooltip_Color /><X>201612</X><Y>1148354.00</Y></Datapoint></Datapoints></Series></MultiSeries></Results>");

            /* If we are building from grid xml */
            if (res.Descendants("RootItem").Any())
            {
                return GridToChart.Convert(res);
            }

            /* Otherwise load it from standard chart xml */
            var loadedChart = new Model.Chart
            {
                ChartType = res.Element("ChartType").MaybeValue(),
                YAxisType = res.Element("yAxisType").MaybeValue(),
                XAxisType = res.Element("xAxisType").MaybeValue(),
                YAxisTitle = res.Element("yAxisTitle").MaybeValue(),
                XAxisTitle = res.Element("xAxisTitle").MaybeValue(),
                XAxisBrush = res.Element("xAxisBrush").MaybeValue(),
                YAxisBrush = res.Element("yAxisBrush").MaybeValue(),
                Title = res.Element("Title").MaybeValue(),
                DisplayXandYTooltip = res.Element("DisplayXandYTooltip").MaybeValue() == "0" ? Visibility.Collapsed : Visibility.Visible
            };

            var seriesCollection = res.Descendants("Series");

            var seriesList = seriesCollection.Select(seriesXml => LoadSeries(seriesXml, loadedChart)).ToList();

            loadedChart.Series = new ObservableCollection<SingleSeries>(seriesList);


            return loadedChart;
        }

        private SingleSeries LoadSeries(XElement xml, Model.Chart loadedChart)
        {
            var chartType = loadedChart.ChartType ?? "Categorical";
            var singleSeries = new SingleSeries();
            
            var datapointsCollection = (from r in xml.Descendants().Where(e => e.Name.LocalName.ToString().ToLowerInvariant() == "datapoint") select r).ToList();

            singleSeries.Datapoints = chartType.Equals("Range") ?
                new ObservableCollection<Datapoint>(new ObservableCollection<RangeDatapoint>(XmlToDatapoints.LoadRangeDataPoints(datapointsCollection))) :
                chartType.Equals("Categorical") ?
                new ObservableCollection<Datapoint>(new ObservableCollection<CategoricalDatapoint>(XmlToDatapoints.LoadCategoricalDatapoints(datapointsCollection))) :
                new ObservableCollection<Datapoint>(new ObservableCollection<LinearDatapoint>(XmlToDatapoints.LoadLinearDatapoints(datapointsCollection)));

            singleSeries.Datapoints.Do(d => d.DisplayXandYTooltip = loadedChart.DisplayXandYTooltip);
            singleSeries.SeriesType = xml.Element("SeriesType").MaybeValue();
            singleSeries.SeriesName = xml.Element("SeriesName").MaybeValue();
            singleSeries.SeriesBrush = xml.Element("SeriesBrush").MaybeValue();

            var individualAxisXml = xml.Element("IndividualAxis");
            if (individualAxisXml != null)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    LinearAxis individualAxis = new LinearAxis();

                    individualAxis.Title = singleSeries.SeriesName;

                    // By default left. To be on the right the xml must contain the part below.
                    // <IndividualAxis>
                    //  <Position>Right</Position>
                    // </ IndividualAxis>
                    var individualAxisPositionXml = individualAxisXml.Element("Position");
                    if (individualAxisPositionXml != null
                        && !string.IsNullOrEmpty(individualAxisPositionXml.Value)
                        && individualAxisPositionXml.Value.ToLower() == "right")
                        individualAxis.HorizontalLocation = AxisHorizontalLocation.Right;

                    // If SeriesBrush is not specified the axis is black.
                    if (!string.IsNullOrEmpty(singleSeries.SeriesBrush))
                        individualAxis.ElementBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(singleSeries.SeriesBrush));

                    singleSeries.IndividualAxis = individualAxis;
                });
            }

            Application.Current.Dispatcher.Invoke(delegate
            {
                // In order to change the colour of the bar series we cannot simply change the "fill", "stroke" or any reasonable property, as they are not accessible.
                // Therefore, we must create our own template, which is a rectangle with customized fill property.
                // The other series types don't need their own templates.
                if (!string.IsNullOrEmpty(singleSeries.SeriesBrush))
                {
                    DataTemplate pointTemplate = chartType.ToLower() == "categorical" ? new DataTemplate(typeof(Rectangle)) : new DataTemplate(typeof(Ellipse));
                    FrameworkElementFactory shapeFactory = chartType.ToLower() == "categorical" ? new FrameworkElementFactory(typeof(Rectangle)) : new FrameworkElementFactory(typeof(Ellipse));
                    shapeFactory.SetValue(Shape.FillProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString(singleSeries.SeriesBrush)));
                    if(chartType.ToLower() != "categorical")
                    {
                        shapeFactory.SetValue(FrameworkElement.WidthProperty, 10.0);
                        shapeFactory.SetValue(FrameworkElement.HeightProperty, 10.0);
                    }
                    pointTemplate.VisualTree = shapeFactory;

                    singleSeries.PointTemplate = pointTemplate;
                }
            });



            return singleSeries;
        }

        public delegate void Changed();

        public event Changed Outliers;
        public event Changed MetaData;
        public void UpdateOutliers(string columnCode, bool insert)
        {
            if (Chart.Outliers == null) Chart.Outliers = new System.Collections.Generic.List<string>();

            Chart.Outliers = Chart.Outliers.Distinct().ToList();

            var contains = Chart.Outliers.Contains(columnCode);

            if (insert)
            {
                if (!contains)
                    Chart.Outliers.Add(columnCode);
            }
            else
                if (contains)
                Chart.Outliers.Remove(columnCode);

            if (Outliers != null)
                Outliers();
        }

        #region Loading Properties 

        public bool NoData
        {
            get
            {
                return Chart == null || Chart.Series == null || !Chart.Series.Any();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged(this, vm => vm.IsLoading);
                NotifyPropertyChanged(this, vm => vm.NoDataMessage);
            }
        }

        public bool NoDataMessage { get { return NoData && !IsLoading; } }

        #endregion

        #region MetaData

        public void LoadMetaData(XElement xml)
        {
            if (xml == null) return;

            VerticalLines = new List<string>(xml.Elements("ChartLine").Select(l => l.MaybeValue()));
            MarkedAreas = new List<MarkedArea>(xml.Elements("MarkedArea").Select(a => new MarkedArea(a)));

            if (MetaData != null)
                MetaData();
        }

        public void RefreshMetaData()
        {
            if (MetaData != null)
                MetaData();
        }

        public List<string> VerticalLines { get; set; }
        public List<MarkedArea> MarkedAreas { get; set; }

        #endregion
    }

    public class MarkedArea
    {
        public string HorizontalFrom { get; set; }
        public string HorizontalTo { get; set; }
        public string Fill { get; set; }

        public MarkedArea(XElement xml)
        {
            HorizontalFrom = xml.Attribute("HorizontalFrom").MaybeValue();
            HorizontalTo = xml.Attribute("HorizontalTo").MaybeValue();
            Fill = xml.Attribute("Fill").MaybeValue();
        }
    }

}
