using System.Windows;

namespace Exceedra.LabelControl
{
    public class LabelViewModel
    {
        public static LabelViewModel NewCenterText(string errorText)
        {
            return new LabelViewModel
            {
                Text = errorText,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        public string Text { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
    }
}
