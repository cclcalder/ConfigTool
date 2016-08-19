using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Exceedra.Common.Utilities;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.TreeGrid.Models;
using Exceedra.Common;
using Model.Annotations;
using Model.Entity.Listings;
using Property = Exceedra.Controls.DynamicGrid.Models.Property;
using ChartViewModel = Exceedra.Chart.ViewModels.RecordViewModel;
using ChartModel = Exceedra.Chart.Model;
using Model.Entity.Generic;
using Exceedra.MultiSelectCombo.ViewModel;
using Exceedra.SingleSelectCombo.ViewModel;
using System.Windows.Input;
using Model;
using Exceedra.TreeGrid.Controls;
using System.Windows.Media;

namespace Exceedra.TreeGrid.ViewModels
{
    public class TreeGridViewModel : INotifyPropertyChanged
    {
        public TreeGridViewModel()
        {
            LoadedNodes = new List<TreeGridNode>();
            Headers = new List<TextBlockWithIcon> { new TextBlockWithIcon
            { InnerTextBlock = new TextBlock
            {
                Width = 100,
                Visibility = Visibility.Visible,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Text = "No Data",
                FontSize = 9
            }}
            };
            IsLoading = false;
        }

        public TreeGridViewModel(TreeGridNode fullTree)
        {
            BuildHeaders(fullTree);

            VisibleNodes = LoadedNodes = new List<TreeGridNode> { fullTree };
        }

        public TreeGridViewModel(XElement xml, TreeGridNode emptyHierarchy)
        {
            IsLoading = true;
            LoadedNodes = new List<TreeGridNode>();

            var roots = xml.Elements("Root").ToList();

            if (!roots.Any())
            {
                roots.Add(xml);
            }

            foreach (var root in roots)
            {
                var grids = root.Elements("Grid").ToList();

                var templateXml = grids.First().Element("Rows");

                var nodes = grids.Select(g => new TreeGridNode(g));

                var parentAdditionalIdxs = (root.Element("ParentNodes") != null) ? root.Element("ParentNodes").Elements().Select(e => new Tuple<string, string>(e.Attribute("Idx").Value, e.Attribute("Seasonal_Idx").MaybeValue())) : null;

                var tree = BuildTree(nodes, TreeGridNode.CloneTreeGridNode(emptyHierarchy), templateXml, parentAdditionalIdxs);

                var nameOverride = root.Element("NameOverride").MaybeValue();
                if (nameOverride != null)
                    tree.Name = nameOverride;

                LoadedNodes.Add(tree);
            }

            ChartMetaData = xml.Element("MetaData");

            VisibleNodes = LoadedNodes;

            BuildChart(LoadedNodes);

            IsLoading = false;
        }

        /* Built this method so you can parse in the prebuilt nodes. Did this for Planning. */
        public TreeGridViewModel(IEnumerable<TreeGridNode> dataNodes, TreeGridNode emptyHierarchy, XElement gridTemplateXml, XElement metaData = null)
        {
            IsLoading = true;

            ChartMetaData = metaData;

            VisibleNodes = LoadedNodes = new List<TreeGridNode> { BuildTree(dataNodes, emptyHierarchy, gridTemplateXml) };

            BuildChart(LoadedNodes);

            IsLoading = false;
        }

