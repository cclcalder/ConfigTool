using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Exceedra.DynamicGrid.Models;
using Model;
using EventHandlerEx = Exceedra.Common.Utilities.EventHandlerEx;

namespace Exceedra.Controls.DynamicRow.Models
{
    [Serializable]
    public class RowRecord : RecordBase, INotifyPropertyChanged
    {
        public string HeaderText { get; set; }

        private ObservableCollection<RowProperty> _properties;
        public ObservableCollection<RowProperty> Properties
        {
            get
            {
                if (_properties == null) _properties = new ObservableCollection<RowProperty>();
                return _properties;
            }
            set
            {
                _properties = value;
                EventHandlerEx.Raise(PropertyChanged, this, "Properties");
                EventHandlerEx.Raise(PropertyChanged, this, "VisibleProperties");

                foreach (var p in _properties)
                {
                    //only fire event if its a dropdown or datepicker
                    if (p.ControlType.ToLower().Contains("dropdown") || p.ControlType.ToLower() == "datepicker")
                        p.PropertyChanged += ControlChanged;

                }

            }
        }

        public ObservableCollection<RowProperty> VisibleProperties
        {
            get
            {
                if (Properties == null) return new ObservableCollection<RowProperty>();
                return new ObservableCollection<RowProperty>(Properties.Where(p => p.IsDisplayed));
            }
        }

        public override bool ArePropertiesFulfilled()
        {
            return Properties.All(rowProperty => !rowProperty.IsRequired || rowProperty.HasValue());
        }

        private int? _width;

        public int? Width
        {
            get { return _width; }
            set
            {
                _width = value ?? 400;
                EventHandlerEx.Raise(PropertyChanged, this, "Width");
            }
        }

