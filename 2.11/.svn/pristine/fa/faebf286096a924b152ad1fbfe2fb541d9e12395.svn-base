using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Model.Entity.Generic;
using Telerik.Windows.Controls;

namespace Exceedra.MultiSelectCombo.Behaviour
{
    public class FilteringBehaviour : ComboBoxFilteringBehavior
    {
        private string _filterText;

        public override List<int> FindMatchingIndexes(string text)
        {
            _filterText = text;
            var filterWords = text.ToLower().Split(' ');

            return ComboBox.Items.OfType<ComboboxItem>()
                .Where(i => i.Name != null && filterWords.All(filterWord => i.Name.ToLower().Contains(filterWord)))
                .Select(i => ComboBox.Items.IndexOf(i)).ToList();
        }

        public override int FindFullMatchIndex(ReadOnlyCollection<int> matchIndexes)
        {
            var fullMatch = ComboBox.Items.OfType<ComboboxItem>().FirstOrDefault(i => i.Name == _filterText);
            if (fullMatch == null)
            {
                return -1;
            }
            var fullMatchIndex = ComboBox.Items.IndexOf(fullMatch);
            if (matchIndexes.Contains(fullMatchIndex))
            {
                return fullMatchIndex;
            }
            return -1;
        }
    }
}
