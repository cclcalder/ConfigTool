using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Exceedra.Common;
using Model;
using EventHandlerEx = Exceedra.Common.Utilities.EventHandlerEx;


namespace Exceedra.DynamicGrid.Models
{
    /// <summary>
    /// This class is open for extension but closed for modification. It is used in both the vertical and horizontal dyanmic grids. - LB 25/02/2015
    /// </summary>
    [Serializable]
    public class Option : INotifyPropertyChanged
    {
        //<Option>
        //   <Value>7</Value>
        //   <Text>Coupon on Pack</Text>
        //   <IsSelected>0</IsSelected>
        //   <FilterAttribOptions>                     
        //     <OptionID>1</OptionID>
        //     <OptionID>3</OptionID>          
        //   </FilterAttribOptions>
        // </Option>

        public Option()
        {

        }
        private Option(XElement att, string idx = "Idx", string name = "Name", string selected = "IsSelected", string order = "SortOrder")
        {
            if (att == null) return;

            // ReSharper disable once PossibleNullReferenceException
            Item_Idx = att.Element(idx).MaybeValue() ?? att.Element("Item_Idx").MaybeValue() ?? (att.Name.LocalName.Equals("Value") ? att.Value : null);
            // ReSharper disable once PossibleNullReferenceException
            Item_Name = att.Element(name).MaybeValue() ?? att.Element("Item_Name").MaybeValue();
            // ReSharper disable once PossibleNullReferenceException
            IsSelected = att.Element(selected) != null && att.Element(selected).Value == "1";
            // ReSharper disable once PossibleNullReferenceException
            SortOrder = att.Element(order) != null ? int.Parse(att.Element(order).Value) : 0;
        }

        public int SortOrder { get; set; }

