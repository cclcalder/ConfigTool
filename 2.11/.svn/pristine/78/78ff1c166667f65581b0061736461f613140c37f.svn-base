using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Model.Entity.Listings;
using Model.Entity.Generic;
using AutoMapper;
using Model.DataAccess.Listings;

namespace Exceedra.TreeGrid.Models
{
    public class TreeGridNode : TreeViewHierarchy
    {
        public TreeGridNode() { }

        public TreeGridNode(XElement xml)
        {
            ParentIdx = xml.Element("ParentIdx").MaybeValue() ?? xml.Element("Parent_Idx").MaybeValue();
            Data = ConvertXmlToGrid(xml.Element("Rows"));
            Idx = "-1";
            AdditionalIdx = xml.Element("Seasonal_Idx").MaybeValue();
        }

        /* Cheat property so I can access the SeasonalIdx  */
        private string _additionalIdx;
        public string AdditionalIdx { get { return (_additionalIdx == "-1" || _additionalIdx == null) ? "1" : _additionalIdx; } set { _additionalIdx = value; } }

        /* Set of Idxs indicating properties to dissagregate on. Lower index = Higher priority */
        public List<string> DisaggregationIdxs { get; set; }

        public double RowHeight { get; set; }

        public RecordViewModel Measures { get; set; }

        public RecordViewModel Sums { get; set; }

        private RecordViewModel _data;

