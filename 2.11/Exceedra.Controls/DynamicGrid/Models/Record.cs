using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using System.Xml.Serialization;
using Exceedra.Common.Utilities;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.DynamicRow.ViewModels;
using Exceedra.Controls.DynamicTab.ViewModels;
using Exceedra.DynamicGrid.Models;
using Exceedra.Schedule.ViewModels;

using Model.DataAccess;
using Exceedra.Chart.Model;

namespace Exceedra.Controls.DynamicGrid.Models
{
    [Serializable]
    public class Record : RecordBase
    {

        public ObservableCollection<Property> Clone()
        {
            return CreateDeepCopy();
        }

        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public Record() { Item_AggrType = "";
            Item_IsDisplayed = true;
        }

        public Record(Record template)
        {
            Item_Idx = template.Item_Idx;
            Item_IsDisplayed = template.Item_IsDisplayed;
            Item_Type = template.Item_Type;
            Properties = template.CloneProperties();
        }

        private ObservableCollection<Property> CloneProperties()
        {
            var props = Properties.Select(p => new Property(p));
            return new ObservableCollection<Property>(props);
        }

        //public Record(XElement r)
        //{ 
        //    this.rec = r;
        //      int rso = 0;
        //        int.TryParse(rec.Element("Item_RowSortOrder").Value, out rso);

        //        var nr = new Record()
        //        {
        //            Item_Idx = rec.Element("Item_Idx").Value,
        //            Item_Type = rec.Element("Item_Type").Value,
        //            Item_RowSortOrder = rso
        //        };
        //        var depCols = (from rrr in rec.Elements("DependentColumns") select rrr).ToList();
        //        var depCol = depCols.Elements().FirstOrDefault();
        //         var attr = (from a in rec.Elements("Attributes") select a).ToList();

        //       // var props = new List<Property>();

        //        var props = (from rr in attr.Elements("Attribute")
        //                  select new Property(rr, nr.Item_Idx, nr.Item_Type)).ToList();

        //        nr.Properties = new ObservableCollection<Property>(props);
        //}

        public static Record FromXml(XElement xml)
        {
            var serializer = new XmlSerializer(typeof(Record), new XmlRootAttribute(xml.Name.ToString()));
            var deserializedRecord = (Record)serializer.Deserialize(new StringReader(xml.ToString()));

            return deserializedRecord;
        }

        public string SelectedItems
        {
            get
            {
                var x = (from p in this.Properties where p.ColumnCode.ToLower() == "isselected" && p.Value.ToLower() == "true" select p.IDX).SingleOrDefault();
                return x;
            }
        }

        public void DeselectAll()
        {
            var x = (this.Properties.Where(p => p.ColumnCode.ToLower() == "isselected")).ToList();
            foreach (var y in x)
            {
                y.Value = "false";
            }
        }

        public void SelectAll()
        {
            var x = (this.Properties.Where(p => p.ColumnCode.ToLower() == "isselected")).ToList();
            foreach (var y in x)
            {
                y.Value = "true";
            }
        }

        public string CheckedItems(string column)
        {
            var x = (from p in this.Properties where p.ColumnCode.ToLower() == column.ToLower() && p.Value.ToLower() == "true" select p.IDX).SingleOrDefault();
            return x;
        }

        public bool CheckedItem(string column)
        {

            var x = (from p in this.Properties where p.ColumnCode.ToLower() == column.ToLower() select Convert.ToBoolean(p.Value)).Single();
            return x;

        }

        private bool _item_IsDisplayed;
        public bool Item_IsDisplayed
    {
            get
            {
                return _item_IsDisplayed;
            }
            set
            {
                _item_IsDisplayed = value;
                PropertyChanged.Raise(this, "Item_IsDisplayed");
            }
        }

