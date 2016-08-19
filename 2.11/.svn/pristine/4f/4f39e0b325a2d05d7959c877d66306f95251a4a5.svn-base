
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;

using info.lundin.math;
using System.Windows;
using System.Globalization;
using System.Text.RegularExpressions;
using Exceedra.Common;
using Exceedra.Common.Utilities;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.DynamicTab.ViewModels;
using Exceedra.DynamicGrid.Models;
using Property = Exceedra.Controls.DynamicGrid.Models.Property;
using System.Xml.Serialization;

namespace Exceedra.Controls.DynamicGrid.ViewModels
{
    public class RecordViewModel : RecordContainer
    {
        /* Basic constructor. Used when we want an initial loading wheel, but need to wait for other data before we can begin loading this grid */
        public RecordViewModel()
        {
            IsDataLoading = true;
            //IsRunning = true;

            //PanelMainMessage = "Loading dynamic data";
            //PanelSubMessage = "...";
        }

        /* Constructor to display no data. */
        public RecordViewModel(bool error)
        {
            IsDataLoading = false;
            GridTitle = "No Data";
        }

        /* Main constructor that will display a loading wheel until the records and totals have been calculated */
        public RecordViewModel(XElement res)
        {
            IsDataLoading = true;
            //IsRunning = true;

            //PanelMainMessage = "Loading dynamic data";
            //PanelSubMessage = "...";


            //Records = new ObservableCollection<Record>();
            LoadRecords2(res);

            if (Records != null &&
                Records.Any(rec => rec.Properties.Any(prop => !String.IsNullOrWhiteSpace(prop.TotalsAggregationMethod))))
                CalulateRecordColumnTotal(Records.FirstOrDefault());

            //ColumnsFormulas();
            HasChanged = false;
            IsDataLoading = false;
            //IsRunning = false;
        }

        /* Using this can cause the loading wheel to not show as the Xaml isn't bound to this VM until this method returns.
         * I have removed its use but left it here incase.
         */
        public static RecordViewModel LoadWithData(XElement res)
        {
            var instance = new RecordViewModel();
            instance.Init(res);

            return instance;
        }


        #region Add/Delete Record

        public void AddRecord(Record newRecord)
        {
            newRecord.PropertyChanged += NewRecordOnPropertyChanged;
            Records.Add(newRecord);
            NotifyPropertyChanged(this, vm => vm.Records);
        }

        private void NewRecordOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HasChanges")
            {
                if (((Record)sender).HasChanges > 0)
                    HasChanged = true;
            }
        }

        public void RemoveRecord(Record record)
        {
            record.PropertyChanged += NewRecordOnPropertyChanged;
            Records.Remove(record);
            NotifyPropertyChanged(this, vm => vm.Records);
        }

        public void AddRecord(List<Property> props, string it, int c)
        {
            var nr = new Record()
            {
                Item_Idx = "0",
                Item_Type = it,
                Item_RowSortOrder = c + 1,
                Item_IsDisplayed = true
            };

            nr.Properties = new ObservableCollection<Property>(props);
            nr.PropertyChanged += NewRecordOnPropertyChanged;
            Records.Add(nr);
            NotifyPropertyChanged(this, vm => vm.Records);
        }

        public void AddRecord(List<Property> props, string it, int c, TabbedViewModel t)
        {
            var nr = new Record()
            {
                Item_Idx = c.ToString(),
                Item_Type = it,
                Item_IsDisplayed = true,
                DetailsViewModel = t
            };

            nr.Properties = new ObservableCollection<Property>(props);
            nr.PropertyChanged += NewRecordOnPropertyChanged;

            Records.Add(nr);
            NotifyPropertyChanged(this, vm => vm.Records);
        }

        public void AddRecord(List<Property> props, string it, int c, string idx)
        {
            var nr = new Record()
            {
                Item_Idx = idx ?? "0",
                Item_Type = it,
                Item_RowSortOrder = c + 1,
                Item_IsDisplayed = true
            };

            nr.Properties = new ObservableCollection<Property>(props);
            nr.PropertyChanged += NewRecordOnPropertyChanged;

            Records.Add(nr);
            NotifyPropertyChanged(this, vm => vm.Records);
        }

        public void DeleteRecord(Record record)
        {
            record.PropertyChanged -= NewRecordOnPropertyChanged;
            Records.Remove(record);
            NotifyPropertyChanged(this, vm => vm.Records);
        }

        #endregion


        #region TreeGridNewXmlIn
        /* This region is for building RVMs for the TreeGrid UserControl. 
         * It requires different defaults to a standard grid, and we are using attribute xml as it is easier to read.
         *
         */

        public static RecordViewModel LoadWithNewXml(XElement xml)
        {
            var instance = new RecordViewModel();

            if (xml.Name == "Planning")
                instance.ConvertPlanningNodeToDynGrid(xml);
            else
                instance.LoadNewStyle(xml);

            instance.IsDataLoading = false;
            return instance;
        }

        public bool CanAddComments { get; set; }

        /* New method to read in attribute xml */
        public void LoadNewStyle(XElement xml)
        {
            Records = new ObservableCollection<Record>();
            var recordXmls = xml.Elements("Row");

            foreach (var recordXml in recordXmls)
            {
                var record = new Record
                {
                    Item_Idx = recordXml.Attribute("Idx").MaybeValue(),
                    Item_Name = recordXml.Attribute("Name").MaybeValue(),
                    Item_Type = recordXml.Attribute("Item_Type").MaybeValue() ?? recordXml.Attribute("Code").MaybeValue() ?? "TreeGridMeasure",
                    Item_Code = recordXml.Attribute("Code").MaybeValue(),
                    Item_IsDisplayed = recordXml.Attribute("IsDisplayed").MaybeValue() == "1",
                    Item_RowSortOrder = recordXml.Attribute("SortOrder").MaybeValue().AsNumericInt(),
                    Item_Colour = recordXml.Element("Measure_Colour").MaybeValue() ?? "#FFFFFFFF",
                    Item_AggrType = recordXml.Attribute("AggrType").MaybeValue(),
                    Properties = new ObservableCollection<Property>()
                };

                var columnXmls = recordXml.Element("Columns").Elements("Column");

                foreach (var columnXml in columnXmls)
                {
                    var background = columnXml.Attribute("BackgroundColour").MaybeValue();
                    if (string.IsNullOrEmpty(background)) background = "transparent";
                    var border = columnXml.Attribute("BorderColour").MaybeValue();
                    if (string.IsNullOrEmpty(border)) border = background;
                    var fore = columnXml.Attribute("ForeColour").MaybeValue();
                    if (string.IsNullOrEmpty(fore)) fore = "#FF000000";

                    record.Properties.Add(new Property
                    {
                        IDX = columnXml.Attribute("Idx").MaybeValue(),
                        ColumnCode = columnXml.Attribute("Idx").MaybeValue(),
                        HeaderText = columnXml.Attribute("Name").MaybeValue(),
                        ControlType = columnXml.Attribute("ControlType").MaybeValue() ?? "treegridcell",
                        IsEditable = columnXml.Attribute("IsEditable").MaybeValue() == "1",
                        StringFormat = columnXml.Attribute("Format").MaybeValue(),
                        BackgroundColour = background,
                        BorderColour = border,
                        ForeColour = fore,
                        Value = columnXml.Attribute("Value").MaybeValue(),
                        Value2 = columnXml.Attribute("Value2").MaybeValue() == "1" ? "true" : "false",
                        Width = columnXml.Attribute("Width").MaybeValue() ?? "80",
                        TotalsAggregationMethod = columnXml.Attribute("TotalsAggregationMethod").MaybeValue(),
                        FitWidth = columnXml.Attribute("FitWidth").MaybeValue(),
                        Alignment = columnXml.Attribute("Alignment").MaybeValue() ?? "Left",
                        IsDisplayed = columnXml.Attribute("IsDisplayed").MaybeValue() == "1",
                        IsExpandable = columnXml.Attribute("IsExpandable").MaybeValue() == "1"
                    });

                }

                Records.Add(record);
            }
        }

        public static Dictionary<string, string> Measures { get; set; }

        public void ConvertPlanningNodeToDynGrid(XElement xml)
        {
            CanAddComments = true;
            var records = new ObservableCollection<Record>();
            var rows = xml.Elements();

            foreach (var row in rows)
            {
                var record = new Record
                {
                    Item_Idx = row.Element("Measure_Idx").MaybeValue(),
                    Item_Name = row.Element("Measure_Name").MaybeValue() ?? row.Element("Measure_Code").MaybeValue(),
                    Item_Code = row.Element("Measure_Code").MaybeValue(),
                    Item_Type = row.Element("Item_Type").MaybeValue() ?? "TreeGridMeasure",
                    Item_IsDisplayed = row.Element("Item_IsDisplayed").MaybeValue() != "0",
                    Item_RowSortOrder = row.Element("Measure_SortOrder").MaybeValue().AsNumericInt(),
                    Item_AggrType = row.Element("Measure_AggrType").MaybeValue(),
                    Item_Colour = row.Element("Measure_Colour").MaybeValue() ?? "#FFFFFFFF",
                    Properties = new ObservableCollection<Property>()
                };

                var columnXmls = row.Element("Columns").Elements("Column");

                foreach (var columnXml in columnXmls)
                {
                    record.Properties.Add(new Property
                    {
                        IDX = columnXml.Attribute("Idx").MaybeValue(),
                        ColumnCode = columnXml.Attribute("Idx").MaybeValue(),
                        HeaderText = columnXml.Attribute("Name").MaybeValue(),
                        ControlType = columnXml.Attribute("ControlType").MaybeValue() ?? "treegridcell",
                        IsEditable = columnXml.Attribute("IsEditable").MaybeValue() == "1",
                        StringFormat = row.Element("Measure_Format").MaybeValue(),
                        BackgroundColour = columnXml.Attribute("BackgroundColour").MaybeValue() ?? columnXml.Attribute("Colour").MaybeValue() ?? "transparent",
                        BorderColour = columnXml.Attribute("BorderColour").MaybeValue() ?? columnXml.Attribute("Colour").MaybeValue() ?? "transparent",
                        ForeColour = columnXml.Attribute("ForeColour").MaybeValue() ?? "#000000",
                        Value = columnXml.Attribute("Value").MaybeValue(),
                        Width = columnXml.Attribute("Width").MaybeValue() ?? "80",
                        TotalsAggregationMethod = columnXml.Attribute("TotalsAggregationMethod").MaybeValue(),
                        FitWidth = columnXml.Attribute("FitWidth").MaybeValue(),
                        Alignment = columnXml.Attribute("Alignment").MaybeValue(),
                        IsDisplayed = true,
                        CommentList = GetComments(columnXml),
                        //Calculation = row.Element("Measure_Calculation").MaybeValue()// Calculation = record.Item_Name == "Last Year Volume" ? "Base#Volume + Base#Override" : null 
                    });
                    if (record.Properties.Last().Value != null && record.Properties.Last().Value.StartsWith("="))
                        record.Properties.Last().Calculation = record.Properties.Last().Value.Trim().TrimStart("=");
                }

                records.Add(record);
            }

            /* Temp code until the db can configure the correct nodes itself */
            var recordCodes = records.Select(r => r.Item_Code);
            RemovedRecords = records.Where(r => r.Properties.Any(p => p.Calculation != null)).Where(r => SplitIntoRecordCodes(r.Properties.First(p => p.Calculation != null).Calculation).Where(code => !recordCodes.Contains(code)).Any()).Distinct();

            Records = new ObservableCollection<Record>(records.Except(RemovedRecords));
        }

