using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Exceedra.DynamicGrid.Models;
using info.lundin.math;
using Telerik.Pivot.Core;
using Telerik.Pivot.Core.Aggregates;

namespace Exceedra.Pivot.ViewModels
{
    public class PivotCalculatedField : CalculatedField
    {
        /// <summary>
        /// When setting the equation we extract codes used in it - however we must be able to distinguish 
        /// which ones are properties codes and which ones are calculated fields codes for the pivot grid to work properly.
        /// This array contains all fields codes for the purpose described above.
        /// </summary>
        public static IEnumerable<string> CalculatedFieldsCodes { get; set; }

        public delegate void ErrorReportedHandler(string errorText);
        public event ErrorReportedHandler ErrorReported;
        protected virtual void OnErrorReported(string errortext)
        {
            if (ErrorReported != null)
                ErrorReported(errortext);
        }

        private string _equation;
        public string Equation
        {
            get { return _equation; }
            set
            {
                _equation = value;

                // Extracting codes used in the equation - we must specify them for the RequiredFields() method (a telerik's requirement)
                _codesToRequiredFields = new Dictionary<string, RequiredField>();

                foreach (var propertyCode in SharedMethods.ExtractColumnCodes(_equation).Distinct())
                {
                    _codesToRequiredFields.Add(propertyCode,
                        // Is it a calculated field code or a property code?
                        CalculatedFieldsCodes.Contains(propertyCode)
                            ? RequiredField.ForCalculatedField(propertyCode)
                            : RequiredField.ForProperty(propertyCode));
                }
            }
        }

        private List<string> equationsWithErrors = new List<string>();

        public string CalculatedFieldXml { get; set; }

        private Dictionary<string, RequiredField> _codesToRequiredFields;
        protected override IEnumerable<RequiredField> RequiredFields()
        {
            return _codesToRequiredFields.Values;
        }

        protected override AggregateValue CalculateValue(IAggregateValues aggregateValues)
        {
            var copyOfEquation = _equation;

            foreach (var propertyCode in _codesToRequiredFields.Keys)
            {
                var aggregateValue = aggregateValues.GetAggregateValue(_codesToRequiredFields[propertyCode]);

                if (aggregateValue.IsError())
                {
                    if (!equationsWithErrors.Contains(Equation))
                    {
                        var errorMessage = "Calculated field equation error. Could not find or resolve the property of code \"" + propertyCode + "\"" +
                            "\nInvalid property: " + CalculatedFieldXml + "\"\n\n";

                        OnErrorReported(errorMessage);

                        equationsWithErrors.Add(Equation);
                    }

                    return AggregateValue.ErrorAggregateValue;
                }

                var value = aggregateValue.ConvertOrDefault<double>();

                copyOfEquation = copyOfEquation.ReplaceWholeWord(propertyCode, value.ToString(CultureInfo.InvariantCulture));
            }

            ExpressionParser parser = new ExpressionParser();

            double calculatedValue;
            try { calculatedValue = parser.Parse(copyOfEquation); }
            catch (Exception) { return AggregateValue.ErrorAggregateValue; }

            var calculatedValueInEquationType = new DoubleAggregateValue(calculatedValue);
            return calculatedValueInEquationType;
        }
    }
}
