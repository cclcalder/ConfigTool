using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Model.Entity.Listings;
using Telerik.Windows.Controls;

namespace WPF.Pages
{
    /// <summary>
    /// Interaction logic for dummy.xaml
    ///// </summary>
    //public partial class dummy : Page
    //{
    //    private RecordViewModel _vm;

    //    private XElement _testData =
    //        XElement.Parse("");

    //    public dummy()
    //    {
    //        DummyList = new ObservableCollection<DummyClass>();

    //        var data = new RecordViewModel(_testData);

    //        var measures = new RecordViewModel();

    //        var measureRecords = new List<Record>();

    //        /* Build the measures grid based off the first column in our test data */
    //        foreach (var measure in data.Records.Where(r => r.Item_IsDisplayed))
    //        {
    //            var tempProperty = new Property
    //            {
    //                ColumnCode = measure.Properties[0].ColumnCode,
    //                IDX = measure.Properties[0].IDX,
    //                IsDisplayed = true,
    //                Value = measure.Properties[0].Value,
    //                IsEditable = false,
    //                ControlType = "Textbox",
    //                HeaderText = measure.Properties[0].HeaderText
    //            };

    //            var temp = new Record
    //            {
    //                Item_Idx = measure.Item_Idx,
    //                Item_Type = measure.Item_Type,
    //                Item_IsDisplayed = true,
    //                Properties = new ObservableCollection<Property> { tempProperty }
    //            };

    //            measureRecords.Add(temp);

    //        }

    //        measures.Records = new ObservableCollection<Record>(measureRecords);


    //        var skus = new DummyClass();
    //        skus.Children = new MTObservableCollection<TreeViewHierarchy>();
    //        skus.Name = "Parent1";
    //        skus.Measures = measures;
    //        skus.Data = GetTestGridData();
    //        skus.Children.Add(new DummyClass { Name = "Child1", Measures = measures, Data = GetTestGridData(), Parent = skus });
    //        skus.Children.Add(new DummyClass { Name = "Child2", Measures = measures, Data = GetTestGridData(), Parent = skus });
    //        skus.Children.Add(new DummyClass { Name = "Child3", Measures = measures, Data = GetTestGridData(), Parent = skus });

    //        skus.Children[0].Children = new MTObservableCollection<TreeViewHierarchy>();
    //        skus.Children[0].Children.Add(new DummyClass { Name = "Child1-1", Measures = measures, Data = GetTestGridData(), Parent = skus.Children[0] });
    //        skus.Children[0].Children.Add(new DummyClass { Name = "Child1-2", Measures = measures, Data = GetTestGridData(), Parent = skus.Children[0] });
    //        skus.Children[0].Children.Add(new DummyClass { Name = "Child1-3", Measures = measures, Data = GetTestGridData(), Parent = skus.Children[0] });

    //        skus.Children[1].Children = new MTObservableCollection<TreeViewHierarchy>();
    //        skus.Children[1].Children.Add(new DummyClass { Name = "Child2-1", Measures = measures, Data = GetTestGridData(), Parent = skus.Children[1] });
    //        skus.Children[1].Children.Add(new DummyClass { Name = "Child2-2", Measures = measures, Data = GetTestGridData(), Parent = skus.Children[1] });
    //        skus.Children[1].Children.Add(new DummyClass { Name = "Child2-3", Measures = measures, Data = GetTestGridData(), Parent = skus.Children[1] });

    //        skus.Children[2].Children = new MTObservableCollection<TreeViewHierarchy>();
    //        skus.Children[2].Children.Add(new DummyClass { Name = "Child3-1", Measures = measures, Data = GetTestGridData(), Parent = skus.Children[2] });
    //        skus.Children[2].Children.Add(new DummyClass { Name = "Child3-2", Measures = measures, Data = GetTestGridData(), Parent = skus.Children[2] });
    //        skus.Children[2].Children.Add(new DummyClass { Name = "Child3-3", Measures = measures, Data = GetTestGridData(), Parent = skus.Children[2] });
    //        skus.Children[2].Children.Add(new DummyClass { Name = "Child3-1", Measures = measures, Data = GetTestGridData(), Parent = skus.Children[2] });
    //        skus.Children[2].Children.Add(new DummyClass { Name = "Child3-2", Measures = measures, Data = GetTestGridData(), Parent = skus.Children[2] });
    //        skus.Children[2].Children.Add(new DummyClass { Name = "Child3-3", Measures = measures, Data = GetTestGridData(), Parent = skus.Children[2] });

