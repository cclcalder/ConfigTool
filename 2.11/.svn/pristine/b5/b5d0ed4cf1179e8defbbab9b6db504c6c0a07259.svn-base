using System;

namespace Exceedra.DynamicGrid.Models.Validation
{
    public class MaxDateValidationRule : ValidationRule
    {
        public MaxDateValidationRule(PropertyBase source, PropertyBase target) : base(source, target)
        {
        }

        public override string Validate()
        {
            DateTime sourceDate = new DateTime();
            bool isSourceDateParsed = DateTime.TryParse(Source.Value, out sourceDate);

            DateTime targetDate = new DateTime();
            bool isTargetDateParsed = DateTime.TryParse(Target.Value, out targetDate);

            // If one of the strings is not parsable we won't return any error
            // because checking if the string is actually a date is not the case for this validation rule.
            // We only check if the sourceDate is before the targetDate.
            if (!isSourceDateParsed || !isTargetDateParsed)
                return string.Empty;

            if (sourceDate > targetDate)
                return "The " + Source.HeaderText + " is after the " + Target.HeaderText + ". Please select valid dates.";

            return string.Empty;
        }
    }
}