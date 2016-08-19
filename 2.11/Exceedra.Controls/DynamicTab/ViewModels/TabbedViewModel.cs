using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.Serialization;
using Coder.UI.WPF;
using Exceedra.Common;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.UserInterface;
using Exceedra.Controls.ViewModels;
using Model;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.DynamicRow.ViewModels;
using Exceedra.Schedule.ViewModels;
using Model.DataAccess;

namespace Exceedra.Controls.DynamicTab.ViewModels
{
    [Serializable]
    public class TabbedViewModel : Base
    {
        public TabbedViewModel()
        {
            IsDataLoading = true;
            IsRunning = true;

            PanelMainMessage = "Loading dynamic data";
            PanelSubMessage = "...";

        }


        public void AddRecord(List<Property> props, string it, int c)
        {
            var nr = new Record()
            {
                Item_Idx = "0",
                Item_Type = it,
                Item_RowSortOrder = c + 1
            };

            nr.Properties = new ObservableCollection<Property>(props);

            Records.Add(nr);
            NotifyPropertyChanged(this, vm => vm.Records);
        }

        public TabbedViewModel(bool error)
        {
            IsDataLoading = false;
            IsRunning = false;

            Records = new ObservableCollection<Record>();
        }
        public TabbedViewModel(XElement res, bool footer = false)
        {
            IsDataLoading = true;
            IsRunning = true;

            PanelMainMessage = "Loading dynamic data";
            PanelSubMessage = "...";


            //Records = new ObservableCollection<Record>();
            LoadRecords2(res);

            //if (footer == true)
            //    CalulateRecordColumnTotal(Records.FirstOrDefault());

            //ColumnsFormulas();
            HasChanged = false;
        }

        public static TabbedViewModel LoadWithData(XElement res)
        {
            var instance = new TabbedViewModel();
            instance.Init(res);

            return instance;
        }

        public void Init(XElement res, bool footer = false)
        {
            LoadRecords2(res);

            HasChanged = false;
        }

        private ObservableCollection<Record> _records;

