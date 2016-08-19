using System;
using AutoMapper;
using Exceedra.Common.Utilities;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.DynamicRow.ViewModels;
using Exceedra.Converters;
using Exceedra.Pivot.ViewModels;
using Exceedra.SingleSelectCombo.ViewModel;
using Exceedra.TreeGrid.Models;
using Exceedra.TreeGrid.ViewModels;
using Model.DataAccess.Listings;
using Model.Entity.Generic;
using Model.Entity.Listings;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using WPF.Navigation;

namespace WPF.Pages.Dev
{
    /// <summary>
    /// Interaction logic for ControlsTest.xaml
    /// </summary>
    public partial class ControlsTest : Page, INotifyPropertyChanged
    {
        public ControlsTest()
        {
            InitializeComponent();

            var controls = new List<ComboboxItem>
            {
                new ComboboxItem { Idx = "0", IsEnabled = true, Name = "DynamicGrid", IsSelected = true },
                new ComboboxItem { Idx = "1", IsEnabled = true, Name = "SlimGrid", IsSelected = true },
                new ComboboxItem { Idx = "2", IsEnabled = true, Name = "VerticalGrid", IsSelected = true },
                new ComboboxItem { Idx = "3", IsEnabled = true, Name = "Chart", IsSelected = true },
                new ComboboxItem { Idx = "4", IsEnabled = true, Name = "Pivot", IsSelected = true },
                new ComboboxItem { Idx = "5", IsEnabled = true, Name = "TreeGrid", IsSelected = true }
            };
            ControlsVM = new SingleSelectViewModel();
            ControlsVM.SetItems(controls);
            PropertyChanged.Raise(this, "ControlsVM");
            frmMain.Navigate(new Uri("https://app.powerbi.com/view?r=eyJrIjoiZmYwMWQzM2MtN2M1NC00NjcxLWJlZDgtNWFlNGUyZmVkZmJkIiwidCI6IjJlNmJjMmRjLWZhOTMtNDYyYi1iZTNjLWNmZmMwOTZjNjU3OSIsImMiOjh9"));

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            ControlObject = new ContentObject { Type = ControlsVM.SelectedItem.Name, ViewModel = GetVM() };
        }

        private object GetVM()
        {
            var xml = XElement.Parse(XmlIn);
            switch (ControlsVM.SelectedItem.Name)
            {
                case "DynamicGrid": return new RecordViewModel(xml);
                case "SlimGrid":
                    { var test = RecordViewModel.LoadWithNewXml(xml);
                        test.Records[0].Properties.Where(p => p.ControlType.ToLower() == "dropdown").Do(p =>
                        {
                            p.Values = new System.Collections.ObjectModel.ObservableCollection<Exceedra.DynamicGrid.Models.Option> { new Exceedra.DynamicGrid.Models.Option { IsSelected = true, Item_Idx = "1", Item_Name = "Common Profile" }, new Exceedra.DynamicGrid.Models.Option { IsSelected = false, Item_Idx = "2", Item_Name = "Easter Profile" }, new Exceedra.DynamicGrid.Models.Option { IsSelected = false, Item_Idx = "3", Item_Name = "Xmas Profile" } };
                        });

                        return test;
                    }
                case "VerticalGrid": return new RowViewModel(xml);
                case "Chart": return new Exceedra.Chart.ViewModels.RecordViewModel(xml);
                case "Pivot": return ExceedraRadPivotGridViewModel.LoadWithData(xml);
                case "TreeGrid": return new TreeGridViewModel(xml, GetProductsAsTreeGridNodes());
            }

            return null;
        }

        public TreeGridNode GetProductsAsTreeGridNodes()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TreeViewHierarchy, TreeGridNode>());
            var mapper = config.CreateMapper();

            var skus = ListingsAccess.GetFilterProducts(false, true).Result.FlatTree;

            var skusAsTreeGrid = skus.Select(n => mapper.Map<TreeGridNode>(n));

            return TreeGridNode.ConvertListToTree(skusAsTreeGrid.ToList());
        }

        public SingleSelectViewModel ControlsVM { get; set; }

        private ContentObject _controlObject;
        public ContentObject ControlObject
        {
            get { return _controlObject; }
            set
            {
                _controlObject = value;
                PropertyChanged.Raise(this, "ControlObject");
            }
        }

        private string _xmlIn;
        public string XmlIn
        {
            get { return string.IsNullOrEmpty(_xmlIn) ? "Enter Xml here..." : _xmlIn; }
            set { _xmlIn = value; }
        }

        public void NavigationlinkClicked(string type, string idx, bool pop)
        {
            RedirectMe.Goto(type, idx,"","","",pop);
        }
    }


}
