//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Xml.Linq;
//using Exceedra.Common.Utilities;
//using Model;

//namespace Exceedra.Controls.DynamicGrid.Models
//{
//    public class SimpleList : INotifyPropertyChanged 

//    {

//        //public int Key { get; set; }
//        //public string Value { get; set; }

//        public string Item_Idx { get; set; }
//        public string Item_Name { get; set; }
//        private bool _IsSelected;
//        public bool IsSelected
//        {
//            get
//            {
//                return _IsSelected;
//            }
//            set
//            {
//                _IsSelected = value;
//                PropertyChanged.Raise(this, "IsSelected");
//            }
//        }


//        public static ObservableCollection<SimpleList> GetFromXML(XElement attr)
//        {

//            var SimpleLists = new List<SimpleList>();

//            if (attr != null)
//            {
//                SimpleLists.AddRange(attr.Elements("Item").Select(att => new SimpleList
//                {
//                    Item_Idx = att.Element("Item_Idx").Value,
//                    Item_Name = att.Element("Item_Name").Value,
//                    IsSelected = (att.Element("IsSelected").Value == "1" ? true : false)
//                }));

//                SimpleLists.AddRange(attr.Elements("Value").Select(att => new SimpleList
//                {
//                    Item_Idx = att.Element("Item_Idx").Value,
//                    Item_Name = att.Element("Item_Name").Value,
//                    IsSelected = (att.Element("IsSelected").Value == "1" ? true : false)
//                }));

//                SimpleLists.AddRange(attr.Elements("Dropdown").Select(att => new SimpleList
//                {
//                    Item_Idx = att.Element("Item_Idx").Value,
//                    Item_Name = att.Element("Item_Name").Value,
//                    IsSelected = (att.Element("IsSelected").Value == "1" ? true : false)
//                }));
//            }
//            else
//            {
//                //check for static data
//                return null;

//            }


//            return new ObservableCollection<SimpleList>(SimpleLists);
//        }

//        public static ObservableCollection<SimpleList> Get(XElement attr, string datasource)
//        {
//            if (attr == null)
//                return null;

//            var SimpleLists = new List<SimpleList>();

//            if (attr != null)
//            {
//                //we have some Data input, get dataset from WS
//                var argument = new XElement("GetItems"); ;

//                if (attr != null)
//                {
//                    // Expected XML Input:
//                    //<GetItems>
//                    //  <User_Idx>?</User_Idx>
//                    //  <MenuItem_Idx>?</MenuItem_Idx>
//                    //  <SelectedItem_Idx>?</SelectedItem_Idx>
//                    //</GetItems>

//                    argument.Add(new XElement("ColumnCode", Helpers.xml.FixNullInline(attr.Element("ColumnCode"), "")));
//                    argument.Add(new XElement("User_Idx", Helpers.xml.FixNullInline(attr.Element("User_Idx"), "")));

//                    var needsSelected = new string[] { StoredProcedure.Admin.GetItemStatuses, StoredProcedure.Admin.GetProfiles, StoredProcedure.Admin.GetRobAppTypes, StoredProcedure.Admin.GetRoles };

//                    if (needsSelected.Contains(datasource))
//                    {
//                        argument.Add(new XElement("SelectedItem_Idx", Helpers.xml.FixNullInline(attr.Element("SelectedItem_Idx"), "")));
//                    }

//                    var ParentItems = new XElement("ParentItems");

//                    var c = (attr.Elements("Input").Elements().Count() > 0);
//                    if (c)
//                    {
//                        foreach (var x in attr.Elements("Input"))
//                        {
//                            var item = new XElement("Item");
//                            item.Add(new XElement("ColumnCode", x.Element("ColumnCode").Value));

//                            var values = new XElement("Values");

//                            var ValuesXML = x.Element("Values");

//                            foreach (var i in ValuesXML.Elements("SelectedItem_Idx"))
//                            {
//                                values.Add(new XElement("SelectedItem", i.Value));
//                            }
//                            item.Add(values);

//                            ParentItems.Add(item);
//                        }
//                    }

//                    argument.Add(ParentItems);

//                }

//                var res = Model.DataAccess.@WebServiceProxy.Call(datasource, argument.ToString());
//                //Expected XML Output:
//                //<Results>
//                //  <Item>
//                //    <Item_Idx>?</Item_Idx>
//                //    <Item_Name>?</Item_Name>
//                //    <IsSelected>0 or 1</IsSelected>
//                //  </Item>
//                //</Results>
//                SimpleLists.AddRange(res.Elements("Results").Select(att => new SimpleList
//                {
//                    Item_Idx = att.Element("Item_Idx").Value,
//                    Item_Name = att.Element("Item_Name").Value,
//                    IsSelected = (att.Element("IsSelected").Value == "1" ? true : false)
//                }));
//            }
//            else
//            {
//                //check for static data