        [XmlIgnore]
        public IEnumerable<Record> RemovedRecords{ get; set; }

        private List<Model.Entity.Generic.ListboxComment> GetComments(XElement xml)
        {
            var commentsXml = xml.Element("Comments").MaybeElements();

            if (!commentsXml.Any()) return new List<Model.Entity.Generic.ListboxComment>();

            return commentsXml.Select(c => new Model.Entity.Generic.ListboxComment { Value = c.Attribute("Value").MaybeValue(), TimeStamp = c.Attribute("TimeStamp").MaybeValue(), UserName = c.Attribute("UserName").MaybeValue(), Idx = c.Attribute("Idx").MaybeValue().AsNumericInt() }).ToList();
        }


        #endregion

        public void Init(XElement res)
        {
            //Records = new ObservableCollection<Record>();
            LoadRecords2(res);

            if (Records != null &&
                Records.Any(rec => rec.Properties.Any(prop => !String.IsNullOrWhiteSpace(prop.TotalsAggregationMethod))))
                CalulateRecordColumnTotal(Records.FirstOrDefault());

            //ColumnsFormulas();
            IsDataLoading = false;
            HasChanged = false;
        }

        //public void Init(List<Record> inputRecords)
        //{
        //    LoadRecords2(inputRecords);

        //    if (Records != null &&
        //        Records.Any(rec => rec.Properties.Any(prop => !String.IsNullOrWhiteSpace(prop.TotalsAggregationMethod))))
        //        CalulateRecordColumnTotal(Records.FirstOrDefault());

        //    //ColumnsFormulas();
        //    HasChanged = false;
        //}

        public string ColumnsFormulas(Property col)
        {
            //foreach(var record in Records)
            //{
            //    foreach (var property in record.Properties.Where(p => p.Calculation != null))
            //    {
            //        if(property.Calculation.Contains("IF(COUNTIF"))
            //        {
            if (col.Calculation.Contains("%"))
            {
                string columnCode = col.Calculation.Split(new char[] { ',', ',' })[2].Trim();

                var sendingProperty = (from rec in Records
                                       where rec.Item_Idx == col.IDX
                                       select rec.Properties.Where(a => a.ColumnCode == columnCode).FirstOrDefault()).FirstOrDefault();
                //record.Properties.Where(b => b.ColumnCode == columnCode).FirstOrDefault();

                if (sendingProperty.Value.Contains("%"))
                {
                    string columnCode1 = col.Calculation.Split(new char[] { ',', '*' })[3].Trim();
                    string columnCode2 = col.Calculation.Split(new char[] { '*', '/' })[1].Trim();

                    var column1 = (from rec in Records
                                   where rec.Item_Idx == col.IDX
                                   select rec.Properties.Where(a => a.ColumnCode == columnCode1).FirstOrDefault()).FirstOrDefault();
                    //record.Properties.Where(c => c.ColumnCode == columnCode1).First(); 
                    var column2 = (from rec in Records
                                   where rec.Item_Idx == col.IDX
                                   select rec.Properties.Where(a => a.ColumnCode == columnCode2).FirstOrDefault()).FirstOrDefault();
                    //record.Properties.Where(d => d.ColumnCode == columnCode2).First();

                    //record.Properties.Where(c => c.ColumnCode == columnCode1).First().PropertyChanged += ColumnProperty_PropertyChanged;
                    //record.Properties.Where(d => d.ColumnCode == columnCode2).First().PropertyChanged += ColumnProperty_PropertyChanged;

                    double column1Double;
                    double.TryParse(column1.Value, NumberStyles.Any, CultureInfo.CurrentCulture, out column1Double);

                    var trimmedCol2Value = column2.Value.Replace(@"%", "");
                    double column2Double;
                    double.TryParse(trimmedCol2Value.Trim(), out column2Double);

                    double outputDouble = (column1Double * column2Double) / 100;

                    return outputDouble.ToString();
                }
                else
                {

                    //record.Properties.Where(b => b.ColumnCode == columnCode).FirstOrDefault().PropertyChanged += ColumnProperty_PropertyChanged;

                    return sendingProperty.Value;
                }
            }
            return null;
            //property.PropertyChanged += ColumnProperty_PropertyChanged;

            //        }
            //    }
            //}
        }



        //private void ColumnProperty_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //        ColumnsFormulas();
        //}

        private ObservableCollection<Record> _records;

        // I know it's lame but I was in a rush, sorry!
        public void NotifyRecordsChanged()
        {
            NotifyPropertyChanged(this, vm => vm.Records);
        }

        public ObservableCollection<Record> Records
        {
            get { return _records; }
            set
            {
                if (_records != value)
                {
                    _records = value;
                    HasChanged = true;
                    NotifyPropertyChanged(this, vm => vm.Records);
                }

                if (_records != null && _records.Any())
                {
                    // IsDataLoading = false;
                    // IsRunning = false; 
                    //PanelSubMessage = _records.Count.ToString() + " results loaded";
                }
                else
                {
                    //IsDataLoading = true;
                    //IsRunning = false;

                    //PanelMainMessage = "No Data found";

                }


            }
        }

        private ObservableCollection<Record> _results;

        public ObservableCollection<Record> Results
        {
            get { return _results; }
            set
            {
                if (_results != value)
                {
                    _results = value;
                    NotifyPropertyChanged(this, vm => vm.Results);

                }
            }
        }

        private ObservableCollection<HeaderRow> _headers;

        public ObservableCollection<HeaderRow> Headers
        {
            get { return _headers; }
            set
            {
                if (_headers != value)
                {
                    _headers = value;
                    NotifyPropertyChanged(this, vm => vm.Headers);
                }

            }
        }

        private string _gridTitle;

        public string GridTitle
        {
            get { return _gridTitle; }
            set
            {
                _gridTitle = string.IsNullOrWhiteSpace(value) ? null : value;
                NotifyPropertyChanged(this, vm => vm.GridTitle);
            }
        }

        private bool _popOutNav;

        public bool PopOutNavigation
        {
            get { return _popOutNav; }
            set
            {
                _popOutNav = value;
                NotifyPropertyChanged(this, vm => vm.PopOutNavigation);
            }
        }

        private bool _isFilterVisible;

        public bool IsFilterVisible
        {
            get { return _isFilterVisible; }
            set
            {
                _isFilterVisible = value;
                NotifyPropertyChanged(this, vm => vm.IsFilterVisible);
            }
        }

        /* TEST XML:
         * Expader grid xml: res = XElement.Parse("<Results><Grid_Title>Test</Grid_Title><RootItem><Item_Idx>365</Item_Idx><Item_Type>Product_Details</Item_Type><Item_RowSortOrder>365</Item_RowSortOrder><Attributes><Attribute><ColumnCode>Sku_Idx</ColumnCode><HeaderText>ID</HeaderText><Value>365</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DataSourceInput /><DependentColumns /><UpdateToColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod>         <ExternalData /><ColumnSortOrder>1</ColumnSortOrder></Attribute><Attribute><ColumnCode>SKU_CODE</ColumnCode><HeaderText>CODE</HeaderText><Value>000000000002001252</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DataSourceInput /><DependentColumns /><UpdateToColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod>         <ExternalData /><ColumnSortOrder>1</ColumnSortOrder></Attribute><Attribute><ColumnCode>SKU_NAME</ColumnCode><HeaderText>Name</HeaderText><Value>Material1</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DataSourceInput /><DependentColumns /><UpdateToColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod>         <ExternalData /><ColumnSortOrder>1</ColumnSortOrder></Attribute><Attribute><ColumnCode>TabbedView</ColumnCode><HeaderText>TabbedView</HeaderText><Value /><Format /><ForeColour /><BorderColour /><IsDisplayed>0</IsDisplayed><IsEditable>0</IsEditable><ControlType>HorizontalGrid</ControlType><DataSource>clnt.Procast_SP_BR01_GetHierarchyDetails</DataSource>         <DataSourceInput>&lt;Sku_Idx&gt;365&lt;/Sku_Idx&gt;</DataSourceInput>         <DependentColumns /><UpdateToColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod>         <ExternalData /><ColumnSortOrder>4</ColumnSortOrder></Attribute></Attributes></RootItem></Results>");
         * Add others...
         * 
         */


