namespace WPF
{
    using System;
    using System.Globalization;
    using System.Windows.Controls;

    public class NormalDateViladation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            DateTime date;

            try
            {
                if (value == null) return new ValidationResult(false, "Please specify a valid date.");
                date = DateTime.Parse(value.ToString());
            }
            catch (FormatException)
            {
                return new ValidationResult(false, "Please specify a valid date.");
            }

            //if (DateTime.Now.Date > date)
            //{
            //    return new ValidationResult(false, "Specified date should not be in the past.");
            //}
            //else
            {
                return ValidationResult.ValidResult;
            }
        }
    }
}