//            }


//            return new ObservableCollection<SimpleList>(SimpleLists);
//        }

//        public static ObservableCollection<SimpleList> GetFromXML(string attr, string datasource)
//        {

//            var SimpleLists = new List<SimpleList>();

//            if (attr != null)
//            {
//                //we have some Data input, get dataset from WS 
//                var argument = XElement.Parse(attr.Replace("&gt;", ">").Replace("&lt;", "<"));
//                // Expected XML Input:
//                //<GetItems>
//                //  <User_Idx>?</User_Idx>
//                //  <MenuItem_Idx>?</MenuItem_Idx>
//                //  <SelectedItem_Idx>?</SelectedItem_Idx>
//                //</GetItems> 

//                var res = Model.DataAccess.@WebServiceProxy.Call(datasource, argument.ToString());
//                //Expected XML Output:
//                //<Results>
//                //  <Item>
//                //    <Item_Idx>?</Item_Idx>
//                //    <Item_Name>?</Item_Name>
//                //    <IsSelected>0 or 1</IsSelected>
//                //  </Item>
//                //</Results>
//                SimpleLists.AddRange(res.Elements("Item").Select(att => new SimpleList
//                {
//                    Item_Idx = att.Element("Item_Idx").Value,
//                    Item_Name = att.Element("Item_Name").Value,
//                    IsSelected = (att.Element("IsSelected").Value == "1" ? true : false)
//                }));

//                SimpleLists.AddRange(res.Elements("Value").Select(att => new SimpleList
//                {
//                    Item_Idx = att.Element("Item_Idx").Value,
//                    Item_Name = att.Element("Item_Name").Value,
//                    IsSelected = (att.Element("IsSelected").Value == "1" ? true : false)
//                }));

//                SimpleLists.AddRange(res.Elements("Dropdown").Select(att => new SimpleList
//                {
//                    Item_Idx = att.Element("Item_Idx").Value,
//                    Item_Name = att.Element("Item_Name").Value,
//                    IsSelected = (att.Element("IsSelected").Value == "1" ? true : false)
//                }));
//            }
//            else
//            {
//                //check for static data
//                return null;

//            }


//            return new ObservableCollection<SimpleList>(SimpleLists);
//        }

//        internal static List<int> GetFilters(IEnumerable<XElement> attr)
//        {
//            return (from x in attr
//                    select Convert.ToInt32(x.Value)).ToList();

//        }

//        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;


//        //        public static ObservableCollection<SimpleList> Get(string p)
//        //        {

//        //            var x =  @"<Results>
//        //                            <Item>
//        //                              <Item_Idx>1</Item_Idx>
//        //                              <Item_Name>a</Item_Name>
//        //                              <Item_IsSelected>1</Item_IsSelected>
//        //                            </Item>
//        //                            <Item>
//        //                              <Item_Idx>2</Item_Idx>
//        //                              <Item_Name>b</Item_Name>
//        //                              <Item_IsSelected>1</Item_IsSelected>
//        //                            </Item>
//        //                            <Item>
//        //                              <Item_Idx>3</Item_Idx>
//        //                              <Item_Name>c</Item_Name>
//        //                              <Item_IsSelected>1</Item_IsSelected>
//        //                            </Item>
//        //                          </Results>";


//        //            var options = new List<Option>();
//        //            var res = XElement.Parse(x);

//        //            foreach (var att in res.Elements("Item"))
//        //            {
//        //                var o = new Option
//        //                {
//        //                    Item_Idx = att.Element("Item_Idx").Value,
//        //                    Item_Name = att.Element("Item_Name").Value,
//        //                    IsSelected = (att.Element("Item_IsSelected").Value == "1" ? true : false)
//        //                };

//        //                options.Add(o);
//        //            }

//        //            return new ObservableCollection<Option>(options);

//        //        }

//        //public class GetDropdownItems
//        //{ 
//        //   public string ColumnCode {get;set;}
//        //   public List<Item> ParentItems  {get;set;}
//        //}

//        //public class Item
//        //{
//        //   public string ColumnCode {get;set;}
//        //   public List<SelectedItem> Values { get; set; }
//        //}

  



       
//    }
//}