        public List<Record> LoadRecords2(XElement res)
        {
            if (res == null)
                return null;

            GridTitle = res.Element("Grid_Title").MaybeValue();
            IsFilterVisible = res.Element("Grid_FilterIsVisible").MaybeValue() == "1";

            var loadedRecords = new List<Record>();

            var xUseAttributes = res.Element("UseAttributes");
            if (xUseAttributes != null && xUseAttributes.Value == "1")
            {
                #region load using an xml with attributes

                #region load template

                Record templateRecord = null;

                var xItemTemplate = res.Element("ItemTemplate");
                if (xItemTemplate != null)
                {
                    templateRecord = new Record
                    {
                        Item_Type = xItemTemplate.Attribute("Item_Type").MaybeValue(),
                        Item_Name = xItemTemplate.Attribute("Item_Name").MaybeValue(),
                        Properties = new ObservableCollection<Property>()
                    };

                    var xAttributes = xItemTemplate.Element("Attributes");
                    if (xAttributes != null)
                    {
                        foreach (var xAttribute in xAttributes.Elements("Attribute"))
                        {
                            var templateProperty = xmlToProperty(xAttribute, true);
                            templateRecord.Properties.Add(templateProperty);
                        }
                    }
                }

                #endregion

                foreach (var xRootItem in res.Elements("RootItem"))
                {
                    #region calculate row sort order

                    int rowSortOrder = 0;
                    var xRowSortOrder = xRootItem.Attribute("Item_RowSortOrder");
                    if (xRowSortOrder != null)
                        int.TryParse(xRowSortOrder.Value, out rowSortOrder);

                    while (loadedRecords.Any(r => r.Item_RowSortOrder == rowSortOrder)) rowSortOrder++;

                    #endregion

                    var record = new Record()
                    {
                        Item_Idx = xRootItem.Attribute("Item_Idx").MaybeValue() ?? xRootItem.Attribute("Idx").MaybeValue(),
                        Item_Type = xRootItem.Attribute("Item_Type").MaybeValue() ?? ( templateRecord != null ? templateRecord.Item_Type : null ),
                        Item_Name = xRootItem.Attribute("Item_Name").MaybeValue() ?? ( templateRecord != null ? templateRecord.Item_Name : null ),
                        Item_RowSortOrder = rowSortOrder,
                        Item_IsDisplayed = (xRootItem.Attribute("Item_IsDisplayed").MaybeValue() ?? "1") == "1"
                    };

                    var xAttributes = xRootItem.Element("Attributes");
                    if (xAttributes != null)
                    {
                        var properties = new List<Property>();
                        var xAttributeList = xAttributes.Elements("Attribute").ToList();

                        if (templateRecord != null)
                        {
                            foreach (var templateProperty in templateRecord.Properties)
                            {
                                var xAttribute = xAttributeList.FirstOrDefault(xAttr => xAttr.Attribute("ColumnCode").Value == templateProperty.ColumnCode);
                                if (xAttribute == null) xAttribute = new XElement("Missing");

                                Property property = xmlToProperty(xAttribute, templateProperty, true);
                                property.IDX = record.Item_Idx;
                                property.Type = record.Item_Type;

                                properties.Add(property);
                            }
                        }
                        else
                        {
                            foreach (var xAttribute in xAttributeList)
                            {
                                Property property = xmlToProperty(xAttribute, true);

                                property.IDX = record.Item_Idx;
                                property.Type = record.Item_Type;

                                properties.Add(property);
                            }
                        }

                        record.Properties = new ObservableCollection<Property>(properties);
                        record.PropertyChanged += NewRecordOnPropertyChanged;

                        loadedRecords.Add(record);
                    }
                }

                #endregion
            }
            else
            {
                #region load using a regular xml - without attributes

                var ri = (from r in res.Descendants("RootItem")
                          select r).ToList();


                foreach (var rec in ri)
                {
                    int rso = 0;
                    var sortOrder = rec.Element("Item_RowSortOrder");
                    if (sortOrder != null)
                        int.TryParse(sortOrder.Value, out rso);

                    while (loadedRecords.Any(record => record.Item_RowSortOrder == rso)) rso++;

                    var nr = new Record()
                    {
                        Item_Idx = rec.Element("Item_Idx").MaybeValue() ?? rec.Element("Idx").MaybeValue(),
                        Item_Type = rec.Element("Item_Type").MaybeValue(),
                        Item_Name = rec.Element("Item_Name").MaybeValue(),
                        Item_RowSortOrder = rso,
                        Item_IsDisplayed = (rec.Element("Item_IsDisplayed").MaybeValue() ?? "1") == "1"
                    };
                    var depCols = (from r in rec.Descendants("DependentColumns") select r).ToList();
                    var depCol = depCols.Elements().FirstOrDefault();
                    var attr = (from r in rec.Descendants("Attributes") select r).ToList();

                    var props = new List<Property>();
                    foreach (var att in attr.Elements("Attribute"))
                    {
                        var p = xmlToProperty(att);
                        p.IDX = nr.Item_Idx;
                        p.Type = nr.Item_Type;

                        props.Add(p);

                    }
                    nr.Properties = new ObservableCollection<Property>(props);
                    nr.PropertyChanged += NewRecordOnPropertyChanged;

                    loadedRecords.Add(nr);
                }

                #endregion
            }

            GenerateHeadersColumns(loadedRecords);
            Records = new ObservableCollection<Record>(loadedRecords);
            return CalculateColumns(loadedRecords);
        }

        public Property xmlToProperty(XElement xml, bool useAttributes = false)
        {
            var p = new Property();

            if (useAttributes)
            {
                #region read property using attributes

                p.ColumnCode = xml.Attribute("ColumnCode").Value;

                p.Alignment = xml.Attribute("Alignment").MaybeValue("Left");
                #region formatting alignment

                switch (p.Alignment.ToLower())
                {
                    case "r":
                        p.Alignment = "Right";
                        break;
                    case "c":
                        p.Alignment = "Center";
                        break;
                    case "s":
                        p.Alignment = "Stretch";
                        break;
                }

                #endregion

                p.BorderColour = xml.Attribute("BorderColour").MaybeValue("transparent");
                p.BackgroundColour = xml.Attribute("BackgroundColour").MaybeValue("transparent");
                p.ColumnSortOrder = int.Parse(xml.Attribute("ColumnSortOrder").MaybeValue("0"));
                p.ControlType = xml.Attribute("ControlType").MaybeValue("TextBox");
                p.DataSource = xml.Attribute("DataSource").MaybeValue(string.Empty);
                p.DataSourceInput = xml.Attribute("DataSourceInput").MaybeValue(string.Empty);
                p.DependentColumn = xml.Element("DependentColumns").MaybeValue(string.Empty);
                p.External_Data = xml.Attribute("ExternalData").MaybeValue();
                p.FitWidth = xml.Attribute("FitWidth").MaybeValue(string.Empty); // Content or Header
                p.ForeColour = xml.Attribute("ForeColour").MaybeValue("#000000");
                p.HeaderText = xml.Attribute("HeaderText").MaybeValue(string.Empty);
                p.IsDisplayed = xml.Attribute("IsDisplayed").MaybeValue("1") == "1";
                p.IsEditable = xml.Attribute("IsEditable").MaybeValue("0") == "1";
                p.IsRequired = xml.Attribute("IsRequired").MaybeValue("0") == "1";
                p.TotalsAggregationMethod = xml.Attribute("TotalsAggregationMethod").MaybeValue(string.Empty);
                p.UpdateToColumn = xml.Attribute("UpdateToColumn").MaybeValue(string.Empty);
                p.UpdateToCell = xml.Attribute("UpdateToCell").MaybeValue(string.Empty);
                p.Width = xml.Attribute("Width").MaybeValue(string.Empty);

                p.StringFormat = xml.Attribute("Format").MaybeValue(string.Empty);
                if (p.StringFormat == "P ") p.StringFormat = "P0";

                p.Value = xml.Attribute("Value").MaybeValue(string.Empty);

                #region formatting value

                if (p.StringFormat.ToLower() == "shortdate" && (p.Value.Contains("-") || p.Value.Contains('/')))
                {
                    DateTime dateTimeValue;
                    DateTime.TryParse(p.Value, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTimeValue);

                    // if the value could be anyhow successfully parsed into a DateTime..
                    if (dateTimeValue != new DateTime())
                        p.Value = dateTimeValue.ToShortDateString();
                }

                #endregion

                p.Value2 = xml.Attribute("Value2").MaybeValue();

                if (p.DataSource == string.Empty && xml.Element("Values") != null)
                    p.Values = Option.GetFromXML(xml.Element("Values"));


                if (p.Value.StartsWith("="))
                    p.Calculation = p.Value;

                p.HeaderRowData = GetHeaderRow(xml.Element("HeaderRow"), p.IsDisplayed, p.ColumnCode);

                #endregion
            }
            else
            {
                #region read property without using attributes

                p.External_Data = xml.Element("ExternalData").MaybeValue();

                p.ColumnCode = xml.Element("ColumnCode").Value;

                p.IsDisplayed = (xml.Element("IsDisplayed").MaybeValue() ?? "1") == "1";
                //(att.Element("IsDisplayed").Value == "1" ? true : false);
                p.HeaderRowData = GetHeaderRow(xml.Element("HeaderRow"), p.IsDisplayed, p.ColumnCode);


                p.HeaderText = (xml.Element("HeaderText") == null || xml.Element("HeaderText").Value == ""
                    ? ""
                    : xml.Element("HeaderText").Value);
                p.StringFormat = (xml.Element("Format") == null || xml.Element("Format").Value == ""
                    ? ""
                    : xml.Element("Format").Value);

                if (p.StringFormat == "P ")
                {
                    p.StringFormat = "P0";
                }
                p.Value = (xml.Element("Value") == null || xml.Element("Value").Value == ""
                    ? ""
                    : xml.Element("Value").Value);

                #region formatting to date time if the Value is a date

                if (p.StringFormat.ToLower() == "shortdate" && (p.Value.Contains("-") || p.Value.Contains('/')))
                {
                    DateTime dateTimeValue;
                    DateTime.TryParse(p.Value, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTimeValue);

                    // if the value could be anyhow successfully parsed into a DateTime..
                    if (dateTimeValue != new DateTime())
                        p.Value = dateTimeValue.ToShortDateString();
                }

                #endregion

                //if (p.StringFormat != "")
                //    p.Value = string.Format("{0:" + p.StringFormat + "}", p.Value);


                p.ForeColour = (xml.Element("ForeColour") == null || xml.Element("ForeColour").Value == ""
                    ? "#000000"
                    : xml.Element("ForeColour").Value);
                p.BorderColour = (xml.Element("BorderColour") == null || xml.Element("BorderColour").Value == ""
                    ? "transparent"
                    : xml.Element("BorderColour").Value);
                p.BackgroundColour = (xml.Element("BackgroundColour") == null ||
                                      xml.Element("BackgroundColour").Value == ""
                    ? "transparent"
                    : xml.Element("BackgroundColour").Value);


                p.IsEditable = (xml.Element("IsEditable").MaybeValue() == "1" ? true : false);
                p.IsRequired = xml != null && xml.Element("IsRequired") != null &&
                               xml.Element("IsRequired").Value == "1";
                p.ControlType = (xml.Element("ControlType") == null ? "TextBox" : xml.Element("ControlType").Value);

                p.DataSource = (xml.Element("DataSource") == null ? "" : xml.Element("DataSource").Value);
                p.DataSourceInput = (xml.Element("DataSourceInput") == null
                    ? ""
                    : xml.Element("DataSourceInput").ToString());
                p.DependentColumn = (xml.Element("DependentColumns") == null
                    ? ""
                    : xml.Element("DependentColumns").Value);

                //p.DependentColumn = (depCol == null ? "" : depCol.Value);

                p.UpdateToColumn = (xml.Element("UpdateToColumn") == null ? "" : xml.Element("UpdateToColumn").Value);
                p.UpdateToCell = xml.Element("UpdateToCell").MaybeValue() ?? "";
                p.Value2 = xml.Element("Value2").MaybeValue();

                p.TotalsAggregationMethod = (xml.Element("TotalsAggregationMethod") == null
                    ? ""
                    : xml.Element("TotalsAggregationMethod").Value);

                //take dropdown values from Values node if there is no data source option
                if (p.DataSource == "" && xml.Element("Values") != null)
                {
                    p.Values = Option.GetFromXML(xml.Element("Values"));
                }

                if (p.Value.StartsWith("="))
                {
                    p.Calculation = p.Value;
                }

                p.FitWidth = (xml.Element("FitWidth").MaybeValue() ?? ""); // Content or Header
                p.Alignment = "Left";

                if (xml.Element("Alignment") != null)
                {
                    switch (xml.Element("Alignment").Value.ToLower())
                    {
                        case "r":
                            p.Alignment = "Right";
                            break;
                        case "c":
                            p.Alignment = "Center";
                            break;
                        case "s":
                            p.Alignment = "Stretch";
                            break;
                    }
                }

                p.Width = (xml.Element("Width") == null ? "" : xml.Element("Width").Value); // pixel width

                p.ColumnSortOrder = xml.Element("ColumnSortOrder") != null ? int.Parse(xml.Element("ColumnSortOrder").MaybeValue()) : 0;

                #endregion
            }

            return p;
        }