        public ObservableCollection<Record> Records
        {
            get { return _records; }
            set
            {
                if (_records != value)
                {
                    _records = value;
                    NotifyPropertyChanged(this, vm => vm.Records);
                }

                if (_records != null && _records.Any())
                {

                    PanelSubMessage = _records.Count.ToString() + " results loaded";
                }
                else
                {

                    PanelMainMessage = "No Data found";

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

        public List<Record> LoadRecords2(XElement res)
        {
            if (res == null)
            {
                return null;
            }

            var ri = (from r in res.Elements("RootItem")
                      select r).ToList();

            var loadedRecords = new List<Record>();

            foreach (var rec in ri)
            {
                int rso = 0;
                int.TryParse(rec.Element("Item_RowSortOrder").Value, out rso);

                var nr = new Record()
                {
                    Item_Idx = rec.Element("Item_Idx").Value,
                    Item_Type = rec.Element("Item_Type").Value,
                    Item_RowSortOrder = rso
                };
                var depCols = (from r in rec.Descendants("DependentColumns") select r).ToList();
                var depCol = depCols.Elements().FirstOrDefault();
                var attr = (from r in rec.Elements("Attributes") select r).ToList();

                var props = new List<Property>();
                foreach (var att in attr.Elements("Attribute"))
                {
                    var p = new Property();

                    p.External_Data = att.Element("ExternalData").MaybeValue();
                    p.ColumnCode = att.Element("ColumnCode").Value;
                    p.IsDisplayed = (att.Element("IsDisplayed").MaybeValue() ?? "1") == "1";
                    p.HeaderText = (att.Element("HeaderText") == null || att.Element("HeaderText").Value == "" ? "" : att.Element("HeaderText").Value);
                    p.ForeColour = (att.Element("ForeColour") == null || att.Element("ForeColour").Value == "" ? "#000000" : att.Element("ForeColour").Value);
                    p.BorderColour = (att.Element("BorderColour") == null || att.Element("BorderColour").Value == "" ? "transparent" : att.Element("BorderColour").Value);
                    p.BackgroundColour = (att.Element("BackgroundColour") == null || att.Element("BackgroundColour").Value == "" ? "transparent" : att.Element("BackgroundColour").Value);
                    p.IsEditable = (att.Element("IsEditable").MaybeValue() ?? "1") == "1"; //(att.Element("IsEditable").Value == "1" ? true : false);
                    p.ControlType = (att.Element("ControlType") == null ? "TextBox" : att.Element("ControlType").Value);
                    p.IDX = nr.Item_Idx;
                    p.Type = nr.Item_Type;
                    p.DataSource = (att.Element("DataSource") == null ? "" : att.Element("DataSource").Value);

                    p.DataSourceInput = (att.Element("DataSourceInput") == null ? "" : att.Element("DataSourceInput").ToString());
                    p.DependentColumn = (att.Element("DependentColumns") == null ? "" : att.Element("DependentColumns").Value);

                    if (att.Element("Value") != null && att.Element("Value").Value != "")
                    {
                        var g = att.Element("Value");

                        switch (p.ControlType.ToLower())
                        {
                            case "horizontalgrid":
                                p.TabContent = new Exceedra.Controls.DynamicGrid.ViewModels.RecordViewModel(g);
                                break;
                            case "verticalgrid":
                                p.TabContent = new RowViewModel(g);
                                break;
                            case "schedulegrid":
                                p.TabContent = new ScheduleViewModel(g);
                                break;
                            case "chart":
                                p.TabContent = new Exceedra.Chart.ViewModels.RecordViewModel(g);
                                break;
                            default:
                                p.TabContent = null;
                                break;
                        }

                    }

                    p.PropertyChanged += POnPropertyChanged;

                    #region only temporarily! As soon as we implement the schedule grid in the tab control it should be removed!
                    if (p.ControlType.ToLower() != "schedulegrid")
                    #endregion

                        props.Add(p);


                }
                nr.Properties = new ObservableCollection<Property>(props);

                loadedRecords.Add(nr);
            }

            // var x =  ri.ToList();

            Records = new ObservableCollection<Record>();
            Records.AddRange(loadedRecords);
            return loadedRecords;
        }

        private void POnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            HasChanged = true;
        }

        public void GetContent(string _appTypeID = null)
        {
            foreach (var record in Records)
            {
                foreach (var p in record.Properties) //.Where(r => r.TabContent != null)
                {
                    var argument = record.ConvertDataSourceInput(p, null, _appTypeID, record.Item_Idx);

                    // ad hoc call to DB with proc/xml
                    p.TabContent = @WebServiceProxy.Call(p.DataSource, argument.ToString(), DisplayErrors.No);

                    // convert each TabContent object into the correct type of control ready object
                    switch (p.ControlType)
                    {
                        case "HorizontalGrid":
                            var r = new RecordViewModel((XElement.Parse(p.TabContent.ToString())));
                            //@"<Results><RootItem><Item_Idx>16</Item_Idx><Item_Type>Promotion_PandL_Grid_Second</Item_Type><Item_RowSortOrder>16</Item_RowSortOrder><Attributes><Attribute><ColumnCode>TITLE</ColumnCode><HeaderText>Measure        </HeaderText><Value>Base</Value><Format></Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>1</ColumnSortOrder></Attribute><Attribute><ColumnCode>VOLUME</ColumnCode><HeaderText>Volume (Units)</HeaderText><Value>0</Value><Format>N0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>2</ColumnSortOrder></Attribute><Attribute><ColumnCode>TRADE_SPEND</ColumnCode><HeaderText>Trade Spend</HeaderText><Value>0</Value><Format>C0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>3</ColumnSortOrder></Attribute><Attribute><ColumnCode>NET_REVENUE</ColumnCode><HeaderText>Net Revenue</HeaderText><Value>0</Value><Format>C0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>4</ColumnSortOrder></Attribute><Attribute><ColumnCode>CM</ColumnCode><HeaderText>CM%</HeaderText><Value>0</Value><Format>P0</Format><ForeColour /><BorderColour>#FF0000</BorderColour><BackgroundColour>#FF0000</BackgroundColour><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>5</ColumnSortOrder></Attribute><Attribute><ColumnCode>SWP</ColumnCode><HeaderText>Customer Price</HeaderText><Value>0</Value><Format>C2</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>6</ColumnSortOrder></Attribute><Attribute><ColumnCode>SSP</ColumnCode><HeaderText>Consumer Price</HeaderText><Value>0</Value><Format>C2</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>7</ColumnSortOrder></Attribute><Attribute><ColumnCode>PROMO_ROI</ColumnCode><HeaderText>Promo ROI %</HeaderText><Value>0</Value><Format>P0</Format><ForeColour /><BorderColour>#FF0000</BorderColour><BackgroundColour>#FF0000</BackgroundColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>8</ColumnSortOrder></Attribute><Attribute><ColumnCode>RETAILER_REVENUE</ColumnCode><HeaderText>Retailer Sales</HeaderText><Value>0</Value><Format>C0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>9</ColumnSortOrder></Attribute><Attribute><ColumnCode>RM</ColumnCode><HeaderText>Retailer Margin</HeaderText><Value>0</Value><Format>P0</Format><ForeColour /><BorderColour>#FF0000</BorderColour><BackgroundColour>#FF0000</BackgroundColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>10</ColumnSortOrder></Attribute></Attributes></RootItem><RootItem><Item_Idx>16</Item_Idx><Item_Type>Promotion_PandL_Grid_Second</Item_Type><Item_RowSortOrder>16</Item_RowSortOrder><Attributes><Attribute><ColumnCode>TITLE</ColumnCode><HeaderText>Measure        </HeaderText><Value>Incremental</Value><Format></Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>1</ColumnSortOrder></Attribute><Attribute><ColumnCode>VOLUME</ColumnCode><HeaderText>Volume (Units)</HeaderText><Value>1.8e+006</Value><Format>N0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>2</ColumnSortOrder></Attribute><Attribute><ColumnCode>TRADE_SPEND</ColumnCode><HeaderText>Trade Spend</HeaderText><Value>18502.6</Value><Format>C0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>3</ColumnSortOrder></Attribute><Attribute><ColumnCode>NET_REVENUE</ColumnCode><HeaderText>Net Revenue</HeaderText><Value>831997</Value><Format>C0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>4</ColumnSortOrder></Attribute><Attribute><ColumnCode>CM</ColumnCode><HeaderText>CM%</HeaderText><Value>62.8404</Value><Format>P0</Format><ForeColour /><BorderColour>#00FF00</BorderColour><BackgroundColour>#00FF00</BackgroundColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>5</ColumnSortOrder></Attribute><Attribute><ColumnCode>SWP</ColumnCode><HeaderText>Customer Price</HeaderText><Value>0</Value><Format>C2</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>6</ColumnSortOrder></Attribute><Attribute><ColumnCode>SSP</ColumnCode><HeaderText>Consumer Price</HeaderText><Value>0</Value><Format>C2</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>7</ColumnSortOrder></Attribute><Attribute><ColumnCode>PROMO_ROI</ColumnCode><HeaderText>Promo ROI %</HeaderText><Value>0</Value><Format>P0</Format><ForeColour /><BorderColour>#FF0000</BorderColour><BackgroundColour>#FF0000</BackgroundColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>8</ColumnSortOrder></Attribute><Attribute><ColumnCode>RETAILER_REVENUE</ColumnCode><HeaderText>Retailer Sales</HeaderText><Value>18502.6</Value><Format>C0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>9</ColumnSortOrder></Attribute><Attribute><ColumnCode>RM</ColumnCode><HeaderText>Retailer Margin</HeaderText><Value>-4496.66</Value><Format>P0</Format><ForeColour /><BorderColour>#FF0000</BorderColour><BackgroundColour>#FF0000</BackgroundColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>10</ColumnSortOrder></Attribute></Attributes></RootItem><RootItem><Item_Idx>16</Item_Idx><Item_Type>Promotion_PandL_Grid_Second</Item_Type><Item_RowSortOrder>16</Item_RowSortOrder><Attributes><Attribute><ColumnCode>TITLE</ColumnCode><HeaderText>Measure        </HeaderText><Value>Planned Total</Value><Format></Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>1</ColumnSortOrder></Attribute><Attribute><ColumnCode>VOLUME</ColumnCode><HeaderText>Volume (Units)</HeaderText><Value>1.8e+006</Value><Format>N0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>2</ColumnSortOrder></Attribute><Attribute><ColumnCode>TRADE_SPEND</ColumnCode><HeaderText>Trade Spend</HeaderText><Value>18502.6</Value><Format>C0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>3</ColumnSortOrder></Attribute><Attribute><ColumnCode>NET_REVENUE</ColumnCode><HeaderText>Net Revenue</HeaderText><Value>831997</Value><Format>C0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>4</ColumnSortOrder></Attribute><Attribute><ColumnCode>CM</ColumnCode><HeaderText>CM%</HeaderText><Value>62.8404</Value><Format>P0</Format><ForeColour /><BorderColour>#00FF00</BorderColour><BackgroundColour>#00FF00</BackgroundColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>5</ColumnSortOrder></Attribute><Attribute><ColumnCode>SWP</ColumnCode><HeaderText>Customer Price</HeaderText><Value>0.567</Value><Format>C2</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>6</ColumnSortOrder></Attribute><Attribute><ColumnCode>SSP</ColumnCode><HeaderText>Consumer Price</HeaderText><Value>0.0102792</Value><Format>C2</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>7</ColumnSortOrder></Attribute><Attribute><ColumnCode>PROMO_ROI</ColumnCode><HeaderText>Promo ROI %</HeaderText><Value>2888.56</Value><Format>P0</Format><ForeColour /><BorderColour>#00FF00</BorderColour><BackgroundColour>#00FF00</BackgroundColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>8</ColumnSortOrder></Attribute><Attribute><ColumnCode>RETAILER_REVENUE</ColumnCode><HeaderText>Retailer Sales</HeaderText><Value>18502.6</Value><Format>C0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>9</ColumnSortOrder></Attribute><Attribute><ColumnCode>RM</ColumnCode><HeaderText>Retailer Margin</HeaderText><Value>0</Value><Format>P0</Format><ForeColour /><BorderColour>#FF0000</BorderColour><BackgroundColour>#FF0000</BackgroundColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>10</ColumnSortOrder></Attribute></Attributes></RootItem></Results>"

                            r.Records.Do(t =>
                            {
                                foreach (var v in t.Properties)
                                {
                                    if (v.ControlType.ToLower().Contains("down"))
                                    {
                                        t.InitialDropdownLoad(v);
                                    }
                                }
                            });
                            p.TabContent = r;

                            break;
                        case "VerticalGrid":

                            var row = new RowViewModel((XElement.Parse(p.TabContent.ToString())));
                            row.Records.Do(t =>
                            {
                                foreach (var v in t.Properties)
                                {
                                    if (v.ControlType.ToLower().Contains("down"))
                                    {
                                        t.LoadDependentDrops(v);
                                    }

                                    if (v.ControlType.ToLower().Contains("grid"))
                                    {
                                        //you b'stard what do you mean we need a grid in verticaal grid thats inside a tab inside a grid?
                                    }
                                }
                            });

                            p.TabContent = row;
                            break;
                        case "Chart":

                            break;

                        case "ScheduleGrid":

                            ScheduleViewModel scheduleGrid;

                            //if (XElement.Parse(p.TabContent.ToString()).GetElement("RowsAvailable").Value == "0")
                            //    scheduleGrid = new ScheduleViewModel();
                            //else
                            {
                                scheduleGrid = new ScheduleViewModel((XElement.Parse(p.TabContent.ToString())));
                                scheduleGrid.StartDate = new DateTime(2014, 01, 01);
                                scheduleGrid.EndDate = new DateTime(2016, 01, 01);
                            }

                            p.TabContent = scheduleGrid;
                            break;
                    }
                }
            }
        }

        public static string FixNull(string In, string Default)
        {
            if (string.IsNullOrWhiteSpace(In))
            {
                In = Default;
            }

            return In;
        }



        public static void SerializeToXml<T>(T obj, string fileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            ser.Serialize(fileStream, obj);
            fileStream.Close();
        }
        public static T DeserializeFromXml<T>(string xml)
        {
            T result;
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (TextReader tr = new StringReader(xml))
            {
                result = (T)ser.Deserialize(tr);
            }
            return result;
        }

        public static System.Xml.XmlWriter fileStream { get; set; }

        public void Save()
        {

            var xdoc = ToXml();
            var xml = XElement.Parse(xdoc.ToString());
            var res = Model.DataAccess.@WebServiceProxy.Call("[dbo].[SetData]", xml);

        }

        public XDocument ToXml()
        {
            //fuck me this is going to be fun....
            //  /Results/RootItem/Attributes/Attribute/

            XDocument xdoc = new XDocument(new XElement("Results",
                from r in Records
                select new XElement("RootItem",
                    new XElement("Item_Type", r.Item_Type),
                    new XElement("Item_Idx", r.Item_Idx),
                    new XElement("Attributes",
                        from p in r.Properties
                        select new XElement("Attribute",
                            new XElement("ColumnCode", p.ColumnCode),
                            new XElement("Value", p.Value)
                            )
                         )
                       )
                     )
                 );

            return xdoc;
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

        private string _panelMainMessage;
        /// <summary>
        /// Gets or sets the panel main message.
        /// </summary>
        /// <value>The panel main message.</value>
        public string PanelMainMessage
        {
            get
            {
                return _panelMainMessage;
            }
            set
            {
                _panelMainMessage = value;
                NotifyPropertyChanged(this, vm => vm.PanelMainMessage);
            }
        }

        private string _panelSubMessage;
        /// <summary>
        /// Gets or sets the panel sub message.
        /// </summary>
        /// <value>The panel sub message.</value>
        public string PanelSubMessage
        {
            get
            {
                return _panelSubMessage;
            }
            set
            {
                _panelSubMessage = value;
                NotifyPropertyChanged(this, vm => vm.PanelSubMessage);
            }
        }


        private bool _isRunning;
        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                _isRunning = value;
                NotifyPropertyChanged(this, vm => vm.IsRunning);
            }
        }

        /// <summary>
        /// Gets the panel close command.
        /// </summary>
        public ICommand PanelCloseCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    // Your code here.
                    // You may want to terminate the running thread etc.
                    IsDataLoading = false;
                });
            }
        }

        /// <summary>
        /// Gets the show panel command.
        /// </summary>
        public ICommand ShowPanelCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    IsDataLoading = true;
                });
            }
        }

        /// <summary>
        /// Gets the hide panel command.
        /// </summary>
        public ICommand HidePanelCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    IsDataLoading = false;
                });
            }
        }

        /// <summary>
        /// Gets the change sub message command.
        /// </summary>
        public ICommand ChangeSubMessageCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    PanelSubMessage = string.Format("Message: {0}", DateTime.Now);
                });
            }
        }



        private bool _hasChanged;

        public bool HasChanged
        {
            get { return _hasChanged; }
            set
            {
                _hasChanged = value;

                NotifyPropertyChanged(this, vm => vm.HasChanged);
            }
        }


        public bool HasRecords
        {
            get { return (Records != null || Records.Count > 0); }

        }

        private static string _filter = "Filter...";

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


    }

    class ProcHelper
    {
        public string Name { get; set; }
        public string Args { get; set; }
        public XElement Result { get; set; }
    }
}
