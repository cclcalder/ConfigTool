using System;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using Exceedra.Pivot.ViewModels;
using Telerik.Pivot.Core;
using Telerik.Pivot.Core.Aggregates;
using Telerik.Windows.Controls;

namespace Exceedra.Pivot.Behaviors
{
    public class PivotItemsBehavior : Behavior<RadPivotGrid>
    {
        #region ViewModel Dependency Property

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel",
                    typeof(ExceedraRadPivotGridViewModel),
                    typeof(PivotItemsBehavior),
                    new PropertyMetadata(default(ExceedraRadPivotGridViewModel)));

        public ExceedraRadPivotGridViewModel ViewModel
        {
            get
            {
                return (ExceedraRadPivotGridViewModel)GetValue(ViewModelProperty);
            }
            set
            {
                SetValue(ViewModelProperty, value);
            }
        }

        #endregion

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            // everytime when the view model changes
            // its settings are updated
            UpdateConfiguration();
        }

        /// <summary>
        /// Creates a new data source provider, layouts, expanding settings, aggregates position
        /// based on the view model and assigns it to the pivot grid
        /// </summary>
        private void UpdateConfiguration()
        {
            var pivotGridControl = AssociatedObject;
            if (pivotGridControl == null) return;

            if (ViewModel == null)
            {
                pivotGridControl.DataProvider = null;
                return;
            }

            #region creating local data source provider

            var localDataSourceProvider = new LocalDataSourceProvider();

            // to prevent from refreshing the pivot grid control
            // during assigning descriptions and items
            localDataSourceProvider.BeginInit();

            // filters
            foreach (var reportFilter in ViewModel.PropertiesDefinitions.Where(prop => prop.PivotType == PivotType.Filter))
                localDataSourceProvider.FilterDescriptions.Add(new PropertyFilterDescription { CustomName = reportFilter.Name, PropertyName = reportFilter.Code });

            // columns
            foreach (var column in ViewModel.PropertiesDefinitions.Where(prop => prop.PivotType == PivotType.Column))
            {
                var columnGroupDescriptor = ConstructColumnOrRow(column);
                localDataSourceProvider.ColumnGroupDescriptions.Add(columnGroupDescriptor);
            }

            // rows
            foreach (var row in ViewModel.PropertiesDefinitions.Where(prop => prop.PivotType == PivotType.Row))
            {
                var rowGroupDescriptor = ConstructColumnOrRow(row);
                localDataSourceProvider.RowGroupDescriptions.Add(rowGroupDescriptor);
            }

            // values & calculated fields
            // I have joined adding values and calculated fields together because they have to be added in correct order (according to their PivotBoxSortOrder property)
            var calculatedFields = ViewModel.PropertiesDefinitions.Where(prop => prop.PivotType == PivotType.CalculatedField).ToList();
            PivotCalculatedField.CalculatedFieldsCodes = calculatedFields.Select(calculatedField => calculatedField.Code);

            var valuesAndCalculatedFields = ViewModel.PropertiesDefinitions.Where(prop => prop.PivotType == PivotType.Value || prop.PivotType == PivotType.CalculatedField).ToList();
            foreach (var valueOrCalculatedField in valuesAndCalculatedFields)
            {
                if (valueOrCalculatedField.PivotType == PivotType.Value)
                {
                    var value = valueOrCalculatedField;

                    localDataSourceProvider.AggregateDescriptions.Add(new PropertyAggregateDescription
                    {
                        CustomName = value.Name,
                        PropertyName = value.Code,

                        // Default aggregation is 'sum'
                        AggregateFunction = value.AggregationType ?? AggregateFunctions.Sum,

                        StringFormat = value.Format ?? string.Empty
                    });
                }
                else if (valueOrCalculatedField.PivotType == PivotType.CalculatedField)
                {
                    var calculatedField = valueOrCalculatedField;

                    var pivotCalculatedField = new PivotCalculatedField
                    {
                        Name = calculatedField.Code,
                        Equation = calculatedField.Equation,
                        CalculatedFieldXml = calculatedField.PropertyXml.ToString()
                    };

                    pivotCalculatedField.ErrorReported += text =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ViewModel.OnErrorReported(text);
                        }));
                    };

                    localDataSourceProvider.CalculatedFields.Add(pivotCalculatedField);

                    localDataSourceProvider.AggregateDescriptions.Add(new CalculatedAggregateDescription
                    {
                        CalculatedFieldName = calculatedField.Code,
                        CustomName = calculatedField.Name,
                        StringFormat = calculatedField.Format ?? string.Empty
                    });
                }
            }

            // setting the aggregates position
            localDataSourceProvider.AggregatesPosition = ViewModel.AggregatesPosition;

            // items
            localDataSourceProvider.ItemsSource = ViewModel.Items;

            // to enable refreshing of the pivot grid control again
            localDataSourceProvider.EndInit();

            pivotGridControl.DataProvider = localDataSourceProvider;
            #endregion

            #region layouts

            pivotGridControl.HorizontalLayout = ViewModel.HorizontalLayout;
            pivotGridControl.VerticalLayout = ViewModel.VerticalLayout;

            #endregion

            #region totals positions

            pivotGridControl.RowSubTotalsPosition = ViewModel.RowGrandSubTotalsPositions;
            pivotGridControl.ColumnSubTotalsPosition = ViewModel.ColumnGrandSubTotalsPositions;

            pivotGridControl.RowGrandTotalsPosition = ViewModel.RowGrandTotalsPositions;
            pivotGridControl.ColumnGrandTotalsPosition = ViewModel.ColumnGrandTotalsPositions;

            #endregion

            #region expanding

            pivotGridControl.RowGroupsExpandBehavior = new GroupsExpandBehavior { Expanded = ViewModel.ExpandRows, UpToLevel = ViewModel.ExpandRowsUpToLevel};
            pivotGridControl.ColumnGroupsExpandBehavior = new GroupsExpandBehavior { Expanded = ViewModel.ExpandColumns, UpToLevel = ViewModel.ExpandColumnsUpToLevel };

            #endregion
        }

        private PropertyGroupDescriptionBase ConstructColumnOrRow(Property columnOrRow)
        {
            PropertyGroupDescriptionBase propertyGroupDescription;

            if (columnOrRow.Type == typeof(double)) propertyGroupDescription = new DoubleGroupDescription();
            else if (columnOrRow.Type == typeof(DateTime))
            {
                // As the Format property in the xml has no use when constructing a column or row we use it to specify the step
                DateTimeStep step;
                var parsedSuccessfully = Enum.TryParse(columnOrRow.Format, out step);

                propertyGroupDescription = new DateTimeGroupDescription { Step = parsedSuccessfully ? step : DateTimeStep.Year };
            }
            else propertyGroupDescription = new PropertyGroupDescription();

            propertyGroupDescription.PropertyName = columnOrRow.Code;
            propertyGroupDescription.CustomName = columnOrRow.Name;
            propertyGroupDescription.SortOrder = columnOrRow.GridSorting;

            return propertyGroupDescription;
        }
    }
}