        public Property xmlToProperty(XElement xml, Property template, bool useAttributes = false)
        {
            if (template == null) return xmlToProperty(xml, useAttributes);

            var p = new Property();

            if (useAttributes)
            {
                p.ColumnCode = xml.Attribute("ColumnCode").MaybeValue(template.ColumnCode).MaybeValue(null);

                p.Alignment = xml.Attribute("Alignment").MaybeValue(template.Alignment).MaybeValue("Left");
                #region formatting alignment

                switch (p.Alignment.ToLower())
                {
                    case "r":
                        p.Alignment = "Right";
                        break;
                    case "c":
                        p.Alignment = "Center";
                        break;
                    case "s":
                        p.Alignment = "Stretch";
                        break;
                }

                #endregion

                p.BorderColour = xml.Attribute("BorderColour").MaybeValue(template.BorderColour).MaybeValue("transparent");
                p.BackgroundColour = xml.Attribute("BackgroundColour").MaybeValue(template.BackgroundColour).MaybeValue("transparent");
                p.ColumnSortOrder = xml.Attribute("ColumnSortOrder") != null ? int.Parse(xml.Attribute("ColumnSortOrder").Value) : template.ColumnSortOrder;
                p.ControlType = xml.Attribute("ControlType").MaybeValue(template.ControlType).MaybeValue("TextBox");
                p.DataSource = xml.Attribute("DataSource").MaybeValue(template.DataSource).MaybeValue(string.Empty);
                p.DataSourceInput = xml.Attribute("DataSourceInput").MaybeValue(template.DataSourceInput).MaybeValue(string.Empty);
                p.DependentColumn = xml.Element("DependentColumns").MaybeValue(template.DependentColumn).MaybeValue(string.Empty);
                p.External_Data = xml.Attribute("ExternalData").MaybeValue(template.External_Data);
                p.FitWidth = xml.Attribute("FitWidth").MaybeValue(template.FitWidth).MaybeValue(string.Empty); // Content or Header
                p.ForeColour = xml.Attribute("ForeColour").MaybeValue(template.ForeColour).MaybeValue("#000000");
                p.HeaderText = xml.Attribute("HeaderText").MaybeValue(template.HeaderText.MaybeValue(string.Empty));
                p.IsDisplayed = xml.Attribute("IsDisplayed") != null ? xml.Attribute("IsDisplayed").Value == "1" : template.IsDisplayed;
                p.IsEditable = xml.Attribute("IsEditable") != null ? xml.Attribute("IsEditable").Value == "1" : template.IsEditable;
                p.IsRequired = xml.Attribute("IsRequired") != null ? xml.Attribute("IsRequired").Value == "1" : template.IsRequired;
                p.TotalsAggregationMethod = xml.Attribute("TotalsAggregationMethod").MaybeValue(template.TotalsAggregationMethod).MaybeValue(string.Empty);
                p.UpdateToColumn = xml.Attribute("UpdateToColumn").MaybeValue(template.UpdateToColumn).MaybeValue(string.Empty);
                p.UpdateToCell = xml.Attribute("UpdateToCell").MaybeValue(template.UpdateToCell).MaybeValue(string.Empty);
                p.Width = xml.Attribute("Width").MaybeValue(template.Width).MaybeValue(string.Empty);

                p.StringFormat = xml.Attribute("Format").MaybeValue(template.StringFormat).MaybeValue(string.Empty);
                if (p.StringFormat == "P ") p.StringFormat = "P0";

                p.Value = xml.Attribute("Value").MaybeValue(template.Value).MaybeValue(string.Empty);
                #region formatting value

                if (p.StringFormat.ToLower() == "shortdate" && (p.Value.Contains("-") || p.Value.Contains('/')))
                {
                    DateTime dateTimeValue;
                    DateTime.TryParse(p.Value, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTimeValue);

                    // if the value could be anyhow successfully parsed into a DateTime..
                    if (dateTimeValue != new DateTime())
                        p.Value = dateTimeValue.ToShortDateString();
                }

                #endregion

                p.Value2 = xml.Attribute("Value2").MaybeValue(template.Value2);

                if (p.DataSource == string.Empty && xml.Element("Values") != null)
                    p.Values = Option.GetFromXML(xml.Element("Values"));
                else if(template.Values != null) p.Values = template.Values;


                if (p.Value.StartsWith("="))
                    p.Calculation = p.Value;


                p.HeaderRowData = xml.Element("HeaderRow") != null
                    ? GetHeaderRow(xml.Element("HeaderRow"), p.IsDisplayed, p.ColumnCode)
                    : template.HeaderRowData;
            }
            else throw new Exception("Not implemented yet.");

            return p;
        }

        // private static bool x = false;

        private HeaderRow GetHeaderRow(XElement xElement, bool vis, string c)
        {
            var nhl = new HeaderRow();
            nhl.Operations = new List<HeaderOperation>();

            var nh = new HeaderOperation()
            {
                ParentColumnCode = c,
                Type = "",
                ProRatingColumnCode = "",
                OperationLabel = "",
                Calculation = "",
                Visibility = Visibility.Hidden //(vis ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden) 
            };
            if (xElement == null || xElement.Elements("HeaderOperation") == null)
            {
                nhl.Operations.Add(nh);
                return nhl;
            }

            foreach (var att in xElement.Elements("HeaderOperation"))
            {
                var nh2 = new HeaderOperation()
                {
                    ParentColumnCode = c,
                    Type = "",
                    ProRatingColumnCode = "",
                    OperationLabel = "",
                    Calculation = "",
                    Visibility = Visibility.Hidden//(vis ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden) 
                };

                nh2.OperationLabel = (att.Element("Label") == null || att.Element("Label").Value == "" ? "" : att.Element("Label").Value);
                nh2.Type = (att.Element("Type") == null || att.Element("Type").Value == "" ? "" : att.Element("Type").Value);
                nh2.ProRatingColumnCode = (att.Element("ProRatingColumnCode") == null || att.Element("ProRatingColumnCode").Value == "" ? "" : att.Element("ProRatingColumnCode").Value);
                nh2.Calculation = (att.Element("Calculation") == null || att.Element("Calculation").Value == "" ? "" : att.Element("Calculation").Value);
                //nh2.Visibility = (nh2.Type == "" ? Visibility.Hidden : Visibility.Visible);
                //nh2.Visible = (nh.ProRatingColumnCode != "" ? "System.Windows.Visibility.Visible" : "System.Windows.Visibility.Hidden");
                nhl.Operations.Add(nh2);
            }
            return nhl;

        }

        public List<Record> CalculateColumns(List<Record> loadedRecords)
        {
            foreach (var records in loadedRecords)
            {
                //find calc columns and do stuff
                var r = records;
                r = CalulateRecordColumns(records);

            }

            return loadedRecords;
        }

        public void GenerateHeadersColumns(List<Record> loadedRecords)
        {
            var headers = new List<HeaderRow>();

            if (!loadedRecords.Any()) return;

            var current = 0;
            var numColumns = loadedRecords[0].Properties.Count();

            //find headers columns and add an operation column for each property
            foreach (var col in loadedRecords[0].Properties)
            {


                //If this column has a header operation...
                if (col.HeaderRowData.Operations.Any(r => r.Type != ""))
                {
                    //poll all operations and add them to the correct section of the header
                    foreach (var x in col.HeaderRowData.Operations)
                    {

                        var h = headers.SingleOrDefault(r => r.Label == x.OperationLabel);

                        //If we don't have this header yet, add it...
                        if (h == null)
                        { // add a new header row with the new label and operations to match number of properties

                            var operations = new List<HeaderOperation>();

                            for (int i = 0; i < numColumns; i++)
                            {
                                operations.Add(new HeaderOperation
                                {
                                    ParentColumnCode = loadedRecords[0].Properties[i].ColumnCode,
                                    ProRatingColumnCode = "",
                                    OperationLabel = "",
                                    Type = ""
                                });
                            }

                            //set first column to = label for operation
                            operations[0].OperationLabel = x.OperationLabel;

                            h = new HeaderRow { Label = x.OperationLabel, Operations = operations };
                            headers.Add(h);
                        }

                        //does the operation already exist 
                        //var operation = headers.Where(r=>r.Label == x.Label).SingleOrDefault().Operations.SingleOrDefault(r => r.ColumnCode == x.ColumnCode && r.Type == x.Type && r.Label== x.Label);
                        //if (operation != null)
                        //{
                        //    // aaaaagghhh (shouldn't)
                        //}
                        //else
                        //{                                
                        //h.Operations[current].ParentColumnCode = col.ColumnCode;
                        h.Operations[current].Calculation = (x.Calculation ?? "");
                        h.Operations[current].ProRatingColumnCode = (x.ProRatingColumnCode ?? "");
                        h.Operations[current].OperationLabel = x.OperationLabel;
                        h.Operations[current].Type = x.Type;
                        h.Operations[current].Visibility = x.Visibility;
                        // }



                    }

                }
                else
                {

                    var h = headers.SingleOrDefault(r => r.Label == "");

                    if (h == null)
                    { // add a new header row with the new label, add em

                        var ops = new List<HeaderOperation>();

                        for (int i = 0; i < numColumns; i++)
                        {
                            ops.Add(new HeaderOperation() { ProRatingColumnCode = "", OperationLabel = "", Type = "" });
                        }

                        //set first column to = label for operation
                        ops[0].OperationLabel = "";

                        h = new HeaderRow() { Label = "", Operations = ops };
                        headers.Add(h);

                    }

                    h.Operations[current].ParentColumnCode = col.ColumnCode;
                    h.Operations[current].ProRatingColumnCode = "";
                    h.Operations[current].OperationLabel = "";
                    h.Operations[current].Type = "";
                    h.Operations[current].Visibility = Visibility.Collapsed;



                }

                current++;
            }




            headers = headers.Where(r => r.Label != "").ToList();

            if (headers.Count() > 0)
                Headers = new ObservableCollection<HeaderRow>(headers);
        }

        /// <summary>
        /// Key - column code
        /// Value - value in this column
        /// </summary>
        private List<FilterEntry> CalculationFilters { get; set; }

        private class FilterEntry
        {
            public string ColumnCode { get; set; }
            public string Value { get; set; }
        }

