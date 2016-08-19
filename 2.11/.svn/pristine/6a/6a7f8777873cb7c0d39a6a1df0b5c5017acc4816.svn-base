using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using ViewModels;

namespace WPF.Pages.Terms
{
    /// <summary>
    /// Interaction logic for GroupEditor.xaml
    /// </summary>
    public partial class GroupEditor
    { 
        public static IDictionary<string, object> ToDictionary(XElement element)
        {
            var dict = new Dictionary<string, object>();
            foreach (var e in element.Elements())
            {
                dict.Add(e.Name.LocalName, e.Value);
            }

            return dict;
        }

        public string AppTypeID;
        private string _groupID;

        private GroupEditorViewModel _gvm;

        public GroupEditorViewModel GVM
        {
            get { return _gvm; }
            set
            {
                _gvm = value;                
            }
        }

        public GroupEditor()
        {
            InitializeComponent();

            AppTypeID = "300";
            _groupID = "1";

            load(AppTypeID, _groupID);

            Loaded += Page_Loaded;
        }

        public GroupEditor(string appTypeID, string groupID)
        {
            AppTypeID = appTypeID;
            _groupID = groupID;
            InitializeComponent();
            //load(_appTypeID, _groupID);

            _gvm = new GroupEditorViewModel(appTypeID, groupID);
            DataContext = _gvm;

            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
           

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            load(AppTypeID, _groupID);
        }

        private void load(string appTypeID, string groupID)
        {
            _gvm = new GroupEditorViewModel(appTypeID, groupID);
            DataContext = _gvm;

            //  _gvm.PropertyChanged += GvmOnPropertyChanged;
            //TabControl.ItemDataSource = _gvm.RVM;

            //var r = new RecordViewModel(XElement.Parse(demo));

            //foreach (var x in _gvm.RVM.Records)
            //{
            //    x.TabbedViewModel = new TabbedViewModel(GetData());
            //}


            // DynamicGridControl.DetailsViewHandler = DetailsHandler;
            //DynamicGridControl.CanShowDetails = true;

            // DynamicGridControl.ItemDataSource = _gvm.RVM;
        }

        //private void GvmOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        //{
        //    if (propertyChangedEventArgs.PropertyName == "RVM")
        //    {
        //        DynamicGridControl.ItemDataSource = _gvm.RVM;
        //    }
        //}

        //private void DetailsHandler(object sender, RoutedEventArgs e)
        //{
        //    var obj = ((FrameworkElement)sender).DataContext as Record;

        //    try
        //    {
        //        // the original source is what was clicked.  For example 
        //        // a button.
        //        DependencyObject dep = (DependencyObject)e.OriginalSource;

        //        // iteratively traverse the visual tree upwards looking for
        //        // the clicked row.
        //        while ((dep != null) && !(dep is DataGridRow))
        //        {
        //            dep = VisualTreeHelper.GetParent(dep);
        //        }

        //        // if we found the clicked row
        //        if (dep != null && dep is DataGridRow)
        //        {
        //            // get the row
        //            DataGridRow row = (DataGridRow)dep;

        //            // change the details visibility
        //            if (row.DetailsVisibility == Visibility.Collapsed)
        //            {
        //                row.DetailsVisibility = Visibility.Visible;
                         
        //                if (obj.TabbedViewModel != null)
        //                {
                            
        //                }
                         
        //            }
        //            else
        //            {
        //                row.DetailsVisibility = Visibility.Collapsed;
        //            }
        //        }
        //    }
        //    catch (System.Exception)
        //    {
        //    }