        private TreeGridNode BuildTree(IEnumerable<TreeGridNode> dataNodes, TreeGridNode emptyHierarchy, XElement gridTemplateXml, IEnumerable<Tuple<string, string>> parentAdditionalIdxs = null)
        {

            /* Build out base tree where the leafs have Data (RecordViewModels) */
            var newTree = BuildHierarchy(dataNodes, emptyHierarchy);

            /* If we are given additional data about the parents, then we assign it here. This was initially done to apply seasonal at all levels */
            /* Sometimes at this point we will have cut off parents if they are unnecessary, so we also do a null check in the linq statement. */
            /* This becuase the db doesnt do any tree trimming. */
            if (parentAdditionalIdxs != null)
            {
                var flatTree = TreeViewHierarchy.GetFlatTree(newTree);
                parentAdditionalIdxs.Do(i =>
                {
                    var parent = flatTree.FirstOrDefault(n => n.Idx == i.Item1);
                    if (parent != null) //If we have not removed the parent...
                        parent.AdditionalIdx = i.Item2;
                });
            }

            /* Fill in all the non-leafs Data with a RecordViewModel. 
             * This just needs to be a grid template as all the values will be overwritten by InitialAggregation */
            TreeViewHierarchy.GetFlatTree(newTree).Where(n => n.Data == null).Do(n => n.Data = GetGridTemplate(gridTemplateXml));

            /* Search and set disaggrigation Idxs */
            /* This is poor code, but since only planning and forecasting require this functionality (at the moment) it was not worth developing a dynamic method */
            var forecastRecord = newTree.Data.Records.FirstOrDefault(r => r.Item_Name.ToLower().Contains("saved"));//Saved Forecast
            // var actualsRecord = newTree.Data.Records.FirstOrDefault(r => r.Item_Name.ToLower().Contains("actuals"));

            var overrideRecord = newTree.Data.Records.FirstOrDefault(r => r.Item_Name.ToLower().Contains("override"));
            var baseRecord = newTree.Data.Records.FirstOrDefault(r => r.Item_Name.ToLower().Contains("base"));

            var dissageIdxs = new List<string>();

            if (forecastRecord != null) dissageIdxs.Add(forecastRecord.Item_Idx);
            //if (actualsRecord != null) dissageIdxs.Add(actualsRecord.Item_Idx);
            if (overrideRecord != null) dissageIdxs.Add(overrideRecord.Item_Idx);
            if (baseRecord != null) dissageIdxs.Add(baseRecord.Item_Idx);

            TreeViewHierarchy.GetFlatTree(newTree).Do(n => n.DisaggregationIdxs = dissageIdxs);

            /* Set the row height to 25 if we have dropdowns, otherwise 20  */
            double height = newTree.Data.Records.SelectMany(r => r.Properties).Any(p => p.ControlType.ToLower() == "dropdown") ? 25 : 20;
            TreeViewHierarchy.GetFlatTree(newTree).Do(n => n.RowHeight = height);

            /* Give each node a measures grid */
            var measures = BuildMeasures(newTree);
            TreeViewHierarchy.GetFlatTree(newTree).Do(n => n.Measures = measures);

            /* Give each node a sum grid */
            TreeViewHierarchy.GetFlatTree(newTree).Do(n => n.Sums = BuildSums(newTree));

            InitialSetup(newTree);

            BuildHeaders(newTree);

            return newTree;
        }

        private void InitialSetup(TreeGridNode newTree)
        {
            PerformAllCalculations(newTree.GetLeafs());

            InitialAggregation(newTree);

            TreeViewHierarchy.GetFlatTree(newTree).Do(n => n.RecalcuateSums());

            /* Now we have our initial aggregation, we can apply the change events for user changes */
            TreeViewHierarchy.GetFlatTree(newTree).Do(n =>
            {
                SetEvents(n);
            });

            /* We only use planning level when saving, i.e. the lowest level aka the leafs. 
             * So we only need to watch the children for changes. */
            newTree.GetLeafs().Do(l =>
            {
                l.IveChanged += NotifyChanges;
                l.CommentChanged += NotifyCommentsChanged;
                l.CalculatedChanged += UpdatedCalulations;
            });
        }

        private void SetEvents(TreeGridNode n)
        {
            n.SetChangeEvent();
            n.RecordChanged += UpdateSeries;
            n.RecordChanged += UpdateSums;
            //n.RecordChanged += UpdateTotalsMeasure;
            //n.ValueChanged += UpdateTotalsMeasure;

        }
        private void UnSetEvents(TreeGridNode n)
        {
            n.UnSetChangeEvent();
            n.RecordChanged -= UpdateSeries;
            n.RecordChanged -= UpdateSums;
        }

        public void CleanseTree()
        {
            LoadedNodes.SelectMany(n => TreeViewHierarchy.GetFlatTree(n)).Do(n =>
            {
                UnSetEvents(n);
            });

            LoadedNodes.SelectMany(n => n.GetLeafs()).Do(l =>
            {
                l.IveChanged -= NotifyChanges;
                l.CommentChanged -= NotifyCommentsChanged;
                l.CalculatedChanged -= UpdatedCalulations;
                l.Data.Records.SelectMany(r => r.Properties).Do(p =>
                {
                    p.OriginalValue = p.Value;
                    p.OriginalValue2 = p.Value2;
                    p.HasChanged = false;
                });
            });

            InitialSetup(LoadedNodes[0]);

            RefreshSeries();
        }

        /* We use this top populate the parent nodes as we never load their data. The app uses aggregation to work it out */
        private RecordViewModel GetGridTemplate(XElement xml)
        {
            var rvm = TreeGridNode.ConvertXmlToGrid(xml);

            /* Clear out any existing values */
            rvm.Records.SelectMany(r => r.Properties).Where(p => p.Value.IsNumeric()).Do(p => p.Value = "");
            rvm.Records.SelectMany(r => r.Properties).Where(p => p.ControlType.ToLower().Contains("check")).Do(p => p.Value2 = "0");
            rvm.Records.SelectMany(r => r.Properties).Do(p =>
            {
                p.CommentList = new List<ListboxComment>();
                p.CommentColour = "#9966CC"; //Use purple to indicate comments are being aggregated from the children
            });
            return rvm;
        }