    //        skus.Children[2].Children[5].Children = new MTObservableCollection<TreeViewHierarchy>();
    //        skus.Children[2].Children[5].Children.Add(new DummyClass { Name = "Child4-1", Measures = measures, Data = GetTestGridData(), Parent = skus.Children[2].Children[5] });
    //        skus.Children[2].Children[5].Children.Add(new DummyClass { Name = "Child5-2", Measures = measures, Data = GetTestGridData(), Parent = skus.Children[2].Children[5] });
    //        skus.Children[2].Children[5].Children.Add(new DummyClass { Name = "Child6-3", Measures = measures, Data = GetTestGridData(), Parent = skus.Children[2].Children[5] });


    //        InitialAggregation(skus);

    //        SetHeaders(skus.Data.Records[0].Properties.Count);

    //        skus.GetFlatTree().Do(t => t.SetChangeEvent());

    //        DummyList.Add(skus);

    //        InitializeComponent();




    //    }

    //    private void InitialAggregation(DummyClass fullTree)
    //    {
    //        var leafs = fullTree.GetFlatTree().Where(n => !n.HasChildren);

    //        var parents = fullTree.GetFlatTree().Where(n => n.HasChildren);

    //        /* Wipe all parent data before aggragation */
    //        foreach (var parent in parents)
    //        {
    //            parent.Data.Records.SelectMany(r => r.Properties).Where(p => p.ControlType.ToLower().Contains("textbox") && p.Value.IsNumeric()).Do(p => p.Value = "0");
    //        }

    //        AggregateUpOneLevel(leafs);
    //    }

    //    private void AggregateUpOneLevel(IEnumerable<DummyClass> nodes)
    //    {
    //        foreach (var node in nodes)
    //        {
    //            foreach (var record in node.Data.Records)
    //            {
    //                foreach (var property in record.Properties.Where(p => p.ControlType.ToLower().Contains("textbox") && p.Value.IsNumeric()))
    //                {
    //                    var parentProperty = ((DummyClass)node.Parent).Data.GetProperty(record.Item_Idx, property.ColumnCode);
    //                    if (!parentProperty.Value.IsNumeric())
    //                        parentProperty.Value = "0";

    //                    parentProperty.Value =
    //                        (parentProperty.Value.AsNumeric() + property.Value.AsNumeric()).ToString();
    //                }
    //            }
    //        }

    //        var newLevel = nodes.Where(n => n.Parent != null && n.Parent.Parent != null).Select(n => (DummyClass)n.Parent).Distinct();

    //        if(newLevel.Any())
    //            AggregateUpOneLevel(newLevel);
    //    }


    //    /* Test data. Skip the first column as these are our measures */
    //    public RecordViewModel GetTestGridData()
    //    {
    //        var data = new RecordViewModel(_testData);
    //        data.Records.Do(r => r.Properties = new ObservableCollection<Property>(r.Properties.Skip(1)));
    //        data.Records.SelectMany(r => r.Properties).Do(p => p.Width = "80");
    //        data.Records.SelectMany(r => r.Properties).Where(p => p.IsEditable).Do(p => p.BackgroundColour = "#B2D8B2");
            
    //        //BackgroundColour

    //        return data;
    //    }

    //    public ObservableCollection<DummyClass> DummyList { get; set; }

    //    private void DataControl_OnSelectionChanging(object sender, SelectionChangingEventArgs e)
    //    {
    //        e.Handled = true;
    //    }

    //    private void DataControl_OnSelectionChanged(object sender, SelectionChangeEventArgs e)
    //    {
    //        TreeList.SelectedItem = null;
    //    }

    //    public ObservableCollection<TextBlock> Headers { get; private set; }


    //    private void SetHeaders(int i)
    //    {
    //        Headers = new ObservableCollection<TextBlock>();

    //        for (int j = 0; j < i; j++)
    //        {
    //            var header = new TextBlock
    //            {
    //                Text = "Test",
    //                TextAlignment = TextAlignment.Center,
    //                FontSize = 9,
    //                TextWrapping = TextWrapping.Wrap,
    //                Width = 80