        [XmlText]
        public string Item_Idx { get; set; }
        public string Item_Name { get; set; }
        private bool _IsSelected;
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
                EventHandlerEx.Raise(PropertyChanged, this, "IsSelected");
            }
        }

        public string Background { get; set; }

        public static ObservableCollection<Option> GetFromXML(XElement attr)
        {
            var options = new List<Option>();

            if (attr != null)
            {
                options.AddRange
                    (
                    attr.Elements("Item")
                    .Concat(attr.Elements("Value"))
                    .Concat(attr.Elements("Dropdown"))
                    .Select(att => new Option(att))
                    );

                //options.AddRange(attr.Elements("Item").Select(att => new Option
                //{
                //    Item_Idx = att.Element("Item_Idx").Value,
                //    Item_Name = att.Element("Item_Name").Value,
                //    IsSelected = (att.Element("IsSelected").Value == "1" ? true : false)
                //}));

                //options.AddRange(attr.Elements("Value").Select(att => new Option
                //{
                //    Item_Idx = att.Element("Item_Idx").Value,
                //    Item_Name = att.Element("Item_Name").Value,
                //    IsSelected = (att.Element("IsSelected").Value == "1" ? true : false)
                //}));

                //options.AddRange(attr.Elements("Dropdown").Select(att => new Option
                //{
                //    Item_Idx = att.Element("Item_Idx").Value,
                //    Item_Name = att.Element("Item_Name").Value,
                //    IsSelected = (att.Element("IsSelected").Value == "1" ? true : false)
                //}));
            }
            else
            {
                //check for static data
                return null;

            }


            return new ObservableCollection<Option>(options);
        }

        //public static ObservableCollection<Option> Get(XElement attr, string datasource)
        //{
        //    if (attr == null)
        //        return null;

        //    var options = new List<Option>();

        //    if (attr != null)
        //    {
        //        //we have some Data input, get dataset from WS
        //        var argument = new XElement("GetItems"); ;

        //        if (attr != null)
        //        {
        //            // Expected XML Input:
        //            //<GetItems>
        //            //  <User_Idx>?</User_Idx>
        //            //  <MenuItem_Idx>?</MenuItem_Idx>
        //            //  <SelectedItem_Idx>?</SelectedItem_Idx>
        //            //</GetItems>

        //            argument.Add(new XElement("ColumnCode", Exceedra.Controls.Helpers.xml.FixNullInline(attr.Element("ColumnCode"), "")));
        //            argument.Add(new XElement("User_Idx", Exceedra.Controls.Helpers.xml.FixNullInline(attr.Element("User_Idx"), "")));

        //            var needsSelected = new string[] { StoredProcedure.Admin.GetItemStatuses, StoredProcedure.Admin.GetProfiles, StoredProcedure.Admin.GetRobAppTypes, StoredProcedure.Admin.GetRoles };

        //            if (needsSelected.Contains(datasource))
        //            {
        //                argument.Add(new XElement("SelectedItem_Idx", Exceedra.Controls.Helpers.xml.FixNullInline(attr.Element("SelectedItem_Idx"), "")));
        //            }

        //            var ParentItems = new XElement("ParentItems");

        //            var c = (attr.Elements("Input").Elements().Count() > 0);
        //            if (c)
        //            {
        //                foreach (var x in attr.Elements("Input"))
        //                {
        //                    var item = new XElement("Item");
        //                    item.Add(new XElement("ColumnCode", x.Element("ColumnCode").Value));

        //                    var values = new XElement("Values");

        //                    var ValuesXML = x.Element("Values");

        //                    foreach (var i in ValuesXML.Elements("SelectedItem_Idx"))
        //                    {
        //                        values.Add(new XElement("SelectedItem", i.Value));
        //                    }
        //                    item.Add(values);

        //                    ParentItems.Add(item);
        //                }
        //            }

        //            argument.Add(ParentItems);

        //        }

        //        var res = Model.DataAccess.@WebServiceProxy.Call(datasource, argument.ToString());
        //        //Expected XML Output:
        //        //<Results>
        //        //  <Item>
        //        //    <Item_Idx>?</Item_Idx>
        //        //    <Item_Name>?</Item_Name>
        //        //    <IsSelected>0 or 1</IsSelected>
        //        //  </Item>
        //        //</Results>
        //        options.AddRange(res.Elements("Results").Select(att => new Option(att)));
        //        //options.AddRange(res.Elements("Results").Select(att => new Option
        //        //{
        //        //    Item_Idx = att.Element("Item_Idx").Value,
        //        //    Item_Name = att.Element("Item_Name").Value,
        //        //    IsSelected = (att.Element("IsSelected").Value == "1" ? true : false)
        //        //}));
        //    }
        //    else
        //    {
        //        //check for static data


        //    }


        //    return new ObservableCollection<Option>(options);
        //}

        public static ObservableCollection<Option> GetFromXML(string attr, string datasource, bool cacheMe = false)
        {
            var options = new List<Option>();

            if (attr != null)
            {
                //we have some Data input, get dataset from WS 
                var argument = XElement.Parse(attr.Replace("&gt;", ">").Replace("&lt;", "<"));


                switch (argument.Name.ToString())
                {
                    case "User_Idx":
                        argument.Value = Model.User.CurrentUser.ID;
                        break;

                    case "Promo_Idx":

                        break;

                    case "SalesOrg_Idx":
                        argument.Value = Model.User.CurrentUser.SalesOrganisationID.ToString();
                        break;

                    case "DataSourceInput":
                        //fix to issue on manual claim entry screen where when you load the grid its fine, but when you display the tab it is in the data load is broken and sends null 
                        // values to the DB which doesnt load any data into dropdown
                        if (argument.MaybeElement("User_Idx").Value == "" && argument.MaybeElement("SalesOrg_Idx").Value == "" && argument.MaybeElement("ColumnCode").Value == "")
                        {
                            return null;
                        }
                        break;
                }


                // Expected XML Input:
                //<GetItems>
                //  <User_Idx>?</User_Idx>
                //  <MenuItem_Idx>?</MenuItem_Idx>
                //  <SelectedItem_Idx>?</SelectedItem_Idx>
                //</GetItems> 

                XElement res;

                if (cacheMe) res = Model.DataAccess.@WebServiceProxy.Call(datasource, argument, Model.DataAccess.DisplayErrors.No, cacheMe);
                else res = Model.DataAccess.@WebServiceProxy.Call(datasource, argument.ToString(), Model.DataAccess.DisplayErrors.No);

                        //Expected XML Output:
                //<Results>
                //  <Item>
                //    <Item_Idx>?</Item_Idx>
                //    <Item_Name>?</Item_Name>
                //    <IsSelected>0 or 1</IsSelected>
                //  </Item>
                //</Results>

                options.AddRange
                    (
                    res.Elements("Item")
                    .Concat(res.Elements("Value"))
                    .Concat(res.Elements("Dropdown"))
                    .Select(att => new Option(att))
                    );

                //options.AddRange(res.Elements("Item").Select(att => new Option
                //{
                //    Item_Idx = att.Element("Item_Idx").Value,
                //    Item_Name = att.Element("Item_Name").Value,
                //    IsSelected = (att.Element("IsSelected") != null ? (att.Element("IsSelected").Value == "1" ? true : false) : false),
                //    SortOrder = int.Parse(att.Element("SortOrder").Value)
                //}));

                //options.AddRange(res.Elements("Value").Select(att => new Option
                //{
                //    Item_Idx = att.Element("Item_Idx").Value,
                //    Item_Name = att.Element("Item_Name").Value,
                //    IsSelected = (att.Element("IsSelected") != null ? (att.Element("IsSelected").Value == "1" ? true : false) : false),
                //    SortOrder = int.Parse(att.Element("SortOrder").Value)
                //}));

                //options.AddRange(res.Elements("Dropdown").Select(att => new Option
                //{
                //    Item_Idx = att.Element("Item_Idx").Value,
                //    Item_Name = att.Element("Item_Name").Value,
                //    IsSelected = (att.Element("IsSelected") != null ? (att.Element("IsSelected").Value == "1" ? true : false) : false),
                //    SortOrder = int.Parse(att.Element("SortOrder").Value)
                //}));
            }
            else
            {
                //check for static data
                return null;

            }


            return new ObservableCollection<Option>(options);
        }

      

        internal static List<int> GetFilters(IEnumerable<XElement> attr)
        {
            return (from x in attr
                    select Convert.ToInt32(x.Value)).ToList();

        }

        public event PropertyChangedEventHandler PropertyChanged;


        //        public static ObservableCollection<Option> Get(string p)
        //        {

        //            var x =  @"<Results>
        //                            <Item>
        //                              <Item_Idx>1</Item_Idx>
        //                              <Item_Name>a</Item_Name>
        //                              <Item_IsSelected>1</Item_IsSelected>
        //                            </Item>
        //                            <Item>
        //                              <Item_Idx>2</Item_Idx>
        //                              <Item_Name>b</Item_Name>
        //                              <Item_IsSelected>1</Item_IsSelected>
        //                            </Item>
        //                            <Item>
        //                              <Item_Idx>3</Item_Idx>
        //                              <Item_Name>c</Item_Name>
        //                              <Item_IsSelected>1</Item_IsSelected>
        //                            </Item>
        //                          </Results>";


        //            var options = new List<Option>();
        //            var res = XElement.Parse(x);

        //            foreach (var att in res.Elements("Item"))
        //            {
        //                var o = new Option
        //                {
        //                    Item_Idx = att.Element("Item_Idx").Value,
        //                    Item_Name = att.Element("Item_Name").Value,
        //                    IsSelected = (att.Element("Item_IsSelected").Value == "1" ? true : false)
        //                };

        //                options.Add(o);
        //            }

        //            return new ObservableCollection<Option>(options);

        //        }

        //public class GetDropdownItems
        //{ 
        //   public string ColumnCode {get;set;}
        //   public List<Item> ParentItems  {get;set;}
        //}

        //public class Item
        //{
        //   public string ColumnCode {get;set;}
        //   public List<SelectedItem> Values { get; set; }
        //}






    }

}