        private TreeGridNode BuildHierarchy(IEnumerable<TreeGridNode> dataNodes, TreeGridNode emptyHierarchy)
        {
            var leafs = emptyHierarchy.GetLeafs().ToList();

            foreach (var node in dataNodes)
            {
                var matchedNode = leafs.First(l => l.Idx == node.ParentIdx);
                matchedNode.Data = node.Data;
                matchedNode.AdditionalIdx = node.AdditionalIdx;
            }

            RemoveDatalessLeafs(emptyHierarchy);
            TreeViewHierarchy.RemoveChildlessParents(emptyHierarchy);
            emptyHierarchy = TreeViewHierarchy.RemoveSingularParents(emptyHierarchy);

            /* This isn't empty, it's actually the full tree of data */
            return emptyHierarchy;
        }

        private RecordViewModel BuildMeasures(TreeGridNode node)
        {
            var rvm = new RecordViewModel(false) { Records = new ObservableCollection<Record>() };
            rvm.GridTitle = "";

            node.Data.Records.Where(r => r.Item_IsDisplayed).Do(r => rvm.AddRecord(new Record { Item_Name = r.Item_Name, Item_Idx = r.Item_Idx, Item_IsDisplayed = true, Item_Type = "Measure", Properties = new ObservableCollection<Property> { new Property { Value = r.Item_Name, ColumnCode = "Measure", HeaderText = "Measure", IDX = "1", IsEditable = false, IsDisplayed = true, ControlType = "textbox", Alignment = "Left", BackgroundColour = "#FFFFFFFF", BorderColour = r.Item_Colour } } }));
            return rvm;
        }

        private RecordViewModel BuildSums(TreeGridNode node)
        {
            var rvm = new RecordViewModel(false) { Records = new ObservableCollection<Record>() };
            rvm.GridTitle = "";

            node.Data.Records.Where(r => r.Item_IsDisplayed).Do(r => rvm.AddRecord(new Record { Item_Name = r.Item_Name, Item_Idx = r.Item_Idx, Item_IsDisplayed = true, Item_Type = "Sum", Properties = new ObservableCollection<Property> { new Property { Value = "0", ColumnCode = "0", HeaderText = "Sum", IDX = "0", IsEditable = false, IsDisplayed = true, ControlType = "textbox", StringFormat = "N0", Alignment = "Left", BackgroundColour = "#FFFFFFFF", BorderColour = "#FFFFFFFF" } } }));
            return rvm;
        }

        private List<TextBlockWithIcon> _headers;
        public List<TextBlockWithIcon> Headers
        {
            get { return _headers; }
            set
            {
                _headers = value;
                PropertyChanged.Raise(this, "Headers");
            }
        }

        private void BuildHeaders(TreeGridNode node)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    var temp = new List<TextBlockWithIcon>();

                    node.Data.Records.First(r => r.Item_IsDisplayed).Properties.Do(p =>
                    {
                        var text = p.HeaderText;
                        var index = text.IndexOf('[');
                        if (index > 1)
                            text = text.Insert(index - 1, Environment.NewLine);

                        var tbwi = new TextBlockWithIcon();

                        var tb = new TextBlock
                        {
                            TextAlignment = TextAlignment.Center,
                            TextWrapping = TextWrapping.Wrap,
                            Text = text,
                            FontSize = p.IsExpandable ? 11 : 9,
                            Tag = p.IDX,
                            FontWeight = p.IsExpandable ? FontWeights.Bold : FontWeights.Normal,
                            Foreground = p.IsExpandable ? new BrushConverter().ConvertFromString("#000000") as Brush : new BrushConverter().ConvertFromString("#555555") as Brush
                        };

                        tbwi.InnerTextBlock = tb;
                        tbwi.InnerIcon = p.IsExpandable ? (int)FontAwesome.WPF.FontAwesomeIcon.Plus : 0;
                        tbwi.Visibility = p.IsDisplayed ? Visibility.Visible : Visibility.Collapsed;
                        tbwi.Background = p.IsExpandable ? new BrushConverter().ConvertFromString("#DDDDDD") as Brush : new BrushConverter().ConvertFromString("#00FFFFFF") as Brush;
                        tbwi.Width = p.Width.IsNumeric() ? p.Width.AsNumericInt() : 80;
                        tbwi.Tag = p.IDX;
                        tbwi.Margin = new Thickness(0, -5, 0, -5);
                        tbwi.Height = 30;                    

                        temp.Add(tbwi);
                    });