        public RecordViewModel Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                BuildLookup();
            }
        }

        public void SetChangeEvent()
        {
            Data.Records.SelectMany(r => r.Properties).Do(p =>
            {
                p.PropertyChanged += ValueChange;
                p.PropertyChanged += HasChangesEvent;
                p.PropertyChanged += CommentChange;
            });

            /* Special cases for when non-editable properties can change, e.g. total volume planning measure */
            Data.Records.Where(r => r.Item_Name == "Total Volume").SelectMany(r => r.Properties).Where(p => !p.IsEditable).Do(p =>
            {
                p.PropertyChanged += UpdateSeries;
            });
        }

        public void UnSetChangeEvent()
        {
            Data.Records.SelectMany(r => r.Properties).Where(p => p.IsEditable).Do(p =>
            {
                p.PropertyChanged -= ValueChange;
                p.PropertyChanged -= HasChangesEvent;
                p.PropertyChanged -= CommentChange;
            });

            Data.Records.Where(r => r.Item_Name == "Total Volume").SelectMany(r => r.Properties).Where(p => !p.IsEditable).Do(p =>
            {
                p.PropertyChanged -= UpdateSeries;
            });
        }

        private void UpdateSeries(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                FireRecordChanged(sender);
            }
        }
        private void ValueChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "NonInternalValue")
            {
                var recordIdx = Data.Records.First(r => r.Properties.Contains((Property)sender)).Item_Idx;
                if (Parent != null)
                    ((TreeGridNode)Parent).Recalcuate(recordIdx, ((Property)sender).ColumnCode, false);

                Disaggregate(recordIdx, ((Property)sender).ColumnCode, DisaggregationIdxs.Except(recordIdx).ToList());

                //We already know it's not locked so check if we can push this value to other grids.
                if (!((Property)sender).LeftRightLocked && ((Property)sender).IsDisplayed)
                    FirePropertyChanged(sender);

            }
            else if (e.PropertyName == "Value" || e.PropertyName == "Value2")
            {
                FireRecordChanged(sender);

                if (!((Property)sender).LeftRightLocked && ((Property)sender).IsDisplayed)
                    FireCalculatedChanged(sender);
            }
            else if (e.PropertyName == "SelectedItem" && !((Property)sender).Locked)
            {
                if(Children != null && Children.Any())
                {
                    var recordIdx = Data.Records.First(r => r.Properties.Contains((Property)sender)).Item_Idx;
                    Children.Select(c => ((TreeGridNode)c)).Do(c =>
                    {
                        var childProperty = c.Data.GetProperty(recordIdx, ((Property)sender).ColumnCode);
                        childProperty.SelectedItem = childProperty.Values.First(v => v.Item_Idx == ((Property)sender).SelectedItem.Item_Idx);
                    });
                }

                //FireSelectedItemChanged(sender);
            }
        }



        private void CommentChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "NonInternalCommentsChange")
            {
                var recordIdx = Data.Records.First(r => r.Properties.Contains((Property)sender)).Item_Idx;
                if (Parent != null)
                    ((TreeGridNode)Parent).AggregateComments(recordIdx, ((Property)sender).ColumnCode);

                DisaggregateComments(recordIdx, ((Property)sender).ColumnCode);
            }
        }

        private void HasChangesEvent(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HasChanged")
            {
                FireIveChanged(sender);
            }
            else if (e.PropertyName == "HasCommentChanged")
            {

            }
        }

        public bool HasChanges { get { return Data.Records.Any(r => r.Properties.Any(p => p.HasChanged)); } }

        public bool HasCommentChanges { get { return Data.Records.Any(r => r.Properties.Any(p => p.CommentsChanged)); } }

        public bool HasValue2Changes { get { return Data.Records.Any(r => r.Properties.Any(p => p.HasValue2Changed)); } }

        /* Fire this event when record values change */
        public delegate void Change();
        public event Change IveChanged;

        /* Fire this event when record values change */
        public delegate void CommentsChange();
        public event CommentsChange CommentChanged;

        /* Fire this event when the selected Combobox Item changes */
        public delegate void SelectedChange();
        public event SelectedChange SelectedItemChange;

        /* Fire this event when a record values change so we can update the chart */
        public delegate void SeriesChange(Tuple<string, TreeGridNode> changedRecord);
        public event SeriesChange RecordChanged;

        /* Fire this event when a record values change so we can update the chart */
        public delegate void PropertyChange(Tuple<Tuple<string, string>, TreeGridNode> changedRecord);
        public event PropertyChange ValueChanged;

        /* Fire this event when a property changes so we can update any calculated measures and the totals measure (if there is one)
         * Basically is used when we have some custom code for an instance of the TreeGrid. 
         * Planning - Update totals measure
         * Forecasting - Push change from forecast -> baseline and vice versa.
         */
        public delegate void CalculatedChange(Tuple<Tuple<string, string>, TreeGridNode> changedRecord);
        public event CalculatedChange CalculatedChanged;

        private void FireIveChanged(object sender)
        {
            if (IveChanged != null)
                IveChanged();
        }

        private void FireCommentChanged(object sender)
        {
            if (CommentChanged != null)
                CommentChanged();
        }

        //private void FireSelectedItemChanged(object sender)
        //{
        //    if (SelectedItemChange != null)
        //        SelectedItemChange();
        //}

        private void FireRecordChanged(object sender)
        {
            var changedRecord = Data.Records.First(r => r.Properties.Contains((Property)sender));
            if (!changedRecord.Item_IsDisplayed) return;
            var recordIdx = changedRecord.Item_Idx;
            if (RecordChanged != null)
                RecordChanged(new Tuple<string, TreeGridNode>(recordIdx, this));
        }

        private void FirePropertyChanged(object sender)
        {
            var recordIdx = Data.Records.First(r => r.Properties.Contains((Property)sender)).Item_Idx;
            var columnCode = ((Property)sender).ColumnCode;
            if (ValueChanged != null)
                ValueChanged(new Tuple<Tuple<string, string>, TreeGridNode>(new Tuple<string, string>(recordIdx, columnCode), this));
        }

        private void FireCalculatedChanged(object sender)
        {
            var recordIdx = Data.Records.First(r => r.Properties.Contains((Property)sender)).Item_Idx;
            var columnCode = ((Property)sender).ColumnCode;
            if (CalculatedChanged != null)
                CalculatedChanged(new Tuple<Tuple<string, string>, TreeGridNode>(new Tuple<string, string>(recordIdx, columnCode), this));
        }

        /* i.e. aggragate */
        //initialSetup will set the OriginalValue so not to indicate any changes have happend.
        public void Recalcuate(string recordIdx, string columnCode, bool initialSetup)
        {
            var property = Data.GetProperty(recordIdx, columnCode);
            property.Locked = true;

            double newValue = 0;
            var childValues = Children.Select(c => ((TreeGridNode)c).CellLookUp[recordIdx + "$" + columnCode].Value.AsNumericDouble());

            if (childValues.All(v => v == -1) && Data.IsRecordOverride(recordIdx))
                newValue = -1;
            else
                childValues.Do(v =>
                {
                    if (v == -1 && Data.IsRecordOverride(recordIdx))
                        newValue += 0;
                    else
                        newValue += v;
                });

            if (Data.GetRecord(recordIdx).Item_AggrType == "AVG")
                newValue = newValue / childValues.Count();

            var newString = newValue.ToString();
            if (newValue == 0 && Children.All(c => !((TreeGridNode)c).CellLookUp[recordIdx + "$" + columnCode].Value.IsNumeric()))
                newString = "-";

            property.Value = initialSetup ? (property.OriginalValue = newString) : newString;

            if (Parent != null)
                ((TreeGridNode)Parent).Recalcuate(recordIdx, columnCode, initialSetup);

            property.Locked = false;
        }

        public void RecalcuateComments(string recordIdx, string columnCode)
        {
            var property = Data.GetProperty(recordIdx, columnCode);

            var childProperties = Children.Select(c => ((TreeGridNode)c).CellLookUp[recordIdx + "$" + columnCode]).Where(c => c.HasComment);

            childProperties.Do(v =>
                {
                    var newComments = v.CommentList.Distinct(c => c.Idx).Where(c => !property.CommentList.Any(pc => pc.Idx == c.Idx));

                    property.CommentList.AddRange(newComments);
                    property.AssertCommentState();
                });

            if (Parent != null)
                ((TreeGridNode)Parent).RecalcuateComments(recordIdx, columnCode);

        }

        /* If no recordIdx then recalc all */
        public void RecalcuateSums()
        {
            Sums.Records.Do(r => r.GetProperty("0").SetValue(Data.GetRecord(r.Item_Idx).Properties.Where(p => p.Value.IsNumeric() && (!Data.IsRecordOverride(r.Item_Idx) || p.Value != "-1")).Sum(p => p.Value.AsNumericInt()).ToString()));
        }

        public void RecalcuateSums(string recordIdx)
        {
            Sums.GetProperty(recordIdx, "0").SetValue(Data.GetRecord(recordIdx).Properties.Where(p => p.Value.IsNumeric() && (!Data.IsRecordOverride(recordIdx) || p.Value != "-1")).Sum(p => p.Value.AsNumericInt()).ToString());
        }

        public void AggregateComments(string recordIdx, string columnCode)
        {
            var property = Data.GetProperty(recordIdx, columnCode);

            var newValue = new List<ListboxComment>();
            Children.Select(c => ((TreeGridNode)c).Data.GetProperty(recordIdx, columnCode)).Do(p => newValue.AddRange(p.CommentList));

            property.CommentsLocked = true;
            property.CommentList = newValue.Distinct(c => c.Idx).ToList();
            property.CommentsLocked = false;

            if (Parent != null)
                ((TreeGridNode)Parent).AggregateComments(recordIdx, columnCode);
        }

        public void DisaggregateComments(string recordIdx, string columnCode)
        {
            var propertyLastComment = Data.GetProperty(recordIdx, columnCode).CommentList.Last();

            Children.Select(c => ((TreeGridNode)c).Data.GetProperty(recordIdx, columnCode)).Do(p =>
            {
                p.CommentsLocked = true;
                p.CommentList.Add(propertyLastComment);
                p.AssertCommentState();
                p.CommentsLocked = false;
            });

            Children.Do(c => ((TreeGridNode)c).DisaggregateComments(recordIdx, columnCode));
        }

        /* When disaggregating, we give the first eligable child all excess value when the user does not input a completely divisible number */
        public void Disaggregate(string recordIdx, string columnCode, List<string> dissagregationIdxs)
        {
            if (Children == null || !Children.Any()) return;

            var newValuePairs = new List<Tuple<double, Property>>();

            var property = CellLookUp[recordIdx + "$" + columnCode]; //Data.GetProperty(recordIdx, columnCode);
            var valueToSpread = property.Value.AsNumericInt();

            var childProperties = Children.Select(c => ((TreeGridNode)c).CellLookUp[recordIdx + "$" + columnCode]).ToList();   //Data.GetProperty(recordIdx, columnCode)).ToList();
            childProperties.Do(c => c.Locked = true);

            int remainingValue = 0;

            if (Data.GetRecord(recordIdx).Item_AggrType == "AVG")
            {
                childProperties.Do(c => c.SetValue(valueToSpread.ToString()));
            }
            else
            {
                if (valueToSpread == -1 && Data.IsRecordOverride(recordIdx))
                {
                    childProperties.Do(c => c.SetValue("-1"));
                }
                else
                {
                    /* Get the values you want to use to determine how to split the total values */
                    var dissagregationProperties = (dissagregationIdxs == null || !dissagregationIdxs.Any()) ? null : Children.Select(c => ((TreeGridNode)c).GetProperty(dissagregationIdxs, columnCode)).ToList();

                    /* If we got no valid properties OR they are all null or not a number OR they are all 0 (Planning: Will only be 0 if override = -1) */
                    if (dissagregationProperties == null || dissagregationProperties.All(p => p == null || !p.Value.IsNumeric()) || dissagregationProperties.All(p => p != null && p.Value == "0"))
                    {
                        int evenSpreadValue = valueToSpread / childProperties.Count();
                        remainingValue = valueToSpread % childProperties.Count();
                        for (int i = 0; i < childProperties.Count(); i++)
                        {
                            newValuePairs.Add(new Tuple<double, Property>(evenSpreadValue, childProperties[i]));
                        }
                    }
                    //Special case where we some are null and some are 0. So we do an even split across the 0's
                    else if (dissagregationProperties.All(p => p == null || p.Value == "0"))
                    {
                        var numOfZeros = dissagregationProperties.Count(d => d != null && d.Value == "0");
                        int evenSpreadValue = valueToSpread / numOfZeros;
                        remainingValue = valueToSpread % numOfZeros;
                        for (int i = 0; i < childProperties.Count(); i++)
                        {
                            if (dissagregationProperties[i] != null && dissagregationProperties[i].Value.IsNumeric())
                            {
                                newValuePairs.Add(new Tuple<double, Property>(evenSpreadValue, childProperties[i]));
                            }
                            else
                            {
                                newValuePairs.Add(new Tuple<double, Property>(0, childProperties[i]));
                            }
                        }
                    }
                    //Otherwise we split proportionality across the children with values. 
                    else
                    {
                        //Total value of our dissagregation properties. What we use to determine each childs share of the value.
                        var totalValue = dissagregationProperties.Where(p => p != null && p.Value.IsNumeric()).Select(p => p.Value.AsNumericAbsInt()).Sum();

                        //Maintain the amount we have apportioned to the children so when we are done we can add any unapportioned value to the first valid child.
                        int apportionedAmount = 0;

                        for (int i = 0; i < childProperties.Count(); i++)
                        {
                            if (dissagregationProperties[i] != null && dissagregationProperties[i].Value.IsNumeric())
                            {
                                int propertyProportion = (int)Math.Floor(dissagregationProperties[i].Value.AsNumericDouble() / totalValue * valueToSpread);
                                apportionedAmount += propertyProportion;
                                newValuePairs.Add(new Tuple<double, Property>(propertyProportion, childProperties[i]));
                            }
                            else
                            {
                                newValuePairs.Add(new Tuple<double, Property>(0, childProperties[i]));
                            }
                        }

                        remainingValue = valueToSpread - apportionedAmount;

                        if (remainingValue < 0)
                        {
                            //ERROR!!!!!
                        }
                    }

                    newValuePairs.Do(p =>
                    {
                        if (p.Item1 > 0 && remainingValue != 0)
                        {
                            p.Item2.SetValue((p.Item1 + remainingValue).ToString());
                            remainingValue = 0;
                        }
                        else
                            p.Item2.SetValue(p.Item1.ToString());
                    });

                    //In the edge case where the children are all 0 and our value to split is less than the numer of children, we give all the value to the first child.
                    if (remainingValue != 0 && newValuePairs.All(v => v.Item1 == 0))
                        newValuePairs.First().Item2.SetValue(remainingValue.ToString());
                }
            }

            Children.Do(c => ((TreeGridNode)c).Disaggregate(recordIdx, columnCode, dissagregationIdxs));
            childProperties.Do(c => c.Locked = false);
        }


        public static TreeGridNode ConvertListToTree(List<TreeGridNode> list)
        {
            var hashedList = list.ToLookup(l => l.ParentIdx);
            list.ForEach(item =>
            {
                item.Children.Clear();
                item.Children.AddRange(hashedList[item.Idx].ToList());
                item.Children.Do(c => c.Parent = item);

            });

            var rootNode = list.First(t => string.IsNullOrEmpty(t.ParentIdx)) ?? list.First(t => t.Parent == null);
            return rootNode;
        }

        public IEnumerable<TreeGridNode> GetLeafs()
        {
            return GetFlatTree(this).Where(n => !n.HasChildren);
        }

        internal static RecordViewModel ConvertXmlToGrid(XElement xml)
        {
            var grid = RecordViewModel.LoadWithNewXml(xml);
            if (grid.Records.Any(r => r.Item_Name == "New Forecast"))
            {
                var actuals = grid.Records.First(r => r.Item_Name.Contains("Actuals"));
                var newForecast = grid.Records.First(r => r.Item_Name == "New Forecast");
                actuals.Properties.Where(p => p.ControlType.ToLower() == "treegridcell" && p.Value == "-").Do(p =>
                {
                    var newFc = newForecast.Properties.First(prop => prop.ColumnCode == p.ColumnCode);
                    p.IsEditable = newFc.IsEditable;
                    p.Value = p.OriginalValue = newFc.Value;
                    p.BackgroundColour = newFc.BackgroundColour;
                    p.StringFormat = "N0";
                });
                newForecast.Item_IsDisplayed = false;
            }

            return grid;
        }

        private void BuildLookup()
        {
            CellLookUp = Data.Records.Select(r => r.Properties.ToDictionary(p => r.Item_Idx + "$" + p.ColumnCode)).SelectMany(d => d).ToDictionary(l => l.Key, l => l.Value);
        }

        //Key = recordIdx$ColumnCode
        public Dictionary<string, Property> CellLookUp { get; set; }

        private Property GetProperty(List<string> dissagregationIdxs, string columnCode)
        {
            Property property = null;
            foreach (var idx in dissagregationIdxs)
            {
                property = CellLookUp[idx + "$" + columnCode];

                if (property == null) continue;

                if (property.Value.IsNumeric())
                {
                    var isOverride = Data.GetRecord(idx).Item_Name.ToLower().Contains("override");
                    if (isOverride && property.Value.AsNumericInt() == -1)
                        return new Property { Value = "0" };
                    else if (isOverride && property.Value.AsNumericInt() == 0) continue;
                    return property;
                }
            }
            return null;
        }


        public static TreeGridNode GetProductsAsTreeGridNodes()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TreeViewHierarchy, TreeGridNode>());
            var mapper = config.CreateMapper();

            var skus = ListingsAccess.GetFilterProducts(false, true).Result.FlatTree;

            var skusAsTreeGrid = skus.Select(n => mapper.Map<TreeGridNode>(n));

            return ConvertListToTree(skusAsTreeGrid.ToList());
        }

        public static TreeGridNode CloneTreeGridNode(TreeGridNode node)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TreeViewHierarchy, TreeGridNode>());
            var mapper = config.CreateMapper();

            var nodes = node.FlatTree;
            var clonedList = nodes.Select(n => mapper.Map<TreeGridNode>(n));

            return ConvertListToTree(clonedList.ToList());
        }
    }
}