        /* Checks all records in a column for a calculation and updates any that exist.  */
        public IEnumerable<Record> CalculateColumn(string columnCode)
        {
            var validRecords = Records.Where(r => r.GetProperty(columnCode).Calculation != null);
            var properties = validRecords.Select(r => r.GetProperty(columnCode));

            properties.Do(p => p.Locked = true);

            properties.Do(p => 
            {
                var calculatedValue = "";
                if (p.Calculation.Contains("IF("))                                    
                    calculatedValue = EvaluateIf(p.Calculation.Remove(0, 1), p.ColumnCode);                
                else
                    calculatedValue = ConvertCodesToValues(p.Calculation, p.ColumnCode);

                ExpressionParser parser = new ExpressionParser();
                var calcValue = parser.Parse(calculatedValue);

                p.Value = p.FormatValue(calcValue.ToString());
            });

            properties.Do(p => p.Locked = false);

            return validRecords;
        }

        public void CalulateRecordColumns()
        {
            if (Records == null)
                return;

            foreach (var r in Records)
            {
                CalulateRecordColumns(r);
            }

            CalulateRecordColumnTotal();
        }

        internal Record CalulateRecordColumns(string rowIdx, string columnIdx)
        {
            var record = GetRecord(rowIdx);
            var col = GetProperty(columnIdx, record);

            HorizontalCalculation(col, record);

            return record;

        }

        public Record CalulateRecordColumns(Record record)
        {
            var invalid = new[] { "+", "-", "*", "/", ".", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "<", ">" };

            foreach (var col in record.Properties)
            {
                HorizontalCalculation(col, record);
                //if (col.Calculation != null)
                //{
                //    //right this is one that has a calc value
                //    var input = col.Calculation.Replace("=", ""); //  <Value>=Num_Customers + Num_Products + Num_Promotions + Num_Terms</Value>
                //    var parts = input.Split(' ');

                //    if (col.Calculation.Contains("IF("))
                //    {
                //        col.Value = EvaluateIf(col.Calculation.Remove(0, 1), col, record);
                //    }
                //    else if (col.Calculation.Contains("=ROWSUM"))
                //    {
                //        col.Value = RowSum(col.Calculation);
                //    }
                //    else if (col.Calculation.Contains("=SUM"))
                //    {
                //        //This is handled by the viewmodel as it references other grids.
                //        continue;
                //    }
                //    else
                //    {
                //        input = ConvertCodesToValues(input, col, record);

                //        ExpressionParser parser = new ExpressionParser();
                //        var calcValue = parser.Parse(input);

                //        col.Value = col.FormatValue(calcValue.ToString());
                //    }
                //}


            }
            //NotifyPropertyChanged(this, vm => vm.Records);
            HasChanged = true;
            return record;
        }

        public void HorizontalCalculation(Property col, Record record)
        {
            if (col.Calculation != null)
            {

                var invalid = new[] { "+", "-", "*", "/", ".", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "<", ">" };

                var input = col.Calculation.Replace("=", ""); //  <Value>=Num_Customers + Num_Products + Num_Promotions + Num_Terms</Value>
                var parts = input.Split(' ');

                if (col.Calculation.Contains("IF("))
                {
                    col.Value = EvaluateIf(col.Calculation.Remove(0, 1), col, record);
                }
                else if (col.Calculation.Contains("=ROWSUM"))
                {
                    col.Value = RowSum(col.Calculation);
                }
                else if (col.Calculation.Contains("=SUM"))
                {
                    //This is handled by the viewmodel as it references other grids.
                }
                else
                {
                    input = ConvertCodesToValues(input, col, record);

                    ExpressionParser parser = new ExpressionParser();
                    var calcValue = parser.Parse(input);

                    col.Value = col.FormatValue(calcValue.ToString());
                }
            }
        }

        private string RowSum(string calculation)
        {
            var rowSumElements = RowSumElement.ConvertToRowSumElements(calculation);

            string rowSumEquation = "";

            foreach (var rowSum in rowSumElements)
            {
                foreach (var rowIdx in rowSum.RowRange)
                {
                    var thisRow = Records.FirstOrDefault(row => row.Item_Idx.Equals(rowIdx));
                    var thisCell = thisRow.Properties.FirstOrDefault(col => col.ColumnCode.Trim().Equals(rowSum.ColumnCode));

                    decimal cellValue;
                    decimal.TryParse(thisCell.Value, NumberStyles.Currency, CultureInfo.CurrentCulture.NumberFormat, out cellValue);

                    rowSum.Result += cellValue;
                }

                rowSumEquation += rowSum.Result.ToString() + rowSum.Operator;
            }

            return RowSumElement.Evaluate(rowSumEquation).ToString(CultureInfo.InvariantCulture);
        }

        public void CalulateRecordColumnTotal()
        {
            if (Records == null)
                return;

            CalulateRecordColumnTotal(Records.FirstOrDefault());
        }

        private Dictionary<string, string> GetPropertiesValuesFromRecords()
        {
            var groupedColumns = Records.SelectMany(x => x.Properties).GroupBy(x => x.ColumnCode);
            Dictionary<string, string> propertiesValues = new Dictionary<string, string>();

            #region calculating properties values

            foreach (var groupedProperty in groupedColumns)
            {
                var columnValues = new List<Double>();
                foreach (var propertyFromSingleRecord in groupedProperty)
                {
                    double cellValue;
                    if (propertyFromSingleRecord.Value == null) continue;
                    double.TryParse(propertyFromSingleRecord.Value.Replace("%", ""), NumberStyles.Currency, CultureInfo.CurrentCulture.NumberFormat, out cellValue);

                    columnValues.Add(cellValue);
                }

                var firstPropertyFromGroup = groupedProperty.FirstOrDefault();

                var propertyValue = string.Empty;
                if (firstPropertyFromGroup.TotalsAggregationMethod != null)
                    switch (firstPropertyFromGroup.TotalsAggregationMethod.ToLower())
                    {
                        case "sum":
                            propertyValue = firstPropertyFromGroup.FormatValue(columnValues.Sum().ToString());
                            break;
                        case "avg":
                            propertyValue = firstPropertyFromGroup.FormatValue(columnValues.Average().ToString());
                            break;
                        case "count":
                            propertyValue = "# " + Records.Count();
                            break;
                    }

                if (!propertiesValues.ContainsKey(groupedProperty.Key))
                    propertiesValues.Add(groupedProperty.Key, propertyValue);
            }

            #endregion

            return propertiesValues;
        }

        /// <summary>
        /// Assigns to Results new collection containing one item - copy of record - and notifies Results change
        /// </summary>
        /// <param name="record">Record to be copied</param>
        /// /// <param name="propertiesValues">calculated values of the properties of all records - provided by GetPropertiesValuesFromRecords() function</param>
        /// <returns></returns>
        public void CalulateRecordColumnTotal(Record record)
        {
            var propertiesValues = GetPropertiesValuesFromRecords();

            if (propertiesValues.Values.All(String.IsNullOrWhiteSpace)) return;

            #region creating copy of record

            var copyOfRecord = new Record { Properties = new ObservableCollection<Property>() };

            foreach (var recordProperty in record.Properties)
            {
                var copyOfProperty = new Property
                {
                    IsDisplayed = recordProperty.IsDisplayed,
                    Value = (recordProperty.TotalsAggregationMethod != null) ? propertiesValues.SingleOrDefault(x => x.Key == recordProperty.ColumnCode).Value : null
                };

                copyOfRecord.Properties.Add(copyOfProperty);
            }

            #endregion

            if (
                copyOfRecord.Properties.Where(prop => prop.IsDisplayed)
                    .All(prop => String.IsNullOrWhiteSpace(prop.Value))) return;

            Results = new ObservableCollection<Record> { copyOfRecord };
            NotifyPropertyChanged(this, vm => vm.Results);
        }

        public static string FixNull(string In, string Default)
        {
            if (string.IsNullOrWhiteSpace(In))
            {
                In = Default;
            }

            return In;
        }

        #region "Dummy"



        //      public static string DummyData = @"
        //<Results>
        //  <RootItem> 
        //        <Item_Type>Scenario</Item_Type>
        //        <Item_Idx>1</Item_Idx>
        //        <Item_RowSortOrder>1</Item_RowSortOrder>
        //     <Attributes>
        //      <Attribute>
        //        <ColumnCode>Scen_Name</ColumnCode>
        //        <HeaderText>Name</HeaderText>
        //        <Value>(LIVE) - Live</Value>
        //        <Format />
        //        <ForeColour>#0000FF</ForeColour>
        //        <BorderColour>#FFFFFF</BorderColour>
        //        <IsDisplayed>1</IsDisplayed>
        //        <IsEditable>0</IsEditable>
        //        <IsHyperlink>1</IsHyperlink>
        //        <TotalsAggregationMethod>COUNT</TotalsAggregationMethod>
        //      </Attribute>
        //      <Attribute>
        //        <ColumnCode>Author</ColumnCode>
        //        <HeaderText>Author</HeaderText>
        //        <Value>Test User, Exceedra</Value>
        //        <Format />
        //        <ForeColour>#006633</ForeColour>
        //        <BorderColour>#FFFFFF</BorderColour>
        //        <IsDisplayed>1</IsDisplayed>
        //        <IsEditable>0</IsEditable>
        //        <IsHyperlink>0</IsHyperlink>
        //        <TotalsAggregationMethod>COUNT</TotalsAggregationMethod>
        //      </Attribute> 
        //    </Attributes>
        //  </RootItem>
        //  <RootItem>
        //    <Item_Type>Scenario2</Item_Type>
        //    <Item_Idx>3</Item_Idx>
        //    <RowSortOrder>3</RowSortOrder> 
        //    <Attributes>
        //      <Attribute>
        //        <ColumnCode>Scen_Name</ColumnCode>
        //        <HeaderText>Name</HeaderText>
        //        <Value>(PVB-8) - Budget 2014</Value>
        //        <Format />
        //        <ForeColour>#0000FF</ForeColour>
        //        <BorderColour>#FFFFFF</BorderColour>
        //        <IsDisplayed>1</IsDisplayed>
        //        <IsEditable>0</IsEditable>
        //        <IsHyperlink>1</IsHyperlink>
        //        <TotalsAggregationMethod>COUNT</TotalsAggregationMethod>
        //      </Attribute>
        //      <Attribute>
        //        <ColumnCode>Author</ColumnCode>
        //        <HeaderText>Author</HeaderText>
        //        <Value>Van Bosch, Peter</Value>
        //        <Format />
        //        <ForeColour>#006633</ForeColour>
        //        <BorderColour>#FFFFFF</BorderColour>
        //        <IsDisplayed>1</IsDisplayed>
        //        <IsEditable>0</IsEditable>
        //        <IsHyperlink>0</IsHyperlink>
        //        <TotalsAggregationMethod>COUNT</TotalsAggregationMethod>
        //      </Attribute> 
        //    </Attributes>
        //  </RootItem>
        //</Results>
        //";