        private int _hasChanges;
        public int HasChanges
        {
            get
            {
                if (_hasChanges == 0 && DetailsViewModel != null)
                {
                    var changeProperty = DetailsViewModel.GetType().GetProperty("HasChanges");
                    _hasChanges = changeProperty != null ? (int)changeProperty.GetValue(DetailsViewModel, null) : _hasChanges;
                }

                return _hasChanges;
            }
            set
            {
                _hasChanges = value;
                PropertyChanged.Raise(this, "HasChanges");
            }
        }

        public bool HasInnerGrid
        {
            get
            {
                return Properties.Any(prop =>
                    prop.ControlType == "HorizontalGrid"
                    || prop.ControlType == "VerticalGrid"
                    || prop.ControlType == "TabbedView"
                    );
            }
        }

        private ObservableCollection<Property> _properties;

        /* You cannot just return _properties.Orderby... as this restricts any ability to use Properties.Add()
         * Instead we overwrite the _properties value which allows .Add. 
         * Also, this should result in the list maintaining it's new order, so each subsequent order will be trivial.
         */
        [XmlArray("Attributes")]
        [XmlArrayItem("Attribute")]
        public ObservableCollection<Property> Properties
        {
            get { return _properties; }
            set
            {
                if (_properties == value) return;

                _properties = value;

                if (_properties != null)
                    _properties.SortBy(prop => prop.ColumnSortOrder);

                // Uncomment if you want to automatically sort columns that are added after.
                //_properties.CollectionChanged += (sender, args) =>
                //{
                //    if (args.Action == NotifyCollectionChangedAction.Add)
                //        _properties.SortBy(prop => prop.ColumnSortOrder);
                //};

                PropertyChanged.Raise(this, "Properties");
                PropertyChanged.Raise(this, "IsValid");
            }
        }

        public bool IsValid
        {
            get
            {
                if (Properties == null) return true;

                // if there's any required property with no value set
                foreach (var requiredProperty in Properties.Where(x => x.IsRequired))
                    switch (requiredProperty.ControlType.ToLower())
                    {
                        case "dropdown":
                        case "multidropdown":
                            if (requiredProperty.SelectedItem == null && requiredProperty.SelectedItems == null) return false;
                            if (requiredProperty.SelectedItem != null) break;
                            if (requiredProperty.SelectedItems != null && !requiredProperty.SelectedItems.Any()) return false;
                            break;
                        default:
                            if (string.IsNullOrEmpty(requiredProperty.Value) || !requiredProperty.IsValid) return false;
                            break;
                    }

                return true;
            }
        }

        public override bool ArePropertiesFulfilled()
        {
            if (Properties.Any(property => property.IsRequired && !property.HasValue()))
                return false;

            // checking if the inside grid (if it exists) also have the required records populated
            if (DetailsViewModel is RecordViewModel)
            {
                var details = DetailsViewModel as RecordViewModel;
                if (!details.AreRecordsFulfilled()) return false;
            }

            return true;
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        //private XElement rec;


        ObservableCollection<Property> CreateDeepCopy()
        {
            object result = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
                                            new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bf.Serialize(ms, Properties);

                ms.Position = 0;
                result = bf.Deserialize(ms);
            }

            return result as ObservableCollection<Property>;
        }

