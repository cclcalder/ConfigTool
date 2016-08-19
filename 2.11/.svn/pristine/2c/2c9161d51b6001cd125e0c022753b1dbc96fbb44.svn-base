using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Exceedra.CellsGrid;
using Model.Annotations;
using Exceedra.Common.Mvvm;
using Telerik.Pivot.Core;
using Telerik.Windows.Controls;

namespace Exceedra.Pivot.ViewModels
{
    public class ExceedraRadPivotGridViewModel : INotifyPropertyChanged, ICanvasErrorReportable
    {
        #region ctor

        public ExceedraRadPivotGridViewModel()
        {
            ExpandRowsUpToLevel = int.MaxValue;
            ExpandColumnsUpToLevel = int.MaxValue;
        }

        #endregion

        #region Items

        /// <summary>
        /// Collection of items with properties names (and types) set to columns
        /// and values set to rows (every row represents one item)
        /// </summary>
        public DataTable Items
        {
            get { return _items ?? (_items = new DataTable()); }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        private DataTable _items;

        #endregion

        #region Properties Definitions

        /// <summary>
        /// Collection of properties that will be used to define
        /// filters, columns, rows and values for the pivot grid
        /// </summary>
        public ObservableCollection<Property> PropertiesDefinitions
        {
            get { return _propertiesDefinitions ?? (_propertiesDefinitions = new ObservableCollection<Property>()); }
            set
            {
                _propertiesDefinitions = value;
                OnPropertyChanged("PropertiesDefinitions");
            }
        }

        private ObservableCollection<Property> _propertiesDefinitions;

        #endregion

        #region Is loading data

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        #endregion

        #region Additional settings

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public PivotLayoutType HorizontalLayout { get; set; }

        public PivotLayoutType VerticalLayout { get; set; }

        public PivotAxis AggregatesPosition { get; set; }

        public RowTotalsPosition RowGrandSubTotalsPositions { get; set; }

        public ColumnTotalsPosition ColumnGrandSubTotalsPositions { get; set; }

        public RowTotalsPosition RowGrandTotalsPositions { get; set; }

        public ColumnTotalsPosition ColumnGrandTotalsPositions { get; set; }

        public bool ExpandRows { get; set; }

        public bool ExpandColumns { get; set; }

        /// <summary>
        /// The default value set in the constructor to max int - to apply to all the rows.
        /// </summary>
        public int ExpandRowsUpToLevel { get; set; }

        /// <summary>
        /// The default value set in the constructor to max int - to apply to all the columns.
        /// </summary>
        public int ExpandColumnsUpToLevel { get; set; }

        /// <summary>
        /// Describes if the pivot grid has at least one field (row) defined
        /// </summary>
        private bool _isEmpty;
        public bool IsEmpty
        {
            get { return _isEmpty; }
            set
            {
                _isEmpty = value;
                OnPropertyChanged("IsEmpty");
            }
        }



        #endregion

        /// <summary>
        /// Creates new ExceedraRadPivotViewModel basing on xml
        /// </summary>
        /// <param name="xml"> An xml to parse to a exceedra rad pivot view model </param>
        /// <returns>
        /// <para>New view model with initialized Items, filters, columns, rows and values. </para>
        /// <para>Ready for use by ExceedraRadPivotGrid control (as a ViewModel property)</para>
        /// </returns>
        public static ExceedraRadPivotGridViewModel LoadWithData(XElement xml)
        {
            // NOTE: to load data on the pivot grid properly we use the PivotItemsBehavior class
            // which converts this view model into telerik-suitable data.

            #region xml in

            //<Results>
            //  <Canvas_Element_Idx>3</Canvas_Element_Idx>
            //  <PivotGrid_Code>AP01_PIVOT</PivotGrid_Code>
            //  <PivotMenuIsDisplayed>1</PivotMenuIsDisplayed>
            //  <PivotMenuIsEditable>1</PivotMenuIsEditable>
            //  <Settings>
            //    <Setting Code="Aggregates_Position" Value="Rows" />               <- value: Rows / Columns
            //    <Setting Code="Horizontal_Layout" Value="Compact" />              <- value: Compate / Outline / Tabular
            //    <Setting Code="Vertical_Layout" Value="Compact" />                <- value: Compate / Outline / Tabular
            //    <Setting Code="Row_Sub_Totals_Position" Value="Bottom" />         <- value: Top / Bottom / None
            //    <Setting Code="Column_Sub_Totals_Position" Value="Right" />       <- value: Right / Left / None
            //    <Setting Code="Row_Grand_Totals_Position" Value="Bottom" />       <- value: Top / Bottom / None
            //    <Setting Code="Column_Grand_Totals_Position" Value="Right" />     <- value: Right / Left / None
            //    <Setting Code="Expand_Rows" Value="True" Param="1" />             <- value: True / False; param: /optional/ /any integer/
            //    <Setting Code="Expand_Columns" Value="True" Param="1" />          <- value: True / False; param: /optional/ /any integer/
            //  </Settings>
            //  <FieldProperties>
            //    <Property Code="PropertyCodeThatActsLikeId" Name="PropertyNameVisibleOnTheScreen" Type="PropertyTypeForDataTable" Format="FormatOfValueVisibleOnTheScreen"
            //              PivotType="ToWhichPivotGridBoxItWillBeAssigned" PivotBoxSortOrder="NumberToSortWithinAPivotGridBox" AggregationType="HowTheValuesAreAggregated" GridSorting="HowToSortValuesOfThisProperty"/>
            //
            //     Type:                VARCHAR - VARCHAR(int), e.g. VARCHAR(50), or VARCHAR(MAX)  
            //                          NVARCHAR - same as VARCHAR 
            //                          DECIMAL(19,2):  The syntax is DECIMAL(int a, int b) where a is precision and b is scale, e.g. DECIMAL(5,2) is a number that has 3 digits (at most) before the decimal, and 2 after (145.67)
            //                          INT
            //                          BIGINT
            //                          FLOAT
            //                          BIT    
            //
            //     Format:              Nx / Px / Cx, where x is a number
            //
            //     PivotType:           Filter / Row / Column / Value
            //
            //     AggregationType:     Sum / Avg or Average / Count / Max / Min  
            //
            //     GridSorting:         NONE / ASC / DESC
            //
            //
            //  </FieldProperties>
            //  <Fields>
            //    <Field PropertyName1="PropertyValue" PropertyName2="PropertyValue" />
            //  </Fields>
            //</Results>

            #endregion

            ExceedraRadPivotGridViewModel viewModel = new ExceedraRadPivotGridViewModel();

            viewModel.IsLoading = true;

            #region setting title

            var xPivotGridName = xml.Element("Title");
            viewModel.Title = xPivotGridName != null ? xPivotGridName.Value : string.Empty;

            #endregion

            #region processing settings

            foreach (var xSetting in xml.Elements("Settings").Elements("Setting"))
            {
                var settingCode = xSetting.Attribute("Code").Value.Trim();
                var settingValue = xSetting.Attribute("Value").Value;
                var settingParam = xSetting.Attribute("Param") != null ? xSetting.Attribute("Param").Value : null;

                switch (settingCode)
                {
                    case "Horizontal_Layout":
                        {
                            viewModel.HorizontalLayout = (PivotLayoutType)Enum.Parse(typeof(PivotLayoutType), settingValue);
                            break;
                        }
                    case "Vertical_Layout":
                        {
                            viewModel.VerticalLayout = (PivotLayoutType)Enum.Parse(typeof(PivotLayoutType), settingValue);
                            break;
                        }
                    case "Aggregates_Position":
                        {
                            viewModel.AggregatesPosition = (PivotAxis)Enum.Parse(typeof(PivotAxis), settingValue);
                            break;
                        }
                    case "Row_Sub_Totals_Position":
                        {
                            viewModel.RowGrandSubTotalsPositions = (RowTotalsPosition)Enum.Parse(typeof(RowTotalsPosition), settingValue);
                            break;
                        }
                    case "Column_Sub_Totals_Position":
                        {
                            viewModel.ColumnGrandSubTotalsPositions = (ColumnTotalsPosition)Enum.Parse(typeof(ColumnTotalsPosition), settingValue);
                            break;
                        }
                    case "Row_Grand_Totals_Position":
                        {
                            viewModel.RowGrandTotalsPositions = (RowTotalsPosition)Enum.Parse(typeof(RowTotalsPosition), settingValue);
                            break;
                        }
                    case "Column_Grand_Totals_Position":
                        {
                            viewModel.ColumnGrandTotalsPositions = (ColumnTotalsPosition)Enum.Parse(typeof(ColumnTotalsPosition), settingValue);
                            break;
                        }
                    case "Expand_Rows":
                        {
                            viewModel.ExpandRows = bool.Parse(settingValue);

                            if (settingParam != null)
                                viewModel.ExpandRowsUpToLevel = int.Parse(settingParam);

                            break;
                        }
                    case "Expand_Columns":
                        {
                            viewModel.ExpandColumns = bool.Parse(settingValue);

                            if (settingParam != null)
                                viewModel.ExpandColumnsUpToLevel = int.Parse(settingParam);

                            break;
                        }
                }
            }

            #endregion

            #region processing properties

            foreach (var xProperty in xml.Elements("FieldProperties").Elements("Property"))
            {
                // adding properties
                var property = new Property(xProperty);
                viewModel.PropertiesDefinitions.Add(property);

                // adding columns
                DataColumn column = new DataColumn(property.Code, property.Type);
                viewModel.Items.Columns.Add(column);
            }

            // sorting properties by SortOrder
            viewModel.PropertiesDefinitions = new ObservableCollection<Property>(viewModel.PropertiesDefinitions.OrderBy(pd => pd.PivotBoxSortOrder));

            #endregion

            #region processing fields

            foreach (var xField in xml.Elements("Fields").Elements("Field"))
            {
                // adding rows
                DataRow row = viewModel.Items.NewRow();

                foreach (var fieldAttribute in xField.Attributes())
                {
                    var fieldAttName = fieldAttribute.Name.ToString();

                    DataColumn correspondingColumn = viewModel.Items.Columns[fieldAttName];

                    #region error handling

                    if (correspondingColumn == null)
                    {
                        var errorMessage = "Unable to find the property of code \"" + fieldAttName + "\"\n"
                                           + "Invalid field: " + xField;

                        viewModel.OnErrorReported(errorMessage);
                        continue;
                    }

                    #endregion

                    #region if the value is an exponential number do the trick to parse it

                    fieldAttribute.Value = ConvertExponentials(correspondingColumn.DataType, fieldAttribute.Value);

                    #endregion

                    try
                    {
                        row[correspondingColumn] = fieldAttribute.Value;
                    }

                    #region error handling

                    catch (ArgumentException aex)
                    {
                        var xInvalidProperty = xml.Elements("FieldProperties")
                            .Elements("Property")
                            .FirstOrDefault(
                                xProperty => xProperty.Attribute("Code").Value == correspondingColumn.ColumnName);

                        var invalidPropertyType = xInvalidProperty.Attribute("Type").Value;
                        var errorMessage = "Unable to assign the value of \"" + fieldAttribute.Value +
                                           "\" to the property of type \"" + invalidPropertyType + "\"\n";

                        errorMessage += "Invalid property: " + xInvalidProperty + "\n";
                        errorMessage += "Invalid field: " + xField;

                        viewModel.OnErrorReported(errorMessage);
                    }

                    #endregion

                }

                viewModel.Items.Rows.Add(row);
            }

            // After all the rows have been loaded we tell the view if the pivot grid is empty or not
            viewModel.IsEmpty = viewModel.Items.Rows.Count <= 0;

            #endregion

            viewModel.IsLoading = false;

            return viewModel;
        }

        private static string ConvertExponentials(Type fieldType, string fieldValue)
        {
            // if the field is not a number
            if (
                !(fieldType == typeof(int)) &&
                !(fieldType == typeof(long)) &&
                !(fieldType == typeof(double)) &&
                !(fieldType == typeof(decimal))
                )
                return fieldValue;

            // if the field is not an exponential number
            if (!fieldValue.Contains("e+") && !fieldValue.Contains("E+") &&
                !fieldValue.Contains("e-") && !fieldValue.Contains("E-"))
                return fieldValue;

            double exponentialNumber = double.Parse(fieldValue, NumberStyles.Float);

            string exponentialNumberString = exponentialNumber.ToString(CultureInfo.InvariantCulture);

            return exponentialNumberString;
        }

        public event CanvasErrorHandler ErrorReported;
        public void OnErrorReported(string errorMessage)
        {
            if (ErrorReported != null)
                ErrorReported(errorMessage);
        }

        public void ReportCalculatedFieldError(string errorMessage)
        {
            OnErrorReported(errorMessage);
        }

        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