        private int _callCount;
        private void ControlChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Contains("SelectedItem") || e.PropertyName.Contains("Value"))
            {
                var prop = sender as RowProperty;
                if (prop.DependentColumns != null && prop.DependentColumns.Any())
                    if (prop.ControlType.ToLower() == "multiselectdropdown")
                    {
                        if (_callCount == 0)
                            LoadDependentDrops(prop);

                        _callCount = _callCount + 1;

                        if (_callCount == prop.Values.Count())
                            _callCount = 0;

                    }
                    else if (prop.ControlType.ToLower() != "multiselectdropdown")
                    {
                        _callCount = 0;
                        LoadDependentDrops(prop);
                    }
            }
        }

        /// <summary>
        /// Occurs when the selected value of a property has changed and there's a need to notify other RowRecords so they can resolve their dependencies regarding the source property.
        /// </summary>
        /// <param name="sourceRecord">The record containing the source property.</param>
        /// <param name="sourceProperty">The property that is dependent on properties from other records so that whenever it changes those properties need to be updated.</param>
        public delegate void InterColumnDependencyEventHandler(RowRecord sourceRecord, RowProperty sourceProperty);
        public event InterColumnDependencyEventHandler ResolvingInterColumnDependencies;

        /// <summary>
        /// Properties from other records which data is used to fill the DataSourceInput xml when loading dependent dropdowns
        /// </summary>
        private List<RowProperty> _otherRecordsProperties;
        public List<RowProperty> OtherRecordsProperties
        {
            get { return _otherRecordsProperties ?? (_otherRecordsProperties = new List<RowProperty>()); }
            set { _otherRecordsProperties = value; }
        }

        private Dictionary<string, string> _columnCodeToValueArgs;

        [XmlIgnore] // Without this the serialization of the screens in the promotion editor is broken because it cannot serialize the IDictionary type.
        public Dictionary<string, string> ColumnCodeToValueArgs
        {
            get { return _columnCodeToValueArgs; }
            set { _columnCodeToValueArgs = value; }
        }

        /// <param name="resolveInterColumnDependencies">Notify other records so they can resolve their dependencies regarding the currentProperty.</param>
        /// <param name="columnCodeToValueArgs">An additional ColumnCode/Value dictionary used to fill the DataSourceInput xml</param>
        public void LoadDependentDrops(RowProperty currentProperty, bool resolveInterColumnDependencies = true, Dictionary<string, string> columnCodeToValueArgs = null)
        {
            //are there any columns dependant on this one?
            var dependentColumns = currentProperty.DependentColumns.ToList();
            var dependents = Properties.Where(p1 => dependentColumns.Contains(p1.ColumnCode)).ToList();

            if (columnCodeToValueArgs == null && ColumnCodeToValueArgs != null)
                columnCodeToValueArgs = ColumnCodeToValueArgs;

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

                        var ripProperty = ConvertDataSourceInput(property, false, OtherRecordsProperties, columnCodeToValueArgs);
                        property.Values = Option.GetFromXML(ripProperty.ToString(), property.DataSource);
                        property.IsLoaded = true;
                }
            }

            // This is the mechanism to handle dependencies between multiple records - 
            // the ResolvingInterColumnDependencies is fired so that other records can intercept it and look for dependencies in their properties
            if (resolveInterColumnDependencies && ResolvingInterColumnDependencies != null)
                ResolvingInterColumnDependencies(this, currentProperty);
        }

        private XElement ConvertDataSourceInput(RowProperty property, bool isInitialLoad = false, List<RowProperty> otherRecordsProperties = null, Dictionary<string, string> columnCodeToValueArgs = null)
        {
            var ripProperty = XElement.Parse(property.DataSourceInput.Replace("&gt;", ">").Replace("&lt;", "<"));

            if (ColumnCodeToValueArgs == null && columnCodeToValueArgs != null)
                ColumnCodeToValueArgs = columnCodeToValueArgs;

            foreach (var node in ripProperty.Elements())
            {
                switch (node.Name.ToString())
                {
                    case "IsInitialLoad":
                        node.Value = isInitialLoad ? "1" : "0";
                        break;

                    case "SalesOrg":
                    case "SalesOrg_Idx":
                        node.Value = User.CurrentUser.SalesOrganisationID.ToString();
                        break;

                    case "User_Idx":
                        node.Value = User.CurrentUser.ID;
                        break;

                    case "CustLevel_Idx":
                    case "ProdLevel_Idx":
                    case "Canvas_Report_Idx":

                        if (columnCodeToValueArgs != null)
                        {
                            if (columnCodeToValueArgs != null && columnCodeToValueArgs.ContainsKey(node.Name.ToString()))
                                node.Value = columnCodeToValueArgs[node.Name.ToString()];
                        }
                        else
                        {
                            node.Value = "";
                        }

                        break;

                    case "ColumnCode":
                    case "Item_Idx":
                        node.Value = property.ColumnCode;
                        break;

                    case "Promo_Idx":
                    case "Claim_Idx":
                        node.Value = property.ParentIDx;
                        break;
              
                    case "ControlsWithInput":

                        var columns = node.Elements("Column");

                        // in case if column nodes are wrapped
                        // inside of a "Columns" node
                        if (columns == null || !columns.Any())
                            columns = node.Element("Columns").Elements("Column");

                        foreach (var c in columns)
                        {
                            var xElement = c.Element("ColumnCode");
                            if (xElement != null)
                            {
                                var code = xElement.Value;

                                if (!string.IsNullOrEmpty(code))
                                {
                                    var valuesXml = c.Element("Values");
                                    if (valuesXml == null) continue;

                                    // get property based on column code
                                    var column = Properties.FirstOrDefault(t => t.ColumnCode.ToLower() == code.ToLower());

                                    // if not found look in the otherRecordsProperties
                                    if (column == null && otherRecordsProperties != null)
                                        column = otherRecordsProperties.FirstOrDefault(t => t.ColumnCode.ToLower() == code.ToLower());

                                    // if still not found check in the columnCodeToValueArgs dictionary
                                    if (column == null && columnCodeToValueArgs != null && columnCodeToValueArgs.ContainsKey(code))
                                    {
                                        var convertedOutputValue = ConvertOutputValue(columnCodeToValueArgs[code]);
                                        valuesXml.Add(convertedOutputValue);

                                        continue;
                                    };

                                    // if still not found - give up. It's not worth it. You did all you could.
                                    if (column == null)
                                        continue;

                                    // find the selected items
                                    if (column.Values != null)
                                    {
                                        if (column.ControlType.ToLower().Contains("multi") && column.SelectedItems != null)
                                            foreach (var i in column.SelectedItems)
                                                valuesXml.Add(new XElement("Value", i.Item_Idx));
                                        else if (column.SelectedItem != null)
                                            valuesXml.Add(new XElement("Value", column.SelectedItem.Item_Idx));
                                    }

                                    // if no selected items were found
                                    // check if there is any data in Value property
                                    if (!valuesXml.HasElements && column.Value != null)
                                    {
                                        var convertedOutputValue = ConvertOutputValue(column.Value);
                                        valuesXml.Add(convertedOutputValue);
                                    }
                                }
                            }
                        }
                        break;

                    default:
                        if (columnCodeToValueArgs != null && columnCodeToValueArgs.ContainsKey(node.Name.ToString()))
                            node.Value = columnCodeToValueArgs[node.Name.ToString()];
                        break;
                }
            }

            return ripProperty;
        }

        private XElement ConvertOutputValue(string value)
        {
            string convertedValue = value;

            // if the value is a date time put it into the yyyy-MM-dd format (that suites the database)
            DateTime dateTimeValue;
            bool isDate = DateTime.TryParse(value, out dateTimeValue);
            if (isDate) convertedValue = String.Format("{0:yyyy-MM-dd}", dateTimeValue);

            return new XElement("Value", convertedValue);
        }


        public void InitialDropdownLoad(RowProperty currentProperty, Dictionary<string, string> additionalValues = null)
        {
            if (!currentProperty.IsLoaded)
            {
                if (currentProperty.Values == null)
                {
                    // The code below is uncommented because we don't want to notify the ControlChanged method 
                    // that the selected item in some dropdown has changed and we need to load the dropdown's dependencies.
                    // We don't want to load any dependencies during the dynamic row control initialization - 
                    // it's a db responsibility to provide all the data initially.
                    // We turn the notifying back after the initialization, when we know it was triggered by a user.

                    //currentProperty.PropertyChanged -= ControlChanged;
                    currentProperty.PropertyChanged -= ControlChanged;

                    currentProperty.Values = Option.GetFromXML(ConvertDataSourceInput(currentProperty, true, null, additionalValues).ToString(),
                        currentProperty.DataSource);

                    //currentProperty.PropertyChanged += ControlChanged;
                    currentProperty.PropertyChanged += ControlChanged;

                }

                currentProperty.IsLoaded = true;
            }
        }

        public new event PropertyChangedEventHandler PropertyChanged;
    }
}