        public XElement ConvertDataSourceInput(Property property, ObservableCollection<Property> properties, string arg1 = "", string arg2 = "", string arg3 = "")
        {
            var ripProperty = XElement.Parse(property.DataSourceInput.Replace("&gt;", ">").Replace("&lt;", "<"));
            //var ripProperty = new XElement("DataSourceInput");
            //var ripProperty = XElement.Parse(property.DataSourceInput);


            foreach (var node in ripProperty.Elements())
            {
                switch (node.Name.ToString())
                {
                    case "User_Idx":
                        node.Value = Model.User.CurrentUser.ID;
                        break;

                    case "SalesOrg_Idx":
                        node.Value = Model.User.CurrentUser.SalesOrganisationID.ToString();
                        break;

                    case "AppType_Idx":
                        node.Value = arg1;
                        break;

                    case "ROB_Idx":
                        node.Value = arg2;
                        break;

                    case "ColumnCode":
                        node.Value = property.ColumnCode;
                        break;

                    //case"SalesOrg_Idx":
                    //    node.Value = property.SalesOrg_Idx;
                    //    break;

                    case "ControlsWithInput":

                        var columns = node.Elements("Column");

                        foreach (var c in columns)
                        {

                            var xElement = c.Element("ColumnCode");
                            if (xElement != null)
                            {
                                var code = xElement.Value;

                                if (!string.IsNullOrEmpty(code) && properties != null)
                                {
                                    // get property based on column code and find the selected items
                                    var column = properties.FirstOrDefault(t => t.ColumnCode == code);
                                    if (column != null && column.Value != null)
                                    {
                                        c.Element("Values").Value = column.Value;
                                    }
                                    //if (column != null && column.Value != null)
                                    //{
                                    //    if (column.SelectedItems != null)
                                    //    {
                                    //        foreach (var i in column.SelectedItems)
                                    //        {
                                    //            var element = c.Element("Values");
                                    //            if (element != null)
                                    //                element.Add(new XElement("Value", i.Item_Idx));
                                    //        }
                                    //    }
                                    //    else
                                    //    {

                                    //        var element = c.Element("Values");
                                    //        if (element != null)
                                    //            element.Add(new XElement("Value", column.SelectedItem.Item_Idx));

                                    //    }
                                    //}
                                }
                            }
                        }
                        break;
                }
            }
            return ripProperty;
        }