    //            };

    //            Headers.Add(header);
    //        }

    //    }
    //}

    //public class DummyClass : TreeViewHierarchy
    //{
    //    public DummyClass()
    //    {
    //        Measures = new RecordViewModel();
    //    }

    //    public RecordViewModel Measures { get; set; }

    //    private RecordViewModel _data;

    //    public RecordViewModel Data
    //    {
    //        get
    //        {
    //            return _data;
    //        }
    //        set
    //        {
    //            _data = value;

    //        }
    //    }

    //    public void SetChangeEvent()
    //    {
    //        Data.Records.SelectMany(r => r.Properties).Where(p => p.IsEditable && p.ControlType.ToLower().Contains("textbox")).Do(p => p.PropertyChanged += ValueChange);
    //    }

    //    private void ValueChange(object sender, PropertyChangedEventArgs e)
    //    {
    //        if (e.PropertyName == "NonInternalValue")
    //        {
    //            var recordIdx = Data.Records.First(r => r.Properties.Contains((Property) sender)).Item_Idx;
    //            if(Parent != null)
    //                ((DummyClass)Parent).Recalcuate(recordIdx, ((Property)sender).ColumnCode);

    //            Disaggregate(recordIdx, ((Property)sender).ColumnCode);
    //        }
    //    }

    //    public void Recalcuate(string recordIdx, string columnCode)
    //    {
    //        var property = Data.GetProperty(recordIdx, columnCode);
    //        property.Locked = true;

    //        float newValue = 0;
    //        Children.Select(c => ((DummyClass)c).Data.GetProperty(recordIdx, columnCode)).Do(p => newValue += p.Value.AsNumeric());

    //        property.SetValue(newValue.ToString());

    //        if(Parent != null)
    //            ((DummyClass)Parent).Recalcuate(recordIdx, columnCode);
    //    }

    //    public void Disaggregate(string recordIdx, string columnCode)
    //    {
    //        if(Children == null || !Children.Any()) return;

    //        var newValuePairs = new List<Tuple<float, Property>>();

    //        var property = Data.GetProperty(recordIdx, columnCode);
    //        var valueToSpread = property.Value.AsNumeric();

    //        var childProperties = Children.Select(c => ((DummyClass) c).Data.GetProperty(recordIdx, columnCode)).ToList();
    //        childProperties.Do(c => c.Locked = true);

    //        var dissagregationProperties = Children.Select(c => ((DummyClass) c).Data.GetProperty("1", columnCode)).ToList();

    //        if (dissagregationProperties.All(p => !p.Value.IsNumeric()))
    //        {
    //            for (int i = 0; i < childProperties.Count(); i++)
    //            {
    //                newValuePairs.Add(new Tuple<float, Property>(valueToSpread/childProperties.Count(), childProperties[i]));
    //            }
    //        }
    //        else
    //        {
    //            var totalValue = dissagregationProperties.Where(p => p.Value.IsNumeric()).Select(p => p.Value.AsNumeric()).Sum();

    //            for (int i = 0; i < childProperties.Count(); i++)
    //            {
    //                if (dissagregationProperties[i].Value.IsNumeric())
    //                    newValuePairs.Add(new Tuple<float, Property>(dissagregationProperties[i].Value.AsNumeric() / totalValue * valueToSpread, childProperties[i]));
    //                else
    //                {
    //                    newValuePairs.Add(new Tuple<float, Property>(0, childProperties[i]));
    //                }
    //            }
    //        }
            
    //        newValuePairs.Do(p => p.Item2.SetValue(p.Item1.ToString()));

    //        Children.Do(c => ((DummyClass)c).Disaggregate(recordIdx, columnCode));
    //    }

    //    public new virtual IEnumerable<DummyClass> GetFlatTree()
    //    {
    //        var l = GetChildren(this);

    //        return l.Distinct();
    //    }

    //    private IEnumerable<DummyClass> GetChildren(DummyClass parent)
    //    {
    //        yield return parent;

    //        if (parent.Children != null)
    //        {
    //            foreach (var relative in parent.Children.SelectMany(c => GetChildren((DummyClass)c)))
    //                yield return relative;
    //        }
    //    }
    //}
}
