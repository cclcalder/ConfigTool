using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Controls.DynamicRow.Models;
using Exceedra.Controls.Helpers;
using Exceedra.DynamicGrid.Models;
using Exceedra.DynamicGrid.Models.Validation;
using info.lundin.math;
using Model;
using Model.DataAccess;
using MessageBox = System.Windows.MessageBox;

namespace Exceedra.Controls.DynamicRow.ViewModels
{
    public class RowViewModel : RecordContainer
    {
        private bool _isLoading;

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged(this, vm => vm.IsLoading);
            }
        }

        public RowViewModel()
        {
            IsLoading = false;
            Records = new ObservableCollection<RowRecord>();            
        }

        public RowViewModel(XElement res)
        {
            IsLoading = true;
            Records = new ObservableCollection<RowRecord>(LoadRecordRows(res));
            IsLoading = false;
        }

        public static RowViewModel LoadWithData(XElement res)
        {
            var instance = new RowViewModel();
            instance.Init(res);
            return instance;
        }

        public void Init(XElement res)
        {
            IsLoading = true;
            Records = new ObservableCollection<RowRecord>(LoadRecordRows(res));
        }

        private ObservableCollection<RowRecord> _records;

        public ObservableCollection<RowRecord> Records
        {
            get { return _records; }
            set
            {
                IsLoading = false;
                if (_records != null && _records != value)
                {
                    HasChanged = true;
                }

                if (_records != value)
                {
                    _records = value;
                    NotifyPropertyChanged(this, vm => vm.Records);
                }
            }
        }

        public int LongestLabel { get; set; }
        private readonly Font _defaultFont = new Font("Segoe UI", 12, FontStyle.Bold);

        public List<RowRecord> LoadRecordRows(XElement res)
        {
            var ri = (from r in res.Descendants("RootItem")
                      select r).ToList();

            var loadedRecords = new List<RowRecord>();

            foreach (var rec in ri)
            {
                var nr = new RowRecord()
                {
                    Item_Idx = rec.Element("Item_Idx").MaybeValue(),
                    Item_Type = rec.Element("Item_Type").Value,
                    Item_RowSortOrder = int.Parse(rec.Element("Item_RowSortOrder").MaybeValue() ?? "0"),
                    HeaderText = rec.Element("HeaderText").MaybeValue(),
                    Width = xml.FixIntNullable(rec.Element("Width"))
                };

                if (nr.Item_Idx == null)
                    nr.Item_Idx = rec.Element("Idx").MaybeValue();

                var attr = (from r in rec.Descendants("Attributes") select r).ToList();

                var props = new List<RowProperty>();

                var current = 0;

                foreach (var att in attr.Elements("Attribute"))
                {
                    try
                    {

                        var p = new RowProperty
                        {
                            ColumnCode = att.Element("ColumnCode").Value,
                            HeaderText = att.Element("HeaderText").Value,
                            IsDisplayed = xml.FixBool(att.Element("IsDisplayed"), false), // (att.Element("IsDisplayed").Value == "1" ? true : false),
                            IsEditable = xml.FixBool(att.Element("IsEditable"), false),
                            IsRequired = xml.FixBool(att.Element("IsRequired"), false),
                            StringFormat = xml.FixNullInline(att.Element("Format"), ""),
                            ForeColour = xml.FixNullInline(att.Element("ForeColour"), "#000000"),
                            Value = xml.FixNullInline(att.Element("Value"), ""),
                            MaxWidth = rec.Element("Width") != null ? int.MaxValue : 400,
                            BorderColour = xml.FixNullInline(att.Element("BorderColour"), "transparent"),
                            BackgroundColour = xml.FixNullInline(att.Element("BackgroundColour"), "transparent"),
                            ControlType = xml.FixNullInline(att.Element("ControlType"), "Textbox"),
                            DataSource = xml.FixNullInline(att.Element("DataSource"), ""),
                            DataSourceInput = xml.FixNullInlineString(att.Element("DataSourceInput"), ""),
                            DependentColumns = GetDependentColumns(att.Element("DependentColumns")),

                            Date = (att.Element("Date") == null ? "" : att.Element("Date").Value),

                            ShowCheckBoxColumn = xml.IsNull(att.Element("TemplateEditableCheckbox")),
                            TemplateEditableCheckbox = xml.FixBool(att.Element("TemplateEditableCheckbox"), false),

                            //TotalsAggregationMethod = (att.Element("TotalsAggregationMethod") == null ? "" : att.Element("TotalsAggregationMethod").Value)
                            ParentIDx = nr.Item_Idx ?? "",
                            UpdateToColumn = (att.Element("UpdateToColumn") == null ? "" : att.Element("UpdateToColumn").Value)
                        };

                        //take dropdown values from Values node if there is no data source option
                        if (p.DataSource == "" && att.Element("Values") != null)
                        {
                            p.Values = Option.GetFromXML(att.Element("Values"));
                        }


                        if (p.ControlType.ToLower() == "checkbox")
                        {
                            p.IsChecked = xml.FixBool(att.Element("Value"), false);
                        }

                        if (p.Value.StartsWith("="))
                        {
                            p.Calculation = p.Value;
                        }

                        props.Add(p);
                    }
                    catch (Exception ex)
                    {
                        var r = ex;
                    }

                    current += 1;

                }

                nr.Properties = new ObservableCollection<RowProperty>(props);

                loadedRecords.Add(nr);
            }

            #region Properties validation

            var attributes = ri.Elements("Attributes").Elements("Attribute");
            foreach (var attribute in attributes)
            {
                foreach (var xValidation in attribute.Elements("Validation"))
                {
                    if (xValidation != null)
                    {
                        foreach (var xValidationNode in xValidation.Elements())
                        {
                            string validationType = xValidationNode.Name.ToString();

                            var validationSourceColumnCode = attribute.Element("ColumnCode").Value;
                            var validationSource =
                                loadedRecords.SelectMany(record => record.Properties)
                                    .FirstOrDefault(prop => prop.ColumnCode == validationSourceColumnCode);

                            var validationTargetColumnCode = xValidationNode.Value;
                            var validationTarget =
                                loadedRecords.SelectMany(record => record.Properties)
                                    .FirstOrDefault(prop => prop.ColumnCode == validationTargetColumnCode);

                            if (validationSource == null || validationTarget == null) continue;

                            ValidationRule validationRule = ValidationRuleFactory.Create(validationSource,
                                validationTarget, validationType);

                            validationSource.Validations.Add(validationRule);
                            validationSource.PropertyChanged += (sender, args) =>
                            {
                                if (args.PropertyName == "Value")
                                    Validate(validationSource);

                                if (args.PropertyName == "IsValid")
                                {
                                    NotifyPropertyChanged(this, vm => vm.AreRecordsValid);
                                    NotifyPropertyChanged(this, vm => vm.RecordsErrorMessage);
                                }
                            };

                            // Property can become valid because of changing both the property itself and the target as well.
                            validationTarget.PropertyChanged += (sender, args) =>
                            {
                                if (args.PropertyName == "Value")
                                    Validate(validationSource);
                            };
                        }
                    }
                }
            }

            #endregion

            // var x =  ri.ToList(); 
            return CalculateColumns(loadedRecords);

        }

        /// <summary>
        /// Processes all ValidationRules for the Value of this property against other properties' Values inside of this RowViewModel.
        /// If the Value is invalid, the proper error message will be assigned to the ErrorMessage property.
        /// </summary>
        public void Validate(PropertyBase property)
        {
            if (!property.Validations.Any()) return;

            foreach (var validationRule in property.Validations)
                property.ErrorMessage = validationRule.Validate();
        }

        private List<string> GetDependentColumns(XElement attr)
        {
            if (attr == null)
                return null;

            var options = new List<string>();
            foreach (var att in attr.Elements("DependentColumn"))
            {
                options.Add(att.Value);
            }

            return options;


        }

        public void CalulateRecordColumns()
        {

            foreach (var r in Records)
            {
                CalulateRecordColumns(r);
            }

        }

        //Gets called after we should have loaded the claim Idx so seems fine
        public List<RowRecord> CalculateColumns(List<RowRecord> loadedRecords)
        {
            foreach (var records in loadedRecords)
            {
                //find calc columns and do stuff
                var r = records;
                r = CalulateRecordColumns(records);

            }

            return loadedRecords;
        }

        public RowRecord CalulateRecordColumns(RowRecord record)
        {
            var invalid = new string[] { "+", "-", "*", "/", ".", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "<", ">" };
            foreach (var col in record.Properties.Where(p => !String.IsNullOrEmpty(p.Calculation)))
            {
                try
                {
                    //right this is one that has a calc value
                    var input = col.Calculation.Replace("=", ""); //  <Value>=Num_Customers + Num_Products + Num_Promotions + Num_Terms</Value>
                    var parts = input.Split(' ');

                    var prop = 0;
                    foreach (var portion in parts)
                    {
                        if (!invalid.Contains(portion))
                        {
                            SharedMethods.ReplacePortionWithValue(ref input, portion, col.ColumnCode, null, record.Properties.ToList());
                        }
                        prop += 1;
                    }

                    ExpressionParser parser = new ExpressionParser();
                    var calcValue = parser.Parse(input);

                    col.Value = col.FormatValue(calcValue.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Calculation " + col.Calculation + " failed. Error: " + ex.Message);
                }

            }

            return record;
        }

        #region "Dummy"

        public static string DummyData = @"<Results><RootItem><Item_Idx>1</Item_Idx><Item_Type>Promotion</Item_Type><Item_RowSortOrder>1</Item_RowSortOrder><Attributes><Attribute><ColumnCode>Promo_Name</ColumnCode><HeaderText>Name</HeaderText><Values>CM Test Promo 1</Values><Format /><ForeColour>#0000FF</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Hyperlink</ControlType><DataSource /><DataSourceInput /><DependentColumns /><TotalsAggregationMethod>COUNT</TotalsAggregationMethod></Attribute></Attributes></RootItem></Results>";
        //<Attribute><ColumnCode>Author</ColumnCode><HeaderText>Author</HeaderText><Values>CM</Values><Format /><ForeColour>#006633</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DataSourceInput /><DependentColumns /><TotalsAggregationMethod>COUNT</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Start_Day</ColumnCode><HeaderText>Start Day</HeaderText><Values>2014-01-01</Values><Format>LongDate</Format><ForeColour>#660000</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DataSourceInput /><DependentColumns /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>End_Day</ColumnCode><HeaderText>End Day</HeaderText><Values>2014-06-30</Values><Format>ShortDate</Format><ForeColour>#66FFFF</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DataSourceInput /><DependentColumns /><TotalsAggregationMethod>MIN</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Create_Day</ColumnCode><HeaderText>Day Created</HeaderText><Values>2013-12-31</Values><Format>dd-mmm-yyyy</Format><ForeColour>#990000</ForeColour><BorderColour>#2E2EFE</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DataSourceInput /><DependentColumns /><TotalsAggregationMethod>MAX</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Customers</ColumnCode><HeaderText>Number Of Customers</HeaderText><Values>100</Values><Format>N0</Format><ForeColour>#996600</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DataSourceInput /><DependentColumns /><TotalsAggregationMethod>SUM</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Num_Products</ColumnCode><HeaderText>Number Of Products</HeaderText><Values>200</Values><Format>N2</Format><ForeColour>#99CC00</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DataSourceInput /><DependentColumns /><TotalsAggregationMethod>SUM</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>CalcTotalItems</ColumnCode><HeaderText>Total Items using Mechanic</HeaderText><Values>=Num_Customers + Num_Products</Values><Format>N8</Format><ForeColour>#DF0101</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DataSourceInput /><DependentColumns /><TotalsAggregationMethod>MAX</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>CalcTwiceTotalItems</ColumnCode><HeaderText>Twice Items As Currency</HeaderText><Values>=CalcTotalItems * 2</Values><Format>C8</Format><ForeColour>#DF0101</ForeColour><BorderColour>#FFFFFF</BorderColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DataSourceInput /><DependentColumns /><TotalsAggregationMethod>MAX</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Dropdown</ColumnCode><HeaderText>Test Dropdown</HeaderText><Values /><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Dropdown</ControlType><DataSource>dbo.GetDropDownData1</DataSource><DataSourceInput /><DependentColumns /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>SomeCheckbox</ColumnCode><HeaderText>Some Selection</HeaderText><Values>true</Values><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Checkbox</ControlType><DataSource /><DataSourceInput /><DependentColumns /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Mechanic</ColumnCode><HeaderText>Mechanic</HeaderText><Values><SelectedItem_Idx>1</SelectedItem_Idx><SelectedItem_Idx>2</SelectedItem_Idx><SelectedItem_Idx>3</SelectedItem_Idx></Values><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>MultiSelectDropdown</ControlType><DataSource>dbo.VerticalGetDropdownData</DataSource><DataSourceInput><ColumnCode>Mechanic</ColumnCode><User_Idx>1</User_Idx><Input /></DataSourceInput><DependentColumns /><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Feature</ColumnCode><HeaderText>Feature</HeaderText><Values><SelectedItem_Idx>1</SelectedItem_Idx><SelectedItem_Idx>2</SelectedItem_Idx><SelectedItem_Idx>3</SelectedItem_Idx></Values><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>MultiSelectDropdown</ControlType><DataSource>dbo.VerticalGetDropdowndata</DataSource><DataSourceInput><ColumnCode>Feature</ColumnCode><User_Idx>1</User_Idx><Input><ColumnCode>Mechanic</ColumnCode><Values><SelectedItem_Idx>1</SelectedItem_Idx><SelectedItem_Idx>2</SelectedItem_Idx><SelectedItem_Idx>3</SelectedItem_Idx></Values></Input></DataSourceInput><DependentColumns><DependentColumn>Mechanic</DependentColumn></DependentColumns><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute><Attribute><ColumnCode>Support</ColumnCode><HeaderText>Support</HeaderText><Values><SelectedItem_Idx>1</SelectedItem_Idx><SelectedItem_Idx>2</SelectedItem_Idx><SelectedItem_Idx>3</SelectedItem_Idx></Values><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>MultiSelectDropdown</ControlType><DataSource>dbo.VerticalGetDropdowndata</DataSource><DataSourceInput><ColumnCode>Support</ColumnCode><User_Idx>1</User_Idx><Input><ColumnCode>Mechanic</ColumnCode><Values><SelectedItem_Idx>1</SelectedItem_Idx><SelectedItem_Idx>2</SelectedItem_Idx><SelectedItem_Idx>3</SelectedItem_Idx></Values></Input><Input><ColumnCode>Feature</ColumnCode><Values><SelectedItem_Idx>1</SelectedItem_Idx><SelectedItem_Idx>2</SelectedItem_Idx><SelectedItem_Idx>3</SelectedItem_Idx></Values></Input></DataSourceInput><DependentColumns><DependentColumn>Mechanic</DependentColumn><DependentColumn>Feature</DependentColumn></DependentColumns><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute></Attributes></RootItem></Results>";

        #endregion

        public void Save(string proc, string menuIDx)
        {
            var xdoc = ToXml(menuIDx);
            var xml = XElement.Parse(xdoc.ToString());
            var res = WebServiceProxy.Call(proc, xml);
        }

        public XDocument ToXml(string menuIDx)
        {
            //fuck me this is going to be fun....
            //  /Results/RootItem/Attributes/Attribute/

            var u = new XElement("User_Idx", User.CurrentUser.ID);
            var m = new XElement("MenuItem_Idx", menuIDx);

            XDocument xdoc = new XDocument(new XElement("SaveGrid",
                from r in Records
                select new XElement("RootItem",
                    new XElement("Item_Type", r.Item_Type),
                    new XElement("Item_Idx", r.Item_Idx),
                    new XElement("Attributes",
                        from p in r.Properties
                        select new XElement("Attribute",
                            new XElement("ColumnCode", p.ColumnCode),
                            p.GetCorrectlyFormatedValue()
                            )
                            )
                            )
                            )
                            );

            xdoc.Root.Add(u);
            xdoc.Root.Add(m);

            return xdoc;
        }

        public XDocument ToCoreXml()
        {
            XDocument xdoc = new XDocument(
                from r in Records
                select new XElement("RootItem",
                    new XElement("User_Idx", Model.User.CurrentUser.ID),
                    new XElement("Item_Type", r.Item_Type),
                    new XElement("Item_Idx", r.Item_Idx),
                    new XElement("Attributes",
                        from p in r.Properties
                        select new XElement("Attribute",
                            new XElement("ColumnCode", p.ColumnCode),
                            p.GetCorrectlyFormatedValue()
                            )
                            )
                            )
                            );


            return xdoc;
        }

        public XDocument ToAttributeXml()
        {
            var xdoc = new XDocument(
                new XElement("Attributes",
                    from rt in Records
                    select new XElement("RootItem",
                        new XElement("Item_Type", rt.Item_Type),
                        new XElement("Item_Idx", rt.Item_Idx),
                        new XElement("Attributes",
                            from pp in rt.Properties
                            select new XElement(GetAttributeXElement(pp)
                                )
                                )
                                )
                                )
                                );
            return xdoc;
        }

        private XElement GetAttributeXElement(RowProperty property)
        {
            XElement attributeElement = new XElement("Attribute");

            attributeElement.Add(new XElement("ColumnCode", property.ColumnCode));

            XElement valuesElement = property.GetXmlSelectedItems();

            attributeElement.Add(valuesElement);

            if (property.TemplateEditableCheckbox)
            {
                attributeElement.Add(new XElement("TemplateEditableCheckbox", "1"));
            }
            else
            {
                attributeElement.Add(new XElement("TemplateEditableCheckbox", "0"));
            }

            return attributeElement;
        }

        #region Serialize

        /* Use this to save the VM at a given state so you can compare to a future state. */
        public XElement Serialization { get; set; }

        public void SerializeState()
        {
            Serialization = ToCoreXml().Root;
        }

        #endregion

        internal void UpdateDependentColumns(RowProperty obj)
        {
            foreach (var row in obj.DependentColumns)
            {
                MessageBox.Show(row);
            }
        }

        public override bool AreRecordsFulfilled()
        {
            return Records.All(rowRecord => rowRecord.ArePropertiesFulfilled());
        }

        /// <summary>
        /// Checks if every record has valid properties.
        /// </summary>
        public bool AreRecordsValid
        {
            get
            {
                return Records.SelectMany(rec => rec.Properties).All(prop => prop.IsValid);
            }
        }

        /// <summary>
        /// In case any record has at least one invalid property the error message is assigned here.
        /// Only the first error message found is kept.
        /// If every record has all properties valid this property contains an empty string.
        /// </summary>
        public string RecordsErrorMessage
        {
            get
            {
                if (Records == null) return string.Empty;

                var firstInvalidProperty = Records.SelectMany(rec => rec.Properties)
                    .FirstOrDefault(prop => !string.IsNullOrEmpty(prop.ErrorMessage));

                if (firstInvalidProperty == null)
                    return string.Empty;

                return firstInvalidProperty.ErrorMessage;
            }
        }
    }


}