        //}

//        private XElement GetData()
//        {
//            return XElement.Parse(string.Format(@"<Results>
//                      <RootItem>
//                        <Item_Idx>1</Item_Idx>
//                        <Item_Type>Tab1</Item_Type>
//                        <Item_RowSortOrder>1</Item_RowSortOrder>
//                        <Attributes> 
//                          <Attribute>
//                            <ColumnCode>HG</ColumnCode>
//                            <HeaderText>Horizontal Grid</HeaderText>
//                            <DataSource>app.Procast_SP_ROBGroup_GetTab_Page1</DataSource>
//        		            <DataSourceInput>
//        		              &lt;User_Idx /&gt;
//        		              &lt;SalesOrg_Idx /&gt;
//        		              &lt;AppType_Idx &gt;300&lt;/AppType_Idx&gt; 
//        		              &lt;ROB_Idx /&gt;1&lt;/ROB_Idx&gt; 	
//        		            </DataSourceInput>
//                            <Format/>
//                            <ForeColour />
//                            <BorderColour />
//                            <IsDisplayed>1</IsDisplayed>
//                            <IsEditable>0</IsEditable>
//                            <ControlType>VerticalGrid</ControlType>
//                            <DataSource />
//                            <DependentColumn />
//                            <TotalsAggregationMethod/>
//                          </Attribute>  
//             
//                          </Attributes> 
//                     </RootItem>
//                </Results>"));

//        }
//        private XElement GetData()
//        {
//            var doc = XElement.Parse(@"
//                <Results>
//                  <RootItem>
//                    <Item_Idx>1</Item_Idx>
//                    <Item_Type>Tab1</Item_Type>
//                    <Item_RowSortOrder>1</Item_RowSortOrder>
//                    <Attributes> 
//     
//                      <Attribute>
//                        <ColumnCode>HG</ColumnCode>
//                        <HeaderText>Tabbed VGrid</HeaderText>
//                        <DataSource>app.Procast_SP_ROBGroup_GetTab_Page1</DataSource>
//		                <DataSourceInput>
//		                  &lt;User_Idx /&gt;
//		                  &lt;SalesOrg_Idx /&gt;
//		                  &lt;AppType_Idx &gt;300&lt;/AppType_Idx&gt; 
//		                  &lt;ROB_Idx /&gt;1&lt;/ROB_Idx&gt; 	
//		                </DataSourceInput>
//                        <Format/>
//                        <ForeColour />
//                        <BorderColour />
//                        <IsDisplayed>1</IsDisplayed>
//                        <IsEditable>0</IsEditable>
//                        <ControlType>VerticalGrid</ControlType>
//                        <DataSource />
//                        <DependentColumn />
//                        <TotalsAggregationMethod/>
//                      </Attribute>  
//     
//                      </Attributes> 
//                 </RootItem> 
//                </Results>
//              ");

//            return doc;
//        }

//        private const string c1 = @"<Results><ChartType>Categorical</ChartType><xAxisType>Categorical</xAxisType><yAxisType>Linear</yAxisType><RightClickMenu_IsExportEnabled>1</RightClickMenu_IsExportEnabled><RightClickMenu_IsCategoryEnabled>0</RightClickMenu_IsCategoryEnabled><RightClickMenu_IsLinearEnabled>0</RightClickMenu_IsLinearEnabled><yAxisTitle>Contribution</yAxisTitle><xAxisTitle>Month</xAxisTitle><xAxisMin>201501</xAxisMin><xAxisMax>201601</xAxisMax><yAxisMin>-452807</yAxisMin><yAxisMax>6212064</yAxisMax>-<Series><SeriesType>Bar</SeriesType>-<Datapoints>-<Datapoint><Tooltip_Header1>January 2015</Tooltip_Header1><Tooltip_Header2>AP_Contribution</Tooltip_Header2><Tooltip_Header3/><Tooltip_Color/><X>201501.00</X><Y>1356708.40</Y></Datapoint>-<Datapoint><Tooltip_Header1>February 2015</Tooltip_Header1><Tooltip_Header2>AP_Contribution</Tooltip_Header2><Tooltip_Header3/><Tooltip_Color/><X>201502.00</X><Y>2768089.05</Y></Datapoint></Datapoints></Series>-<Series><SeriesType>Bar</SeriesType>-<Datapoints>-<Datapoint><Tooltip_Header1>January 2015</Tooltip_Header1><Tooltip_Header2>AP_Contribution</Tooltip_Header2><Tooltip_Header3/><Tooltip_Color/><X>201501.00</X><Y>2356708.40</Y></Datapoint>-<Datapoint><Tooltip_Header1>February 2015</Tooltip_Header1><Tooltip_Header2>AP_Contribution</Tooltip_Header2><Tooltip_Header3/><Tooltip_Color/><X>201502.00</X><Y>1768089.05</Y></Datapoint></Datapoints></Series>-<Series><SeriesType>Line</SeriesType>-<Datapoints>-<Datapoint><Tooltip_Header1/><Tooltip_Header2/><Tooltip_Header3/><Tooltip_Color/><X>201501.00</X><Y>2356708.40</Y></Datapoint>-<Datapoint><Tooltip_Header1/><Tooltip_Header2/><Tooltip_Header3/><Tooltip_Color/><X>201502.00</X><Y>1768089.05</Y></Datapoint></Datapoints></Series></Results>";
//        const string g1 = @" <Results><RootItem><Item_Idx>1</Item_Idx><Item_Type>Scenario</Item_Type><Item_RowSortOrder>1</Item_RowSortOrder><Attributes><Attribute><ColumnCode>IsSelected</ColumnCode><HeaderText /><Value>false</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Checkbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Scen_Name</ColumnCode><HeaderText>Scenario Name</HeaderText><Value>(LIVE) - Live</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Hyperlink</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>COUNT</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Author</ColumnCode><HeaderText>Author</HeaderText><Value>Exceedra, Admin</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Start_Day</ColumnCode><HeaderText>Start Date</HeaderText><Value>2015-01-02</Value><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>End_Day</ColumnCode><HeaderText>End Date</HeaderText><Value>2017-01-01</Value><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Create_Day</ColumnCode><HeaderText>Created On</HeaderText><Value>2015-06-06</Value><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Type_Name</ColumnCode><HeaderText>Type</HeaderText><Value>System Controlled, Non-User Editable</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Status_Name</ColumnCode><HeaderText>Status</HeaderText><Value>System Controlled, Non-User Editable</Value><Format /><ForeColour>#000000</ForeColour><BorderColour>#A9A9A9</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Customers</ColumnCode><HeaderText>Customers</HeaderText><Value>193</Value><Format>N0</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Products</ColumnCode><HeaderText>Products</HeaderText><Value>50</Value><Format>N0</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Promotions</ColumnCode><HeaderText>Promotions</HeaderText><Value>0</Value><Format>N0</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>IsActiveBudget</ColumnCode><HeaderText>Active Budget</HeaderText><Value>false</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>CheckBox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute></Attributes></RootItem><RootItem><Item_Idx>2</Item_Idx><Item_Type>Scenario</Item_Type><Item_RowSortOrder>2</Item_RowSortOrder><Attributes><Attribute><ColumnCode>IsSelected</ColumnCode><HeaderText /><Value>false</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Checkbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Scen_Name</ColumnCode><HeaderText>Scenario Name</HeaderText><Value>(CH-1) - test</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Hyperlink</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>COUNT</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Author</ColumnCode><HeaderText>Author</HeaderText><Value>Hogan, Craig</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Start_Day</ColumnCode><HeaderText>Start Date</HeaderText><Value>2015-01-02</Value><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>End_Day</ColumnCode><HeaderText>End Date</HeaderText><Value>2017-01-01</Value><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Create_Day</ColumnCode><HeaderText>Created On</HeaderText><Value>2015-06-08</Value><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Type_Name</ColumnCode><HeaderText>Type</HeaderText><Value>User Scenario</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Status_Name</ColumnCode><HeaderText>Status</HeaderText><Value>Open</Value><Format /><ForeColour>#000000</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Customers</ColumnCode><HeaderText>Customers</HeaderText><Value>17</Value><Format>N0</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Products</ColumnCode><HeaderText>Products</HeaderText><Value>42</Value><Format>N0</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Promotions</ColumnCode><HeaderText>Promotions</HeaderText><Value>0</Value><Format>N0</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>IsActiveBudget</ColumnCode><HeaderText>Active Budget</HeaderText><Value>false</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>CheckBox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute></Attributes></RootItem><RootItem><Item_Idx>3</Item_Idx><Item_Type>Scenario</Item_Type><Item_RowSortOrder>3</Item_RowSortOrder><Attributes><Attribute><ColumnCode>IsSelected</ColumnCode><HeaderText /><Value>false</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Checkbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Scen_Name</ColumnCode><HeaderText>Scenario Name</HeaderText><Value>(CH-2) - test</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Hyperlink</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>COUNT</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Author</ColumnCode><HeaderText>Author</HeaderText><Value>Hogan, Craig</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Start_Day</ColumnCode><HeaderText>Start Date</HeaderText><Value>2015-01-02</Value><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>End_Day</ColumnCode><HeaderText>End Date</HeaderText><Value>2017-01-01</Value><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Create_Day</ColumnCode><HeaderText>Created On</HeaderText><Value>2015-06-08</Value><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Type_Name</ColumnCode><HeaderText>Type</HeaderText><Value>User Scenario</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Status_Name</ColumnCode><HeaderText>Status</HeaderText><Value>Open</Value><Format /><ForeColour>#000000</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Customers</ColumnCode><HeaderText>Customers</HeaderText><Value>17</Value><Format>N0</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Products</ColumnCode><HeaderText>Products</HeaderText><Value>42</Value><Format>N0</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Promotions</ColumnCode><HeaderText>Promotions</HeaderText><Value>0</Value><Format>N0</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>IsActiveBudget</ColumnCode><HeaderText>Active Budget</HeaderText><Value>false</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>CheckBox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute></Attributes></RootItem></Results>"; const string v1 = @"<Results><RootItem><Item_Idx>26</Item_Idx><Item_Type>Promotion</Item_Type><Item_RowSortOrder>1</Item_RowSortOrder><Attributes><Attribute><ColumnCode>MECHANIC</ColumnCode><HeaderText>Mechanic</HeaderText><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Dropdown</ControlType><DataSource>app.Procast_SP_PROMO_Wizard_Attributes_PopulateDropdowns</DataSource><DataSourceInput>&lt;User_Idx /&gt;&lt;Promo_Idx /&gt;&lt;ColumnCode /&gt;</DataSourceInput><DependentColumns /><AttributeGroup_Sort>0</AttributeGroup_Sort></Attribute><Attribute><ColumnCode>FEATURE</ColumnCode><HeaderText>Feature</HeaderText><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Dropdown</ControlType><DataSource>app.Procast_SP_PROMO_Wizard_Attributes_PopulateDropdowns</DataSource><DataSourceInput>&lt;User_Idx /&gt;&lt;Promo_Idx /&gt;&lt;ColumnCode /&gt;</DataSourceInput><DependentColumns /><AttributeGroup_Sort>1</AttributeGroup_Sort></Attribute><Attribute><ColumnCode>SUPPORT</ColumnCode><HeaderText>Support</HeaderText><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>MultiSelectDropdown</ControlType><DataSource>app.Procast_SP_PROMO_Wizard_Attributes_PopulateDropdowns</DataSource><DataSourceInput>&lt;User_Idx /&gt;&lt;Promo_Idx /&gt;&lt;ColumnCode /&gt;</DataSourceInput><DependentColumns /><AttributeGroup_Sort>2</AttributeGroup_Sort></Attribute><Attribute><ColumnCode>OBJECTIVES</ColumnCode><HeaderText>Objectives</HeaderText><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>MultiSelectDropdown</ControlType><DataSource>app.Procast_SP_PROMO_Wizard_Attributes_PopulateDropdowns</DataSource><DataSourceInput>&lt;User_Idx /&gt;&lt;Promo_Idx /&gt;&lt;ColumnCode /&gt;</DataSourceInput><DependentColumns /><AttributeGroup_Sort>3</AttributeGroup_Sort></Attribute><Attribute><ColumnCode>PROMO_FUNDS</ColumnCode><HeaderText>Funding</HeaderText><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Dropdown</ControlType><DataSource>app.Procast_SP_PROMO_Wizard_Attributes_PopulateDropdowns</DataSource><DataSourceInput>&lt;User_Idx /&gt;&lt;Promo_Idx /&gt;&lt;ColumnCode /&gt;</DataSourceInput><DependentColumns /><AttributeGroup_Sort>4</AttributeGroup_Sort></Attribute><Attribute><ColumnCode>HIERARCHY</ColumnCode><HeaderText>Use Hierarchy Level Planning</HeaderText><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Dropdown</ControlType><DataSource>app.Procast_SP_PROMO_Wizard_Attributes_PopulateDropdowns</DataSource><DataSourceInput>&lt;User_Idx /&gt;&lt;Promo_Idx /&gt;&lt;ColumnCode /&gt;</DataSourceInput><DependentColumns /><AttributeGroup_Sort>6</AttributeGroup_Sort></Attribute><Attribute><ColumnCode>ADD_SPEND_REQ</ColumnCode><HeaderText>Additional Spend Request</HeaderText><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Dropdown</ControlType><DataSource>app.Procast_SP_PROMO_Wizard_Attributes_PopulateDropdowns</DataSource><DataSourceInput>&lt;User_Idx /&gt;&lt;Promo_Idx /&gt;&lt;ColumnCode /&gt;</DataSourceInput><DependentColumns /><AttributeGroup_Sort>10</AttributeGroup_Sort></Attribute><Attribute><ColumnCode>USE_UPLIFT</ColumnCode><HeaderText>Use Promotion Uplift Method</HeaderText><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Dropdown</ControlType><DataSource>app.Procast_SP_PROMO_Wizard_Attributes_PopulateDropdowns</DataSource><DataSourceInput>&lt;User_Idx /&gt;&lt;Promo_Idx /&gt;&lt;ColumnCode /&gt;</DataSourceInput><DependentColumns /><AttributeGroup_Sort>12</AttributeGroup_Sort></Attribute><Attribute><ColumnCode>MAINTAIN_BASE</ColumnCode><HeaderText>Keep Promotion Base Volume Updated</HeaderText><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Dropdown</ControlType><DataSource>app.Procast_SP_PROMO_Wizard_Attributes_PopulateDropdowns</DataSource><DataSourceInput>&lt;User_Idx /&gt;&lt;Promo_Idx /&gt;&lt;ColumnCode /&gt;</DataSourceInput><DependentColumns /><AttributeGroup_Sort>13</AttributeGroup_Sort></Attribute></Attributes></RootItem></Results>"; 
//        const string demo = @"<Results><RootItem><Item_Idx>1</Item_Idx><Item_Type>Scenario</Item_Type><Item_RowSortOrder>1</Item_RowSortOrder><Attributes><Attribute><ColumnCode>IsSelected</ColumnCode><HeaderText /><Value>false</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Checkbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Scen_Name</ColumnCode><HeaderText>Scenario Name</HeaderText><Value>(LIVE) - Live</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Hyperlink</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>COUNT</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Author</ColumnCode><HeaderText>Author</HeaderText><Value>Exceedra, Admin</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Start_Day</ColumnCode><HeaderText>Start Date</HeaderText><Value>2015-01-02</Value><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>End_Day</ColumnCode><HeaderText>End Date</HeaderText><Value>2017-01-01</Value><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Create_Day</ColumnCode><HeaderText>Created On</HeaderText><Value>2015-06-06</Value><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Type_Name</ColumnCode><HeaderText>Type</HeaderText><Value>System Controlled, Non-User Editable</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Status_Name</ColumnCode><HeaderText>Status</HeaderText><Value>System Controlled, Non-User Editable</Value><Format /><ForeColour>#000000</ForeColour><BorderColour>#A9A9A9</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Customers</ColumnCode><HeaderText>Customers</HeaderText><Value>193</Value><Format>N0</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Products</ColumnCode><HeaderText>Products</HeaderText><Value>50</Value><Format>N0</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Promotions</ColumnCode><HeaderText>Promotions</HeaderText><Value>3</Value><Format>N0</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>IsActiveBudget</ColumnCode><HeaderText>Active Budget</HeaderText><Value>false</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>CheckBox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute></Attributes></RootItem><RootItem><Item_Idx>2</Item_Idx><Item_Type>Scenario</Item_Type><Item_RowSortOrder>2</Item_RowSortOrder><Attributes><Attribute><ColumnCode>IsSelected</ColumnCode><HeaderText /><Value>false</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Checkbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Scen_Name</ColumnCode><HeaderText>Scenario Name</HeaderText><Value>(CH-1) - test</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Hyperlink</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>COUNT</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Author</ColumnCode><HeaderText>Author</HeaderText><Value>Hogan, Craig</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Start_Day</ColumnCode><HeaderText>Start Date</HeaderText><Value>2015-01-02</Value><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>End_Day</ColumnCode><HeaderText>End Date</HeaderText><Value>2017-01-01</Value><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Create_Day</ColumnCode><HeaderText>Created On</HeaderText><Value>2015-06-08</Value><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Type_Name</ColumnCode><HeaderText>Type</HeaderText><Value>User Scenario</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Status_Name</ColumnCode><HeaderText>Status</HeaderText><Value>Open</Value><Format /><ForeColour>#000000</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Customers</ColumnCode><HeaderText>Customers</HeaderText><Value>17</Value><Format>N0</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Products</ColumnCode><HeaderText>Products</HeaderText><Value>42</Value><Format>N0</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Promotions</ColumnCode><HeaderText>Promotions</HeaderText><Value>0</Value><Format>N0</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>IsActiveBudget</ColumnCode><HeaderText>Active Budget</HeaderText><Value>false</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>CheckBox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute></Attributes></RootItem><RootItem><Item_Idx>3</Item_Idx><Item_Type>Scenario</Item_Type><Item_RowSortOrder>3</Item_RowSortOrder><Attributes><Attribute><ColumnCode>IsSelected</ColumnCode><HeaderText /><Value>false</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Checkbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Scen_Name</ColumnCode><HeaderText>Scenario Name</HeaderText><Value>(CH-2) - test</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Hyperlink</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>COUNT</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Author</ColumnCode><HeaderText>Author</HeaderText><Value>Hogan, Craig</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Start_Day</ColumnCode><HeaderText>Start Date</HeaderText><Value>2015-01-02</Value><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>End_Day</ColumnCode><HeaderText>End Date</HeaderText><Value>2017-01-01</Value><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Create_Day</ColumnCode><HeaderText>Created On</HeaderText><Value>2015-06-08</Value><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Type_Name</ColumnCode><HeaderText>Type</HeaderText><Value>User Scenario</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Status_Name</ColumnCode><HeaderText>Status</HeaderText><Value>Open</Value><Format /><ForeColour>#000000</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Customers</ColumnCode><HeaderText>Customers</HeaderText><Value>17</Value><Format>N0</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Products</ColumnCode><HeaderText>Products</HeaderText><Value>42</Value><Format>N0</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Promotions</ColumnCode><HeaderText>Promotions</HeaderText><Value>0</Value><Format>N0</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>IsActiveBudget</ColumnCode><HeaderText>Active Budget</HeaderText><Value>false</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>CheckBox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute></Attributes></RootItem></Results>";


        //private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        //{
        //    var rg = new Record();
        //    rg.Item_Idx = "-1";


        //    rg.Properties = _gvm.RVM.Records[0].Clone();

        //    _gvm.RVM.Records.Add(rg);

        //}
    }

    //public class ControlTemplateSelector : DataTemplateSelector
    //{
    //    public DataTemplate HorizontalGridTemplate { get; set; }
    //    public DataTemplate VerticalGridTemplate { get; set; }
    //    public DataTemplate ChartTemplate { get; set; }
    //    public DataTemplate DefaultTemplate { get; set; }

    //    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    //    {
    //        var cf = (Property)item;
    //        switch (cf.ControlType)
    //        {
    //            case "HorizontalGrid": return HorizontalGridTemplate;
    //            case "VerticalGrid": return VerticalGridTemplate;
    //            case "Chart": return ChartTemplate;
    //            default: return DefaultTemplate;
    //        }
 
           
    //        return null;
    //    }

    //}
}