        public static string DummyData = @"
    <Results><RootItem><Item_Idx>1</Item_Idx><Item_Type>Scenario</Item_Type><Item_RowSortOrder>1</Item_RowSortOrder><Attributes><Attribute><ColumnCode>IsSelected</ColumnCode><HeaderText>Selection</HeaderText><Value>false</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Checkbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Scen_Name</ColumnCode><HeaderText>Name</HeaderText><Value>(EJ - 01) - Test Row 1</Value><Format /><ForeColour>#0000FF</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Hyperlink</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>COUNT</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Author</ColumnCode><HeaderText>Author</HeaderText><Value>Ed</Value><Format /><ForeColour>#006633</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>COUNT</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Start_Day</ColumnCode><HeaderText>Start_Day</HeaderText><Value>2014-01-01</Value><Format>LongDate</Format><ForeColour>#660000</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>End_Day</ColumnCode><HeaderText>End_Day</HeaderText><Value>2014-06-30</Value><Format>ShortDate</Format><ForeColour>#66FFFF</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>MIN</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Create_Day</ColumnCode><HeaderText>Create_Day</HeaderText><Value>2013-12-31</Value><Format>dd-mmm-yyyy</Format><ForeColour>#990000</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>MAX</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Status_Name</ColumnCode><HeaderText>Status</HeaderText><Value>Draft</Value><Format /><ForeColour>#DF0101</ForeColour><BorderColour>#2E2EFE</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Customers</ColumnCode><HeaderText>Number Of Customers</HeaderText><Value>100</Value><Format>N0</Format><ForeColour>#996600</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>SUM</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Products</ColumnCode><HeaderText>Number Of Products</HeaderText><Value>200</Value><Format>N2</Format><ForeColour>#99CC00</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>SUM</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Promotions</ColumnCode><HeaderText>Number Of Promotions</HeaderText><Value>300</Value><Format>N4</Format><ForeColour>#99CC00</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>AVG</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Terms</ColumnCode><HeaderText>Number Of Terms</HeaderText><Value>400</Value><Format>N8</Format><ForeColour>#9999CC</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>MIN</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>CalcTotalItems</ColumnCode><HeaderText>Total Linked Items</HeaderText><Value>=Num_Customers + Num_Products + Num_Promotions + Num_Terms</Value><Format>N8</Format><ForeColour>#DF0101</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>MAX</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>CalcTwiceTotalItems</ColumnCode><HeaderText>Twice Linked Items As Currency</HeaderText><Value>=CalcTotalItems * 2</Value><Format>C8</Format><ForeColour>#DF0101</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>MAX</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>OtherCheckbox</ColumnCode><HeaderText>Other Selection</HeaderText><Value>false</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Checkbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Dropdown</ColumnCode><HeaderText>Master Dropdown</HeaderText><Value /><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Dropdown</ControlType><DataSource>dbo.GetDropDownData1</DataSource><DependentColumn>ChildDropdown</DependentColumn><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>ChildDropdown</ColumnCode><HeaderText>Child Dropdown</HeaderText><Value /><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>MultiDropdown</ControlType><DataSource>dbo.GetDropDownData2</DataSource><DependentColumn>ChildDropdown2</DependentColumn><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>ChildDropdown2</ColumnCode><HeaderText>Second Child Dropdown</HeaderText><Value /><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>MultiDropdown</ControlType><DataSource>dbo.GetDropDownData3</DataSource><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute></Attributes></RootItem><RootItem><Item_Idx>2</Item_Idx><Item_Type>Scenario</Item_Type><Item_RowSortOrder>2</Item_RowSortOrder><Attributes><Attribute><ColumnCode>IsSelected</ColumnCode><HeaderText>Selection</HeaderText><Value>false</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Checkbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Scen_Name</ColumnCode><HeaderText>Name</HeaderText><Value>(EJ - 02) - Test Row 2</Value><Format /><ForeColour>#0000FF</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Hyperlink</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>COUNT</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Author</ColumnCode><HeaderText>Author</HeaderText><Value>Ed</Value><Format /><ForeColour>#006633</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>COUNT</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Start_Day</ColumnCode><HeaderText>Start_Day</HeaderText><Value>2015-01-01</Value><Format>LongDate</Format><ForeColour>#660000</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>End_Day</ColumnCode><HeaderText>End_Day</HeaderText><Value>2015-06-30</Value><Format>ShortDate</Format><ForeColour>#66FFFF</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>MIN</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Create_Day</ColumnCode><HeaderText>Create_Day</HeaderText><Value>2014-12-31</Value><Format>dd-mmm-yyyy</Format><ForeColour>#990000</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>MAX</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Status_Name</ColumnCode><HeaderText>Status</HeaderText><Value>Draft</Value><Format /><ForeColour>#DF0101</ForeColour><BorderColour>#2E2EFE</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Customers</ColumnCode><HeaderText>Number Of Customers</HeaderText><Value>1000</Value><Format>N0</Format><ForeColour>#996600</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>SUM</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Products</ColumnCode><HeaderText>Number Of Products</HeaderText><Value>2000</Value><Format>N2</Format><ForeColour>#99CC00</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>SUM</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Promotions</ColumnCode><HeaderText>Number Of Promotions</HeaderText><Value>3000</Value><Format>N4</Format><ForeColour>#99CC00</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>AVG</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Terms</ColumnCode><HeaderText>Number Of Terms</HeaderText><Value>4000</Value><Format>N8</Format><ForeColour>#9999CC</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>MIN</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>CalcTotalItems</ColumnCode><HeaderText>Total Linked Items</HeaderText><Value>=Num_Customers + Num_Products + Num_Promotions + Num_Terms</Value><Format>N8</Format><ForeColour>#DF0101</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>MAX</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>CalcTwiceTotalItems</ColumnCode><HeaderText>Twice Linked Items As Currency</HeaderText><Value>=CalcTotalItems * 2</Value><Format>C8</Format><ForeColour>#DF0101</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>MAX</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>OtherCheckbox</ColumnCode><HeaderText>Other Selection</HeaderText><Value>false</Value><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Checkbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Dropdown</ColumnCode><HeaderText>Master Dropdown</HeaderText><Value /><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Dropdown</ControlType><DataSource>dbo.GetDropDownData1</DataSource><DependentColumn>ChildDropdown</DependentColumn><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>ChildDropdown</ColumnCode><HeaderText>Child Dropdown</HeaderText><Value /><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>MultiDropdown</ControlType><DataSource>dbo.GetDropDownData2</DataSource><DependentColumn>ChildDropdown2</DependentColumn><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>ChildDropdown2</ColumnCode><HeaderText>Second Child Dropdown</HeaderText><Value /><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>MultiDropdown</ControlType><DataSource>dbo.GetDropDownData3</DataSource><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute></Attributes></RootItem></Results>";

        #endregion

        public void Save()
        {

            var xdoc = ToXml();
            var xml = XElement.Parse(xdoc.ToString());
            var res = Model.DataAccess.@WebServiceProxy.Call("[dbo].[SetData]", xml);

        }

        public XDocument ToXml(XElement additionalData = null)
        {
            //fuck me this is going to be fun....
            //  /Results/RootItem/Attributes/Attribute/

            if (Records != null)
            {
                XDocument xdoc = new XDocument(new XElement("Results",
                    from r in Records
                    select new XElement("RootItem",
                        new XElement("Item_Type", r.Item_Type),
                        new XElement("Item_Idx", r.Item_Idx),
                        new XElement("Attributes",
                            from p in r.Properties
                            select new XElement("Attribute",
                                new XElement("ColumnCode", p.ColumnCode),
                                ValueElement(p, r)
                                )
                            )
                        )
                    )
                    );

                if (additionalData != null)
                    xdoc.Root.Add(additionalData);

                return xdoc;
            }

            return new XDocument();
        }

        public XDocument ToXml(bool excludeRecordsMarkedToDelete, bool excludeRecordsWithoutChanges, XElement additionalData = null)
        {
            //fuck me this is going to be fun....
            //  /Results/RootItem/Attributes/Attribute/

            var allowedRecords = new List<Record>(Records);


            if (excludeRecordsMarkedToDelete)
                allowedRecords = allowedRecords.SkipWhile(r =>

                            // skip if marked as "to delete"
                            r.HasChanges == 2

                    ).ToList();


            if (excludeRecordsWithoutChanges)
                allowedRecords = allowedRecords.SkipWhile(r =>

                            // take every term that is already saved in the db (not newly created)
                            int.Parse(r.Item_Idx) >= 0

                            &&

                            (

                                // and has no changes
                                r.HasChanges == 0

                                // or it's inner grid has no changes
                                &&

                                (r.DetailsViewModel == null || !((RecordViewModel)r.DetailsViewModel).HasChanged)

                            )

                    ).ToList();


            XDocument xdoc = new XDocument(new XElement("Results",
                from r in allowedRecords
                select new XElement("RootItem",
                    new XElement("Item_Type", r.Item_Type),
                    new XElement("Item_Idx", r.Item_Idx),
                    new XElement("Attributes",
                        from p in r.Properties
                        select new XElement("Attribute",
                            new XElement("ColumnCode", p.ColumnCode),
                            ValueElement(p, r)
                            )
                         )
                       )
                     )
                 );

            if (additionalData != null)
                xdoc.Root.Add(additionalData);

            return xdoc;
        }

        #region Serialize

        /* Use this to save the VM at a given state so you can compare to a future state. */
        public XElement Serialization { get; set; }

        public void SerializeState()
        {
            if(Records!=null)
                Serialization = ToXml().Root;
        }

        #endregion