                    Headers = temp;
                }));

        }


        private static bool RemoveDatalessLeafs(TreeGridNode ti)
        {
            if (ti.HasChildren)
                ti.Children.Remove(ti.Children.Where(c => RemoveDatalessLeafs((TreeGridNode)c)).ToList());

            if (!ti.HadChildrenInitially && ti.Data == null)
                return true;

            return false;
        }

        /* Storage for all the nodes/trees supplied by the DB */
        private List<TreeGridNode> _loadedNodes;
        public List<TreeGridNode> LoadedNodes
        {
            get
            {
                return _loadedNodes;
            }
            set
            {
                _loadedNodes = value;
                PropertyChanged.Raise(this, "LoadedNodes");
            }
        }

        /* Subset of LoadedNodes that should be displayed.
         * Separation is required to allow users to view the data in different modes, e.g. hierarchical, single or leaf.
         */
        private List<TreeGridNode> _visibleNodes;
        public List<TreeGridNode> VisibleNodes
        {
            get
            {
                return _visibleNodes;
            }
            set
            {
                _visibleNodes = value;
                PropertyChanged.Raise(this, "VisibleNodes");
            }
        }

        private TreeGridNode _selectedNode;
        public TreeGridNode SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                _selectedNode = value;

                if (SelectedNode != null)
                {
                    var idx = SelectedNode.Idx + "$";
                    var name = "$" + SelectedNode.Name;
                    ChartDropdown.Items.Do(i => i.IsSelected = (i.Idx.StartsWith(idx) && i.Idx.EndsWith(name)));
                }
                PropertyChanged.Raise(this, "SelectedNode");
            }
        }

        private void UpdateSums(Tuple<string, TreeGridNode> changedRecord)
        {
            //Update row total
            changedRecord.Item2.RecalcuateSums(changedRecord.Item1);
        }

        /* rowIdx, ColumnCode, TreeNode */
        private void UpdatedCalulations(Tuple<Tuple<string, string>, TreeGridNode> changedRecord)
        {
            UpdateTotalsMeasure(changedRecord);

            UpdateColumnCalculations(changedRecord.Item1.Item2, changedRecord.Item2);
            //UpdateRecordCalculations(changedRecord.Item1.Item1, changedRecord.Item1.Item2, changedRecord.Item2);
        }

        /* On load we perfrom all the calculations so everything has the correct visible value. We then update the original value for everything that changed so we can determine if things changed. */
        private void PerformAllCalculations(IEnumerable<TreeGridNode> leafs)
        {
            leafs.Do(l => l.Data.Records.First().Properties.Do(p => UpdateColumnCalculations(p.ColumnCode, l).Do(r => r.Properties.Do(calcProp => calcProp.OriginalValue = calcProp.Value))));
        }

        private IEnumerable<Record> UpdateColumnCalculations(string columnCode, TreeGridNode node)
        {
            /* Updated calucations (if there are any) */
            var calculatedRecords = node.Data.CalculateColumn(columnCode);

            calculatedRecords.Do(r => UpdateSeries(new Tuple<string, TreeGridNode>(r.Item_Idx, node)));

            if (node.Parent != null)
                calculatedRecords.Do(r => AggregateUpOneLevel(new List<TreeGridNode> { (TreeGridNode)node.Parent }, r.Item_Idx, columnCode));

            return calculatedRecords;
        }

        //private void UpdateRecordCalculations(string rowIdx, string columnCode, TreeGridNode node)
        //{
        //    if (string.IsNullOrEmpty(node.Data.GetProperty(rowIdx, columnCode).Calculation))
        //    {
        //        var calculatedRecord = node.Data.CalulateRecordColumns(rowIdx, columnCode);

        //        UpdateSeries(new Tuple<string, TreeGridNode>(calculatedRecord.Item_Idx, node));
        //        AggregateUpOneLevel(new List<TreeGridNode> { (TreeGridNode)node.Parent }, calculatedRecord.Item_Idx);
        //    }
        //}

        /* Custom method for computing the Total Volume value when one of the leafs properties changes. */
        private void UpdateTotalsMeasure(Tuple<Tuple<string, string>, TreeGridNode> changedRecord)
        {
            var columnCode = changedRecord.Item1.Item2;

            var totalVolumeRecord = changedRecord.Item2.Data.Records.FirstOrDefault(r => r.Item_Name == "Total Volume");
            if (totalVolumeRecord != null)
            {
                var baseRecord = changedRecord.Item2.Data.GetRecordByName("Base Volume");
                var overrideRecord = changedRecord.Item2.Data.GetRecordByName("Base Override");

                var totalProperty = totalVolumeRecord.GetProperty(columnCode);

                if (!totalProperty.IsDisplayed || !totalProperty.Value.IsNumeric()) return;

                //foreach (var p in totalVolumeRecord.Properties.Where(p => p.IsDisplayed && p.Value.IsNumeric()))
                //{
                var baseProperty = baseRecord.GetProperty(columnCode);
                var overrideProperty = overrideRecord.GetProperty(columnCode);

                int newTotal = totalProperty.OriginalValue.AsNumericInt();

                if (overrideProperty.HasChanged)
                {
                    //If we had an inital override
                    if (overrideProperty.OriginalValue.AsNumericInt() != 0)
                    {
                        var originalOverride = overrideProperty.OriginalValue.AsNumericInt() == -1 ? 0 : overrideProperty.OriginalValue.AsNumericInt();

                        //but now its been set to 0
                        if (overrideProperty.Value.AsNumericInt() == 0)
                        {
                            newTotal += (-originalOverride) + baseProperty.Value.AsNumericInt();
                        }
                        else
                        {
                            newTotal += (-originalOverride) + (overrideProperty.Value.AsNumericInt() == -1 ? 0 : overrideProperty.Value.AsNumericInt());
                        }
                    }
                    else //If we didn't have an inital override
                    {
                        //but now we do, use that instead of base
                        if (overrideProperty.Value.AsNumericInt() != 0)
                        {
                            newTotal += (-baseProperty.Value.AsNumericInt()) + (overrideProperty.Value.AsNumericInt() == -1 ? 0 : overrideProperty.Value.AsNumericInt());
                        }
                    }
                }

                //Remove all original adjustments and add on the current ones...
                foreach (var record in changedRecord.Item2.Data.Records)
                {
                    if (record.Item_Name == "Base Volume") continue;
                    if (record.Item_Name == "Base Override") continue;

                    var prop = record.GetProperty(columnCode);
                    if (prop.IsEditable && prop.HasChanged)
                    {
                        newTotal += (prop.Value.AsNumericInt() - prop.OriginalValue.AsNumericInt());
                    }
                }

                if (totalProperty.Value.AsNumericInt() != newTotal)
                {
                    totalProperty.Locked = true;
                    totalProperty.SetValue(newTotal.ToString());
                    if (changedRecord.Item2.Parent != null)
                        AggregateUpOneLevel(new List<TreeGridNode> { (TreeGridNode)changedRecord.Item2.Parent }, totalVolumeRecord.Item_Idx, changedRecord.Item1.Item2);
                    UpdateSeries(new Tuple<string, TreeGridNode>(totalVolumeRecord.Item_Idx, changedRecord.Item2));
                }
                //}
            }
        }

        public TreeGridNode GetNode(string idx)
        {
            return LoadedNodes.SelectMany(n => TreeViewHierarchy.GetFlatTree(n)).First(n => n.Idx == idx);
        }

        public IEnumerable<TreeGridNode> GetFlatLeafs()
        {
            return LoadedNodes.SelectMany(n => n.GetLeafs());
        }

        public IEnumerable<TreeGridNode> GetFlatTree()
        {
            return LoadedNodes.SelectMany(n => TreeViewHierarchy.GetFlatTree(n));
        }

        public IEnumerable<RecordViewModel> GetFlatDataGrids()
        {
            return GetFlatTree().Select(n => n.Data);
        }

        #region Charting
    
        private XElement ChartMetaData { get; set; }

        private void BuildChart(List<TreeGridNode> rootNodes)
        {
            ChartDropdown.PropertyChanged += ChartDropdownOnPropertyChanged;

            var nodes = rootNodes.SelectMany(n => TreeViewHierarchy.GetFlatTree(n));

            SeriesData = nodes.SelectMany(n => n.Data.Records.Where(r => r.Item_IsDisplayed).Select(r => new Tuple<ComboboxItem, Record>(new ComboboxItem { Name = n.Name + " - " + r.Item_Name, Idx = n.Idx + "$" + r.Item_Idx + "$" + n.Name, IsEnabled = true }, r))).ToList();

            SeriesData.First().Item1.IsSelected = true;

            ChartDropdown.SetItems(SeriesData.Select(s => s.Item1));
        }

        /* If a new series is selected, reset the entire chart. If a just a value changes, updated that series */
        private void ChartDropdownOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItems")
            {
                ChartVM.IsLoading = true;
                var chart = GetBaseChart();

                chart.Series = GetSeriesToShow();
                
                ChartVM = new ChartViewModel(false) { Chart = chart };
                ChartVM.LoadMetaData(ChartMetaData);

                ConfigureChartOutliers();
            }
            //else if (e.PropertyName == "UpdateSeries")
            //{
            //    UpdateSeries((Record)sender);
            //}
        }

        private void UpdateSeries(Tuple<string, TreeGridNode> changedRecord)
        {
            var measureChartIdx = changedRecord.Item2.Idx + "$" + changedRecord.Item1 + "$" + changedRecord.Item2.Name;

            /* Find the series with changes */
            var seriesToUpdate = ChartDropdown.SelectedItems.FirstOrDefault(i => i.Idx == measureChartIdx);

            /* If we are not displaying that series, do nothing */
            if (seriesToUpdate == null) return;

            /* Get its new set of datapoints */
            var datapoints = SeriesData.First(s => s.Item1.Idx == measureChartIdx).Item2.ToChartSeries();

            /* Override this series existing datapoints */
            ChartVM.Chart.Series.First(s => s.SeriesName == seriesToUpdate.Name).Datapoints = datapoints;

            ConfigureChartOutliers();
        }

        public void RefreshSeries()
        {
            if (SelectedNode != null)
                ChartVM.Chart.Series.Do(s => s.Datapoints = SelectedNode.Data.Records.First(r => s.SeriesName == (SelectedNode.Name + " - " + r.Item_Name)).ToChartSeries());
        }

        private void ConfigureChartOutliers()
        {
            var visibleRecords = ChartDropdown.SelectedItems.Select(i => SeriesData.First(s => s.Item1.Idx == i.Idx).Item2);
            var actualsRecords = visibleRecords.Where(r => r.Item_Type.Contains("ACTUALS"));


            var visibleOutlierSeries = actualsRecords.SelectMany(r => r.Properties.Where(p => p.IsDisplayed && p.Value2 != null).Select(p => new Tuple<string, bool>(p.ColumnCode, p.Value2.ToLower() == "true")));
            var columnGroups = visibleOutlierSeries.GroupBy(o => o.Item1);
            var outlierSettings = columnGroups.Select(g => new Tuple<string, bool>(g.Key, g.Any(v => v.Item2)));
            outlierSettings.Do(o => ChartVM.UpdateOutliers(o.Item1, o.Item2));
        }

        private ObservableCollection<ChartModel.SingleSeries> GetSeriesToShow()
        {
            return new ObservableCollection<ChartModel.SingleSeries>(ChartDropdown.SelectedItems.Select(i => new ChartModel.SingleSeries { SeriesName = i.Name, SeriesType = "Line", Datapoints = SeriesData.First(s => s.Item1.Idx == i.Idx).Item2.ToChartSeries() }));
        }

        private ChartViewModel _chartVM = new ChartViewModel();
        public ChartViewModel ChartVM
        {
            get { return _chartVM; }
            set
            {
                _chartVM = value;
                PropertyChanged.Raise(this, "ChartVM");
            }
        }

        private MultiSelectViewModel _chartDropdown = new MultiSelectViewModel();
        public MultiSelectViewModel ChartDropdown
        {
            get { return _chartDropdown; }
            set
            {
                PropertyChanged.Raise(this, "ChartDropdown");
            }
        }

        public IEnumerable<Tuple<ComboboxItem, Record>> SeriesData { get; set; }

        /* Core chart properties */
        private ChartModel.Chart GetBaseChart()
        {
            return new ChartModel.Chart
            {
                ChartType = "Categorical",
                XAxisTitle = "Periods",
                YAxisTitle = "Volume",
                XAxisType = "Categorical",
                YAxisType = "Linear"
            };
        }

        #endregion

        #region Initial Aggregation

        /* Live aggregation is handled by each node/grid independantly.
         * But when building the tree for the first time, the VM takes control and pushes values from the leafs up the tree.
         */

        private void InitialAggregation(TreeGridNode fullTree)
        {
            var leafs = fullTree.GetLeafs();

            AggregateUpOneLevel(leafs.Where(l => l.Parent != null).Select(l => (TreeGridNode)l.Parent));
        }

        /* AggregateUpOneLevel works telling each parent to recalculate its property values. */
        //Aggregate up for all records and properties
        private void AggregateUpOneLevel(IEnumerable<TreeGridNode> nodes)
        {
            nodes.Do(node =>
            {
                node.Data.Records.Do(record =>
                {
                    record.Properties.Do(p => node.Recalcuate(record.Item_Idx, p.ColumnCode, true));
                    record.Properties.Do(p => node.RecalcuateComments(record.Item_Idx, p.ColumnCode));
                });
            });

            //Get the next set of parents to recalculate
            var newLevel = nodes.Where(n => n.Parent != null).Select(n => (TreeGridNode)n.Parent).Distinct();

            if (newLevel.Any())
                AggregateUpOneLevel(newLevel);
        }

        //Aggregate up for one record only
        private void AggregateUpOneLevel(IEnumerable<TreeGridNode> nodes, string recordIdx)
        {
            if (!nodes.Any()) return;

            nodes.Do(node =>
            {
                var record = node.Data.GetRecord(recordIdx);

                record.Properties.Where(p => p.Value.IsNumeric()).Do(property => node.Recalcuate(recordIdx, property.ColumnCode, false));
            });

            var newLevel = nodes.Where(n => n.Parent != null).Select(n => (TreeGridNode)n.Parent).Distinct();

            if (newLevel.Any())
                AggregateUpOneLevel(newLevel, recordIdx);
        }

        //Aggregate up for one property only
        public void AggregateUpOneLevel(IEnumerable<TreeGridNode> nodes, string recordIdx, string columnCode)
        {
            if (!nodes.Any()) return;

            nodes.Do(node => node.Recalcuate(recordIdx, columnCode, false));

            var newLevel = nodes.Where(n => n.Parent != null).Select(n => (TreeGridNode)n.Parent).Distinct();

            if (newLevel.Any())
                AggregateUpOneLevel(newLevel, recordIdx, columnCode);
        }

        #endregion

        #region Column Names

        private string _mainColumn;
        public string MainColumn
        {
            get
            {
                return _mainColumn;
            }
            set
            {
                _mainColumn = value;
                PropertyChanged.Raise(this, "MainColumn");
            }
        }

        private string _secondColumn;
        public string SecondColumn
        {
            get
            {
                return _secondColumn;
            }
            set
            {
                _secondColumn = value;
                PropertyChanged.Raise(this, "SecondColumn");
            }
        }
        #endregion

        #region Loading Properties 

        public bool NoData
        {
            get
            {
                return VisibleNodes == null || !VisibleNodes.Any();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                ChartVM.IsLoading = value;
                _isLoading = value;
                PropertyChanged.Raise(this, "IsLoading");
                PropertyChanged.Raise(this, "NoDataMessage");
                PropertyChanged.Raise(this, "HasData");
            }
        }

        public bool NoDataMessage { get { return NoData && !IsLoading; } }

        #endregion

        #region Extracting Data 

        private void NotifyChanges()
        {
            PropertyChanged.Raise(this, "HasChanges");
        }

        private void NotifyCommentsChanged()
        {
            PropertyChanged.Raise(this, "HasCommentChanges");
        }



        public bool HasChanges()
        {
            return VisibleNodes != null
                && VisibleNodes.Any(n => n.GetLeafs().Any(l => l.HasChanges));
        }

        public bool HasCommentChanges()
        {
            return VisibleNodes != null
                && VisibleNodes.Any(n => n.GetLeafs().Any(l => l.HasCommentChanges));
        }

        public bool HasValue2Changes()
        {
            return VisibleNodes != null
                && VisibleNodes.Any(n => n.GetLeafs().Any(l => l.HasValue2Changes));
        }

        public IEnumerable<Tuple<string, Record>> GetLowestLevelChanges()
        {
            var changedRecords = new List<Tuple<string, Record>>();

            VisibleNodes.Do(n =>
            {
                /* Gets all the treeNode Idx's and Record pairing where the data has changed */
                changedRecords.AddRange(n.GetLeafs().Where(l => l.HasChanges).SelectMany(l => l.Data.Records.Where(r => r.Properties.Any(p => p.HasChanged && p.IsEditable)).Select(r => new Tuple<string, Record>(l.Idx, r))));
            });

            return changedRecords;
        }

        public IEnumerable<Tuple<string, Record>> GetLowestLevelValueOrCommentChanges()
        {
            var changedRecords = new List<Tuple<string, Record>>();

            VisibleNodes.Do(n =>
            {
                /* Gets all the treeNode Idx's and Record pairing where the data has changed */
                changedRecords.AddRange(n.GetLeafs().Where(l => l.HasChanges || l.HasCommentChanges).SelectMany(l => l.Data.Records.Where(r => r.Properties.Any(p => (p.HasChanged || p.CommentsChanged) && p.IsEditable)).Select(r => new Tuple<string, Record>(l.Idx, r))));
            });

            return changedRecords;
        }

        public IEnumerable<Tuple<string, Record>> GetLowestLevelValueOrValue2Changes()
        {
            var changedRecords = new List<Tuple<string, Record>>();

            VisibleNodes.Do(n =>
            {
                /* Gets all the treeNode Idx's and Record pairing where the data has changed */
                changedRecords.AddRange(n.GetLeafs().Where(l => l.HasChanges || l.HasValue2Changes).SelectMany(l => l.Data.Records.Where(r => r.Properties.Any(p => p.HasChanged || p.HasValue2Changed)).Select(r => new Tuple<string, Record>(l.Idx, r))));
            });

            return changedRecords;
        }

        //Same as GetLowestLevelChanges but just all leafs, changed or not
        public IEnumerable<Tuple<string, Record>> GetLowestLevelNodes()
        {
            var changedRecords = new List<Tuple<string, Record>>();

            VisibleNodes.Do(n =>
            {
                /* Gets all the treeNode Idx's and Record pairing where the data has changed */
                changedRecords.AddRange(n.GetLeafs().SelectMany(l => l.Data.Records.Where(r => r.Properties.Any(p => p.IsEditable)).Select(r => new Tuple<string, Record>(l.Idx, r))));
            });

            return changedRecords;
        }

        public XElement GetSaveXml()
        {
            var changedValues = GetLowestLevelChanges();

            var data = new XElement("Data");

            changedValues.Do(c =>
            {
                var row = new XElement("Row");
                row.SetAttributeValue("Parent_Idx", c.Item1);
                row.SetAttributeValue("Idx", c.Item2.Item_Idx);

                var columns = new XElement("Columns");
                c.Item2.Properties.Do(p =>
                {
                    var column = new XElement("Column");
                    column.SetAttributeValue("Idx", p.IDX);
                    column.SetAttributeValue("Value_New", RecordViewModel.FixValue(p.Value));
                    column.SetAttributeValue("Value_Original", RecordViewModel.FixValue(p.OriginalValue));
                    columns.Add(column);
                });
                row.Add(columns);
                data.Add(row);
            });

            return data;
        }



        #endregion

        #region Property Change Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region ViewModes

        private SingleSelectViewModel _viewModesVM;
        public SingleSelectViewModel ViewModesVM
        {
            get
            {
                if (_viewModesVM == null)
                    SetCoreViewModes();
                return _viewModesVM;
            }
        }

        private SingleSelectViewModel _visibleSingles = new SingleSelectViewModel();
        public SingleSelectViewModel VisibleSingles
        {
            get
            {
                return _visibleSingles;
            }
        }

        private void SetCoreViewModes()
        {
            VisibleSingles.PropertyChanged += VisibleSingles_PropertyChanged;

            var hier = new ComboboxItem { Idx = "0", Name = "Hierarchical", IsSelected = false, IsEnabled = true };
            var leaf = new ComboboxItem { Idx = "1", Name = "Leaf", IsSelected = false, IsEnabled = true };
            var single = new ComboboxItem { Idx = "2", Name = "Single", IsSelected = true, IsEnabled = true };

            _viewModesVM = new SingleSelectViewModel();
            _viewModesVM.PropertyChanged += ViewMode_PropertyChanged;
            _viewModesVM.SetItems(new List<ComboboxItem> { hier, leaf, single });
        }

        private void ViewMode_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                if (LoadedNodes != null && LoadedNodes.Any())
                    switch (ViewModesVM.SelectedItem.Name)
                    {
                        case "Hierarchical":
                            VisibleNodes = LoadedNodes;
                            IsSingleMode = false;
                            break;
                        case "Leaf":
                            VisibleNodes = LoadedNodes.SelectMany(l => l.GetLeafs()).ToList();
                            IsSingleMode = false;
                            break;
                        case "Single":
                            var allNodesAsComboItems = LoadedNodes.SelectMany(n => TreeViewHierarchy.GetFlatTree(n)).Select(l => new ComboboxItem { Idx = l.Idx + "$" + l.Name, Name = l.Name, IsEnabled = true, Colour = (l.Children != null && l.Children.Any()) ? User.CurrentUser.Accent : null }).ToList();
                            allNodesAsComboItems.First().IsSelected = true;
                            IsSingleMode = true;
                            VisibleSingles.SetItems(allNodesAsComboItems);
                            break;
                    }
            }
        }

        private void VisibleSingles_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                VisibleNodes = new List<TreeGridNode> { LoadedNodes.SelectMany(l => TreeViewHierarchy.GetFlatTree(l)).First(l => l.Idx + "$" + l.Name == VisibleSingles.SelectedItem.Idx) };
            }
        }

        private bool _isSingleMode;
        public bool IsSingleMode
        {
            get { return _isSingleMode; }
            set
            {
                _isSingleMode = value;
                PropertyChanged.Raise(this, "IsSingleMode");
            }
        }

        #endregion

        #region Save

        /* Since save often includes data other than just the TreeGrid, set the SaveCommand in the VM of the screen you are using the control 
         * We have the command here so we can easily bind to the save button in the control.
         */
        public ICommand SaveCommand { get; set; }

        #endregion
    }
}