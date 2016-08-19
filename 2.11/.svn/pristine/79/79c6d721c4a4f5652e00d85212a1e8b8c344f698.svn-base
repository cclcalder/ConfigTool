namespace Exceedra.DynamicGrid.Models.Validation
{
    public abstract class ValidationRule
    {
        public PropertyBase Source { get; set; }
        public PropertyBase Target { get; set; }

        protected ValidationRule(PropertyBase source, PropertyBase target)
        {
            Source = source;
            Target = target;
        }

        /// <summary>
        /// Checks if the source property is valid against the target property.
        /// </summary>
        /// <returns>
        /// If the property is invalid returns the error message.
        /// Otherwise returns an empty string.
        /// </returns>
        public abstract string Validate();
    }
}
