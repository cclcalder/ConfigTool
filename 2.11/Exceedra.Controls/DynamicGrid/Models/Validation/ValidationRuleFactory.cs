using System;

namespace Exceedra.DynamicGrid.Models.Validation
{
    public static class ValidationRuleFactory
    {
        /// <summary>
        /// Cretes a validation between two properties.
        /// </summary>
        /// <param name="source">Property being validated</param>
        /// <param name="target">Property which Value is used to validate the source property</param>
        /// <param name="type">Type of validation</param>
        /// <returns></returns>
        public static ValidationRule Create(PropertyBase source, PropertyBase target, string type)
        {
            switch (type)
            {
                case "MinValue": return new MinDateValidationRule(source, target);
                case "MaxValue": return new MaxDateValidationRule(source, target);
                default: throw new Exception("Provided validation rule type not recognized.");
            }
        }
    }
}