        private XElement ValueElement(Property input, Record inputRecord)
        {
            // handling special control types

            if (input.ControlType.ToLower() == "pipeline")
            {
                if (!string.IsNullOrEmpty(input.Value))
                    return new XElement("Value", input.Value);

                if (input.Values != null && input.Values.Any())
                {
                    XElement xValues = new XElement("Values");
                    foreach (var value in input.Values)
                        xValues.Add(new XElement("Value", value.Item_Idx));
                    return xValues;
                }

            }
            else if (input.ControlType.ToLower() == "horizontalgrid")
            {
                var details = inputRecord.DetailsViewModel as RecordViewModel;

                // case when the inner grid is not expanded 
                if (details == null) return new XElement("Value");

                return new XElement("Value", details.ToXml().Root);
            }
            else if (input.ControlType.ToLower() == "dropdown")
            {
                XElement xValues = new XElement("Values");
                if (input.SelectedItem != null) xValues.Add(new XElement("Value", input.SelectedItem.Item_Idx));
                else if (input.SelectedItems != null && input.SelectedItems.Any())
                    foreach (var selectedItem in input.SelectedItems)
                        xValues.Add(new XElement("Value", selectedItem.Item_Idx));

                return xValues;
            }
            else if (input.ControlType.ToLower() == "datepicker")
            {
                var toDbFormat = DateTimeHelper.ToDatabaseFormat((input.Value));

                return new XElement("Value", toDbFormat);
            }

            // handling casual control types

            var returnValue = input.Value;

            // processing the StringFormat
            if (input.StringFormat.ToLower().Contains("c") && input.Value != null && input.Value != "")
            {
                returnValue = Double.Parse(input.Value, NumberStyles.Currency).ToString();
                returnValue = FixValue(returnValue);
            }
            else if (input.StringFormat.ToLower().Contains("p") || input.Value.Contains("%"))
            {
                returnValue = input.Value.Replace(CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                returnValue = FixValue(returnValue);

                if (input.StringFormat.ToLower().Contains("p") || input.StringFormat.ToLower().Contains(""))
                    returnValue = returnValue + "%";
            }
            else
            {
                returnValue = FixValue(returnValue);
            }

            return new XElement("Value", returnValue);
        }

        public static string FixValue(string val)
        {
            var isNum = false;
            decimal d;

            val = FixNull(val, "");
            //rip out the localised %
            val = val.Replace(CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "")
                .Replace(CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "");

            if (CultureInfo.CurrentCulture.IetfLanguageTag != ("en-GB"))
            {
                isNum = decimal.TryParse(val, NumberStyles.Number, CultureInfo.CurrentCulture, out d);
            }
            else
            {
                isNum = decimal.TryParse(val, NumberStyles.Any, CultureInfo.GetCultureInfoByIetfLanguageTag("en-GB"), out d);
            }

            if (isNum == true)
            {
                var x = d.ToString(CultureInfo.GetCultureInfoByIetfLanguageTag("en-GB"));
                return x;
            }
            else
            {
                return val;
            }

        }



        public XDocument ToWebServiceXml()
        {
            XDocument xdoc = new XDocument(new XElement("Results",
                from r in Records
                select new XElement("RootItem",
                    new XElement("Item_Type", r.Item_Type),
                    new XElement("Item_Idx", r.Item_Idx),
                    new XElement("Item_RowSortOrder", r.Item_RowSortOrder),
                    new XElement("Item_IsDisplayed", r.Item_IsDisplayed ? "1" : "0"),
                    new XElement("Attributes",
                        from p in r.Properties
                        select new XElement("Attribute",
                            new XElement("ColumnCode", p.ColumnCode),
                            new XElement("Alignment", p.Alignment),
                            new XElement("BackgroundColour", p.BackgroundColour),
                            new XElement("BorderColour", p.BorderColour),
                            new XElement("HeaderText", p.HeaderText),
                            new XElement("IsDisplayed", p.IsDisplayed ? "1" : "0"),
                            new XElement("ControlType", p.ControlType),
                            new XElement("IsEditable", p.IsEditable ? "1" : "0"),
                            new XElement("UpdateToCell", p.UpdateToCell),
                            new XElement("Value", p.Value),
                            new XElement("Value2", p.Value2)
                            )
                         )
                       )
                     )
                 );

            return xdoc;
        }

        public HeaderRow ExecuteOperations(HeaderRow obj)
        {
            CalulateRecordColumns();
            CalulateRecordColumnTotal(Records.FirstOrDefault());
            //MessageBox.Show(obj.Label);
            return obj;
        }

        internal void UpdateColumn(string amount, string tag)
        {
            var options = tag.Split(',');

            // {0} = Type
            // {1} = ParentColumnCode
            // {2} = ProRatingColumnCode
            // {3} = calculation(s ; )
            var invalid = new string[] { "+", "-", "*", "/", ".", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "<", ">" };

            if (options[0].ToLower() != "calculation")
            {
                DefaultOperations(amount, tag);
            }
            else
            {
                var calcs = options[3].Split(';');
                foreach (var calc in calcs)
                {
                    //format eg: PromoVol_SO = BaseVol_SO * (1 + INPUT / 100)

                    // for each row in grid
                    foreach (var record in Records)
                    {
                        //find column to left of '='
                        var parentColumnCode = calc.Substring(0, calc.IndexOf('=')).Trim();
                        var col = record.Properties.SingleOrDefault(c => c.ColumnCode == parentColumnCode);

                        //swap out column names to right of '=' with values
                        var equation = calc.Right(calc.Length - calc.IndexOf('=')).Trim();

                        //right this is one that has a calc value
                        var input = equation.Replace("=", "").Replace("INPUT", amount).Replace("–", "-"); //  <Value>=Num_Customers + Num_Products + Num_Promotions + Num_Terms</Value>
                        var parts = input.Split(' ');

                        var prop = 0;

                        foreach (var portion in parts)
                        {
                            if (!invalid.Contains(portion) && !string.IsNullOrEmpty(portion))
                            {

                                //make a copy of the string, remove the () 
                                var copy = portion;
                                //find the column code
                                var colCode = copy.Replace("(", "").Replace(")", "");

                                //swap column code for value if its not a number
                                int n;
                                bool isNumeric = int.TryParse(colCode, out n);

                                var val = "";

                                if (!isNumeric && !colCode.Contains("%"))
                                {

                                    val = record.Properties.Where(t => t.ColumnCode == colCode).Select(t => t.Value).SingleOrDefault();
                                }
                                else
                                {
                                    val = colCode;
                                }

                                bool isPerc = val.Contains("%");
                                decimal d;

                                val = val.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                                decimal.TryParse(val, NumberStyles.Any, CultureInfo.CurrentCulture, out d);

                                if (isPerc)
                                {
                                    d = (d / 100);
                                }

                                input = input.Replace(colCode, d.ToString(CultureInfo.GetCultureInfoByIetfLanguageTag("en-GB")));

                            }
                            prop += 1;
                        }


                        ExpressionParser parser = new ExpressionParser();
                        var calcValue = parser.Parse(input);

                        col.Value = col.FormatValue(calcValue.ToString());
                        col.BackgroundColour = "#ffcc00";

                    }
                }
            }
        }

        internal void FilterByColumn(string columnCode, string value)
        {
            CalculationFilters = new List<FilterEntry>();

            CalculationFilters.Add(
                new FilterEntry
                {
                    ColumnCode = columnCode,
                    Value = value
                }
                );
        }

        private void DefaultOperations(string amount, string tag)
        {
            var raw = tag.Split(',');

            //  column.Type + "," + column.ParentColumnCode + "," + column.ProRatingColumnCode
            var method = raw[0];
            var ParentColumnCode = raw[1];
            var ProRatingColumnCode = raw[2];

            var sum = 0d;
            foreach (var r in Records)
            {
                try
                {
                    int d;
                    var value = r.Properties.Where(c => c.ColumnCode == ProRatingColumnCode).SingleOrDefault().Value;
                    int.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out d);
                    sum += d;
                }
                catch (Exception ex)
                {

                }
            }


            foreach (var r in Records)
            {
                bool isRecordAllowed = true;

                if (CalculationFilters != null && CalculationFilters.Any())
                {
                    // is record conform with all the calculation filters?
                    foreach (var calculationFilter in CalculationFilters)
                    {
                        var propertyToFilter = r.Properties.FirstOrDefault(p => p.ColumnCode == calculationFilter.ColumnCode);
                        if (propertyToFilter.SelectedItem.Item_Name != calculationFilter.Value)
                            isRecordAllowed = false;
                    }
                }

                if (!isRecordAllowed) continue;

                //find the column to adjust
                var column = r.Properties.Where(c => c.ColumnCode == ParentColumnCode).SingleOrDefault();

                if (method.ToLower() == "constant")
                {
                    column.Value = amount;
                }
                else if (method.ToLower() == "prorate")
                {
                    int proRateColValue;
                    var value = r.Properties.Where(c => c.ColumnCode == ProRatingColumnCode).SingleOrDefault().Value;
                    int.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out proRateColValue);

                    //value =       (amount / sum(ProrateCol)) * CurrentRow(proRateColValue)
                    if (sum > 0)
                        column.Value = Math.Abs((Convert.ToDouble(amount) / sum) * proRateColValue).ToString();
                }
                else
                {
                }
            }
        }

        private bool _isDataLoading;

        public bool IsDataLoading
        {
            get { return _isDataLoading; }
            set
            {
                _isDataLoading = value;

                NotifyPropertyChanged(this, vm => vm.IsDataLoading);
            }
        }

        //private string _panelMainMessage;
        ///// <summary>
        ///// Gets or sets the panel main message.
        ///// </summary>
        ///// <value>The panel main message.</value>
        //public string PanelMainMessage
        //{
        //    get
        //    {
        //        return _panelMainMessage;
        //    }
        //    set
        //    {
        //        _panelMainMessage = value;
        //        NotifyPropertyChanged(this, vm => vm.PanelMainMessage);
        //    }
        //}

        //private string _panelSubMessage;
        ///// <summary>
        ///// Gets or sets the panel sub message.
        ///// </summary>
        ///// <value>The panel sub message.</value>
        //public string PanelSubMessage
        //{
        //    get
        //    {
        //        return _panelSubMessage;
        //    }
        //    set
        //    {
        //        _panelSubMessage = value;
        //        NotifyPropertyChanged(this, vm => vm.PanelSubMessage);
        //    }
        //}


        //private bool _isRunning;
        //public bool IsRunning
        //{
        //    get
        //    {
        //        return _isRunning;
        //    }
        //    set
        //    {
        //        _isRunning = value;
        //        NotifyPropertyChanged(this, vm => vm.IsRunning);
        //    }
        //}

        ///// <summary>
        ///// Gets the panel close command.
        ///// </summary>
        //public ICommand PanelCloseCommand
        //{
        //    get
        //    {
        //        return new DelegateCommand(() =>
        //        {
        //            // Your code here.
        //            // You may want to terminate the running thread etc.
        //            IsDataLoading = false;
        //        });
        //    }
        //}

        ///// <summary>
        ///// Gets the show panel command.
        ///// </summary>
        //public ICommand ShowPanelCommand
        //{
        //    get
        //    {
        //        return new DelegateCommand(() =>
        //        {
        //            IsDataLoading = true;
        //        });
        //    }
        //}

        ///// <summary>
        ///// Gets the hide panel command.
        ///// </summary>
        //public ICommand HidePanelCommand
        //{
        //    get
        //    {
        //        return new DelegateCommand(() =>
        //        {
        //            IsDataLoading = false;
        //        });
        //    }
        //}

        ///// <summary>
        ///// Gets the change sub message command.
        ///// </summary>
        //public ICommand ChangeSubMessageCommand
        //{
        //    get
        //    {
        //        return new DelegateCommand(() =>
        //        {
        //            PanelSubMessage = string.Format("Message: {0}", DateTime.Now);
        //        });
        //    }
        //}


        public bool HasRecords
        {
            get { return (Records != null || Records.Count > 0); }
        }

        public bool HasNonEmptyRecords
        {
            get { return HasRecords && Records.Any(rec => rec.Item_Idx != "0"); }
        }

        public void Refresh()
        {
            NotifyPropertyChanged(this, vm => vm.Records);
        }


        private string _filter = "Filter...";

        public string Filter
        {
            get { return _filter; }
            set
            {
                if (value != _filter)
                {
                    _filter = value;
                    NotifyPropertyChanged(this, vm => vm.Filter);
                }
            }
        }


        public override bool AreRecordsFulfilled()
        {
            return Records.All(record => record.ArePropertiesFulfilled());
        }

        public void UpdateHeaders()
        {
            NotifyPropertyChanged(this, vm => vm.Headers);
        }

        #region IF Calcualtion Evaluation ACROSS A ROW

        private string TrimEquationType(string calculation, string prefixToRemove)
        {
            var calcLength = calculation.Length;
            var prefixLength = prefixToRemove.Length;
            return calculation.Substring(prefixLength, calcLength - 1 - prefixLength);
        }

        private bool EvaluateCondtional(string conditional, Record record)
        {
            var conditionalComponents = conditional.Split('=');
            var statement = conditionalComponents[0].Trim();
            var value = conditionalComponents[1].Replace('"', ' ').Replace('/', ' ').Trim();
            string statementValue;

            //Do a check to see if the statement is an equation...
            if (statement.ToLower().Contains("rowsum"))
            {
                statementValue = RowSum(statement);
            }
            else if (statement.ToLower().Contains("countif"))
            {
                statementValue = CountIf(statement, record);
            }
            else //If not an equation, then a column:
            {
                statementValue = GetProperty(statement, record).Value;
            }

            var areNumericallyEqual = NumericComparison(statementValue.ToLower(), value.ToLower());

            return areNumericallyEqual || statementValue.ToLower() == value.ToLower();
        }

        private bool NumericComparison(string string1, string string2)
        {
            decimal num1;
            var isNum = Decimal.TryParse(string1.Replace("%", ""), NumberStyles.Any, CultureInfo.CurrentCulture, out num1);
            if (isNum)
            {
                decimal num2;
                isNum = Decimal.TryParse(string2, NumberStyles.Any, CultureInfo.CurrentCulture, out num2);
                if (isNum)
                    return num1.Equals(num2);
            }
            return false;
        }

        private string CountIf(string calculation, Record record)
        {
            var equation = TrimEquationType(calculation, "CountIf(");
            var components = equation.Split(',');
            var columnCode = components[0].Trim();
            var symbolToFind = components[1].Replace('"', ' ').Replace('/', ' ').Trim();

            var propertyValue = GetProperty(columnCode, record).Value;
            var occurences = Regex.Matches(propertyValue, symbolToFind).Count;

            return occurences.ToString();
        }

        private Property GetProperty(string code, Record record)
        {
            return record.Properties.FirstOrDefault(p => p.ColumnCode.Equals(code));
        }

        private string ConvertCodesToValues(string calculation, Property col, Record record)
        {
            var invalid = new[] { "+", "-", "*", "/", ".", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "<", ">" };

            var formattedParts = SplitIntoFormattedParts(calculation);

            foreach (var portion in formattedParts.Distinct())
            {
                decimal testDecimal;
                if (!invalid.Contains(portion) && !decimal.TryParse(portion, out testDecimal))
                {
                    SharedMethods.ReplacePortionWithValue(ref calculation, portion, col.ColumnCode, record.Properties.ToList());
                }
            }

            return calculation;
        }

        /* Here the calculation is expected to be using Item_Codes (aka Records) and we are working down a specific colum */
        private string ConvertCodesToValues(string calculation, string columnCode)
        {
            var invalid = new[] { "+", "-", "*", "/", ".", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "<", ">" };

            var formattedParts = SplitIntoFormattedParts(calculation);

            foreach (var portion in formattedParts.Distinct())
            {
                decimal testDecimal;
                if (!invalid.Contains(portion) && !decimal.TryParse(portion, out testDecimal))
                {
                    calculation = calculation.Replace(portion, GetRecordByCode(portion).GetProperty(columnCode).Value);
                }
            }

            return calculation;
        }

        private string[] SplitIntoFormattedParts(string calculation)
        {        
            calculation = calculation.Trim('=');

            var parts = Regex.Split(calculation, "([-+*/<>])");

            var formattedParts = new string[parts.Where(p => !string.IsNullOrEmpty(p)).Count()];

            /* Since we do a naive replace into the calculation string, just format and order by length so the replace only effect relevant parts. */
            for (int i = 0; i < parts.Length; i++)
            {
                formattedParts[i] = parts[i].Replace("(", "").Replace(")", "").Trim();
            }

            Array.Sort(formattedParts, (x, y) => y.Length.CompareTo(x.Length));

            return formattedParts;
        }

        private IEnumerable<string> SplitIntoRecordCodes(string calculation)
        {
            var operators = new[] { "+", "-", "*", "/", "<", ">" };

            var allParts = SplitIntoFormattedParts(calculation);
            var codeParts = allParts.Where(p => !operators.Contains(p) && !p.IsNumeric());

            return codeParts;
        }

        public string EvaluateIf(string calculation, Property col, Record record)
        {
            var calc = TrimEquationType(calculation, "IF(");

            //0. Conditional
            //1. then
            //2. else
            var calcComponents = StepThroughCalc(calc);

            //var calcComponents = calc.Split(",");

            //We only want 3 components, if we get more we will combine the first n-2
            var numOfComponents = calcComponents.Count();
            if (numOfComponents > 3)
            {
                for (int i = 1; i < numOfComponents - 2; i++)
                {
                    calcComponents[0] += ',' + calcComponents[i];
                }
                calcComponents[1] = calcComponents[numOfComponents - 2];
                calcComponents[2] = calcComponents[numOfComponents - 1];
            }

            var isTrue = EvaluateCondtional(calcComponents[0], record);

            string numericCalculation;
            if (isTrue) //evaulate the then clause
            {
                var thenClause = calcComponents[1].Trim();
                numericCalculation = thenClause.Contains("IF(") ? EvaluateIf(thenClause, col, record) : ConvertCodesToValues(thenClause, col, record);
            }
            else //evaulate the else clause
            {
                var elseClause = calcComponents[2].Trim();
                numericCalculation = elseClause.Contains("IF(") ? EvaluateIf(elseClause, col, record) : ConvertCodesToValues(elseClause, col, record);
            }

            ExpressionParser parser = new ExpressionParser();
            var evaluatedCaluation = numericCalculation.Contains('%') ? numericCalculation : parser.Parse(numericCalculation).ToString();
            return col.FormatValue(evaluatedCaluation);

        }

        private string[] StepThroughCalc(string calc)
        {
            var parenCounter = 0;
            var splitCounter = 0;
            var lastSplitPos = 0;
            string[] splitCalc = new string[3];

            for (int i = 0; i < calc.Length; i++)
            {
                if (calc[i] == '(')
                {
                    parenCounter++;
                }
                else if (calc[i] == ')')
                {
                    parenCounter--;
                }
                else if (calc[i] == ',')
                {
                    if (parenCounter == 0)
                    {
                        splitCalc[splitCounter] = calc.Substring(lastSplitPos, i - lastSplitPos);
                        lastSplitPos = i + 1;
                        splitCounter++;
                        if (splitCounter == 2)
                        {
                            splitCalc[splitCounter] = calc.Substring(lastSplitPos, calc.Length - lastSplitPos);
                            break;
                        }
                    }
                }
            }

            return splitCalc;
        }

        #endregion

        #region IF Calcualtion Evaluation DOWN A COLUMN

        public string EvaluateIf(string calculation, string columnCode)
        {
            var calc = TrimEquationType(calculation, "IF(");

            //0. Conditional
            //1. then
            //2. else
            var calcComponents = StepThroughCalc(calc);

            //var calcComponents = calc.Split(",");

            //We only want 3 components, if we get more we will combine the first n-2
            var numOfComponents = calcComponents.Count();
            if (numOfComponents > 3)
            {
                for (int i = 1; i < numOfComponents - 2; i++)
                {
                    calcComponents[0] += ',' + calcComponents[i];
                }
                calcComponents[1] = calcComponents[numOfComponents - 2];
                calcComponents[2] = calcComponents[numOfComponents - 1];
            }

            var isTrue = EvaluateCondtional(calcComponents[0], columnCode);

            string numericCalculation;
            if (isTrue) //evaulate the then clause
            {
                var thenClause = calcComponents[1].Trim();
                numericCalculation = thenClause.Contains("IF(") ? EvaluateIf(thenClause, columnCode) : ConvertCodesToValues(thenClause, columnCode);
            }
            else //evaulate the else clause
            {
                var elseClause = calcComponents[2].Trim();
                numericCalculation = elseClause.Contains("IF(") ? EvaluateIf(elseClause, columnCode) : ConvertCodesToValues(elseClause, columnCode);
            }

            ExpressionParser parser = new ExpressionParser();
            var evaluatedCaluation = numericCalculation.Contains('%') ? numericCalculation : parser.Parse(numericCalculation).ToString();
            return evaluatedCaluation;

        }

        private bool EvaluateCondtional(string conditional, string columnCode)
        {
            var conditionalComponents = conditional.Split('=');
            var statement = conditionalComponents[0].Trim();
            var value = conditionalComponents[1].Replace('"', ' ').Replace('/', ' ').Trim();
            string statementValue;

            statementValue = GetProperty(statement.Replace("#", " "), columnCode).Value;
            
            var areNumericallyEqual = NumericComparison(statementValue.ToLower(), value.ToLower());

            return areNumericallyEqual || statementValue.ToLower() == value.ToLower();
        }

        #endregion

        #region helper methods
        /* These are a bunch of methods to help reduce repeated code */


        public Property GetProperty(string rowIdx, string columnIdx)
        {
            var rec = GetRecord(rowIdx);
            return rec == null ? null : rec.GetProperty(columnIdx);
        }

        /* Tries to get the first property with a numeric value, checking each record in the dissagregationIdxs list */
        internal Property GetProperty(List<string> dissagregationIdxs, string columnCode)
        {
            Property property = null;
            foreach(var idx in dissagregationIdxs)
            {
                property = GetProperty(idx, columnCode);

                if (property == null) continue;

                if (property.Value.IsNumeric())
                {
                    var isOverride = GetRecord(idx).Item_Name.ToLower().Contains("override");
                    if (isOverride && property.Value.AsNumericInt() == -1)
                        return new Property { Value = "0" };
                    else if (isOverride && property.Value.AsNumericInt() == 0) continue;
                    return property;
                }
            }
            return null;
        }

        public Record GetRecord(string rowIdx)
        {
            return Records.FirstOrDefault(r => r.Item_Idx == rowIdx);
        }

        public Record GetRecordByType(string type)
        {
            return Records.FirstOrDefault(r => r.Item_Type == type);
        }

        public Record GetRecordByName(string name)
        {
            return Records.FirstOrDefault(r => r.Item_Name == name);
        }

        public Record GetRecordByCode(string name)
        {
            return Records.FirstOrDefault(r => r.Item_Code == name);
        }

        public bool IsRecordOverride(string rowIdx)
        {
            var rec = Records.FirstOrDefault(r => r.Item_Idx == rowIdx);
            if (rec == null) return false;

            return rec.Item_Name.ToLower().Contains("override");
        }

        #endregion


    }
}