        private XElement ConvertDataSourceInput(Property property)
        {
            var ripProperty = XElement.Parse(property.DataSourceInput.Replace("&gt;", ">").Replace("&lt;", "<"));



            foreach (var node in ripProperty.Elements())
            {
                switch (node.Name.ToString())
                {
                    case "User_Idx":
                        node.Value = Model.User.CurrentUser.ID;
                        break;

                    case "ColumnCode":
                        node.Value = property.ColumnCode;
                        break;

                    case "SalesOrg_Idx":
                        node.Value = Model.User.CurrentUser.SalesOrganisationID.ToString();
                        break;

                    case "ControlsWithInput":

                        var columns = node.Elements("Column");

                        foreach (var c in columns)
                        {
                            var xElement = c.Element("ColumnCode");
                            if (xElement != null)
                            {
                                var code = xElement.Value;

                                if (!string.IsNullOrEmpty(code))
                                {
                                    // get property based on column code and find the selected items
                                    var column = Properties.FirstOrDefault(t => t.ColumnCode == code);
                                    if (column != null && column.Values != null)
                                    {
                                        if (column.Values.Any(a => a.IsSelected))
                                        {
                                            foreach (var i in column.Values.Where(a => a.IsSelected == true))
                                            {
                                                var element = c.Element("Values");
                                                if (element != null)
                                                    element.Add(new XElement("Value", i.Item_Idx));
                                            }
                                        }
                                        else
                                        {
                                            var element = c.Element("Values");
                                            if (element != null && column.SelectedItem != null)
                                                element.Add(new XElement("Value", column.SelectedItem.Item_Idx));
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            return ripProperty;
        }

        public void InitialDropdownLoad(Property currentProperty, bool cacheMe = false)
        {
            if (!currentProperty.IsLoaded)
            {
                if (currentProperty.Values == null)
                {
                    if (string.IsNullOrEmpty(currentProperty.DataSource)) return;

                    currentProperty.PropertyChanged -= DropdownChanged;
                    currentProperty.Values = new ObservableCollection<Option>(
                        Option.GetFromXML(ConvertDataSourceInput(currentProperty).ToString(),
                        currentProperty.DataSource, cacheMe).OrderBy(x => x.SortOrder));

                    // auto-selecting first item of a dropdown
                    if (!currentProperty.HasSelectedItems)
                        if (currentProperty.IsMultiSelectable)
                            currentProperty.SelectedItems.Add(currentProperty.Values.First());
                        else
                        {
                            currentProperty.SelectedItem = currentProperty.Values.First();
                            currentProperty.SelectedItem.IsSelected = true;
                        }


                    currentProperty.PropertyChanged += DropdownChanged;
                }
                currentProperty.IsLoaded = true;
            }

        }

        private void DropdownChanged(object sender, PropertyChangedEventArgs e)
        {

            var prop = sender as Property;
            if (prop != null && prop.Values != null && prop.Values.Any(a => a.IsSelected))
            {
               if(prop.SelectedItem != null && prop.SelectedItem.Item_Idx == prop.Values.FirstOrDefault(a => a.IsSelected).Item_Idx)
                LoadDependentDrops(prop);
            }
            else
            {
                LoadDependentDrops(prop);
            }
        }

        public void LoadDependentDrops(Property currentProperty, bool cacheMe = false)
        {
            //if (IsRowLoading)
            //{
            //    return;
            //}
            //set value to be ID of selected item
            //  currentProperty.Value = currentProperty.SelectedItem.Item_Idx;

            //are there any columns dependant on this one?
            var dependentColumn = currentProperty.DependentColumn;
            var dependents = new List<Property>();// Properties.Where(r => dependentColumns.Contains(r.ColumnCode));

            foreach (var p1 in Properties)
            {
                if (dependentColumn.Contains(p1.ColumnCode))
                {
                    dependents.Add(p1);
                }
            }

            //convert to array to stop multiple enumerations
            //var rowProperties = dependents as IList<RowProperty> ?? dependents.Where(t => !string.IsNullOrEmpty(t.DataSourceInput)).ToList();
            if (dependents.Any())
            {
                //step each valid property and load its data
                foreach (var property in dependents)
                {

                    //<DataSourceInput >
                    //    <User_Idx></User_Idx>
                    //    <Promo_Idx></Promo_Idx>
                    //    <ControlsWithInput>
                    //        <Column>
                    //          <ColumnCode>AssignedProfile_Idx</ColumnCode>
                    //          <Values/>
                    //        </Column>				
                    //    </ControlsWithInput>
                    //</DataSourceInput>

                    //rip apart the template XML and fill in the gaps...
                    var ripProperty = ConvertDataSourceInput(property);
                    //set selected column values into XML 
                    //if (!property.IsLoaded)
                    //{
                    property.PropertyChanged -= new PropertyChangedEventHandler(DropdownChanged);
                    property.Values =
                        new ObservableCollection<Option>(
                            Option.GetFromXML(ripProperty.ToString(), property.DataSource, cacheMe).OrderBy(x => x.SortOrder));
                    property.PropertyChanged += new PropertyChangedEventHandler(DropdownChanged);
                    property.IsLoaded = true;
                    // }
                    //  
                }
            }
            else // may have a proc but no dependant data?
            {
                if (!currentProperty.IsLoaded)
                {
                    currentProperty.IsLoaded = true;
                    currentProperty.PropertyChanged -= new PropertyChangedEventHandler(DropdownChanged);

                    var options = Option.GetFromXML(currentProperty.DataSourceInput,
                                    currentProperty.DataSource);

                    if (options != null)
                        currentProperty.Values = options;

                    currentProperty.PropertyChanged += new PropertyChangedEventHandler(DropdownChanged);
                    
                }
            }
        }

        private object _detailsViewModel;
        public object DetailsViewModel
        {
            get { return _detailsViewModel; }
            set
            {
                _detailsViewModel = value;
                PropertyChanged.Raise(this, "DetailsViewModel");
            }
        }

        private bool _isDetailsViewModelVisible;

        public bool IsDetailsViewModelVisible
        {
            get
            {
                return _isDetailsViewModelVisible;
            }
            set
            {
                _isDetailsViewModelVisible = value;
                PropertyChanged.Raise(this, "IsDetailsViewModelVisible");
            }
        }

        private object ConvertControlTypeToViewModel(string controlType, string controlContent)
        {
            switch (controlType)
            {
                case "HorizontalGrid":
                    var r = new RecordViewModel(XElement.Parse(controlContent));
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

                    return r;

                case "VerticalGrid":

                    var row = new RowViewModel((XElement.Parse(controlContent)));
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

                    return row;

                case "ScheduleGrid":

                    var scheduleViewModel = new ScheduleViewModel((XElement.Parse(controlContent)));
                    scheduleViewModel.StartDate = new DateTime(2014, 01, 01);
                    scheduleViewModel.EndDate = new DateTime(2016, 01, 01);

                    return scheduleViewModel;

                default: return null;
            }
        }

        public void GetTabContent()
        {
            TabbedViewModel tvm = (TabbedViewModel)DetailsViewModel;

            foreach (var tab in tvm.Records[0].Properties)
            {
                var argument = tab.ConvertDataSourceInput();

                tab.TabContent = @WebServiceProxy.Call(tab.DataSource, argument, DisplayErrors.No);

                tab.TabContent = ConvertControlTypeToViewModel(tab.ControlType, tab.TabContent.ToString());
            }
        }

        public void LoadExpandedGrid()
        {
            foreach (var prop in Properties.Where(c => c.ColumnCode == "TabbedView"))
            {
                var argument = XElement.Parse(prop.DataSourceInput.Replace("&gt;", ">").Replace("&lt;", "<"));

                var t = WebServiceProxy.Call(prop.DataSource, argument, DisplayErrors.No);

                switch (prop.ControlType)
                {
                    case "TabbedView":
                        DetailsViewModel = new TabbedViewModel(t);
                        GetTabContent();
                        break;
                    case "HorizontalGrid":
                        var recordViewModel = new RecordViewModel(t);

                        // loading dropdowns
                        foreach (var record in recordViewModel.Records)
                            foreach (var property in record.Properties.Where(r => r.ControlType.Contains("down")).ToList())
                            {
                                property.DataSourceInput = property.DataSourceInput.Replace("&gt;", ">").Replace("&lt;", "<");
                                record.InitialDropdownLoad(property);
                            }

                        prop.Value = t.ToString();
                        DetailsViewModel = recordViewModel;

                        break;
                    default:
                        return;

                }
            }
        }



        #region Helper Methods

        public string GetId()
        {
            var idProperty = GetIdProperty();

            return idProperty != null ? idProperty.Value : Item_Idx;
        }

        public Property GetProperty(string columnIdx)
        {
            return Properties.First(p => p.ColumnCode == columnIdx);
        }

        public Property GetIdProperty()
        {
            return Properties.Where(property => property.ColumnCode != null)
                .FirstOrDefault(property => property.ColumnCode.ToLower().Contains("_idx"));
        }


        public XElement GetPropertiesAsXml(string newNodeName, List<string> properties)
        {
            var newNode = new XElement(newNodeName);
            newNode.SetAttributeValue("Idx", Item_Idx);

            properties.Do(p => newNode.SetAttributeValue(p, GetProperty(p)));

            return newNode;
        }

        /* Copies the records basic properties */
        public Record GetRecordTemplate()
        {
            return new Record {Properties = Properties.Select(p => p.GetPropertyTemplate()).ToObservableCollection()};
        }

        public ObservableCollection<Datapoint> ToChartSeries()
        {
            var dataPoints = new ObservableCollection<Datapoint>();
            
            foreach (var property in Properties.Where(p => p.IsDisplayed && p.ColumnCode != "Row_Code" && p.ColumnCode != "Row_Name"))
            {
                dataPoints.Add(new CategoricalDatapoint { ID = property.IDX, X = property.HeaderText, Y = (property.Value.IsNumeric() ? (decimal?) property.Value.AsNumericDecimal() : null) });
            }

            return dataPoints;
        }

        #endregion
    }
}
