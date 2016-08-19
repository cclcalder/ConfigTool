using System.Collections.Generic; 
using System.Linq; 
using Model.Entity.Listings;
using Telerik.Windows.Controls; 
using ViewModelBase = ViewModels.ViewModelBase;

namespace WPF.ViewModels.Generic
{
    public class HierarchyReconstructor : ViewModelBase
    { 
        private List<string> _inputHierarchies;

        public List<string> InputHierarchies
        {
            get { return _inputHierarchies; }
            set
            {
                _inputHierarchies = value;
                NotifyPropertyChanged(this, vm => vm.InputHierarchies);
            }
        }

        private List<string> _outputHierarchies;

        public List<string> OutputHierarchies
        {
            get { return _outputHierarchies; }
            set
            {
                _outputHierarchies = value;
                NotifyPropertyChanged(this, vm => vm.OutputHierarchies);
            }
        }

        public HierarchyReconstructor()
        {
            InputHierarchies = new List<string>();
            OutputHierarchies = new List<string>();
        }

        public HierarchyReconstructor(List<string> input)
        {
            InputHierarchies = new List<string>();
            InputHierarchies.AddRange(input);

            OutputHierarchies = new List<string>();
        }
 
        public List<string> ReconstructTree(
           List<TreeViewHierarchy> flatHierarchies, List<string> IDsToFind,
           List<string> reconstrctedHierarchy = null)
        {
            List<string> tempList = new List<string>();
            tempList.AddRange(IDsToFind);
            foreach (var item in flatHierarchies)
            {
                item.Children.RemoveAll(a => a.Idx != null);
            }

            InputHierarchies = new List<string>();
 
            InputHierarchies = IDsToFind;

            foreach (var item in flatHierarchies.Where(a => IDsToFind.Contains(a.Idx)))
            {
                InputHierarchies.Add(item.ParentIdx);
            }
            InputHierarchies = InputHierarchies.Distinct().ToList();

           
            return InputHierarchies;

        }
    }
}
