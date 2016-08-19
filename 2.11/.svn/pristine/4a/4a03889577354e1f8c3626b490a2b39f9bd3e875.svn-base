// ReSharper disable CheckNamespace

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
using Coder.UI.WPF;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.DynamicRow.ViewModels;
using Exceedra.Controls.DynamicTab.ViewModels;
using Exceedra.Schedule.ViewModels;
using Model;
using Model.DataAccess;
using Model.Entity.Generic;
using Model.Entity.GroupEditor;
using Exceedra.Common.Mvvm;
using ViewHelper;
using WPF;
using Exceedra.MultiSelectCombo.ViewModel;

namespace ViewModels
{
    public class GroupEditorViewModel : BaseViewModel
    {

        private string _robGroupID;
        private string _appTypeID;

        private GroupEditorAccess _groupAccess;

        public GroupEditorViewModel(string appTypeID, string robGroupID)
        {
            _robGroupID = robGroupID;
            _appTypeID = appTypeID;

            _groupAccess = new GroupEditorAccess();
            Init();
        }

        private void Init()
        {
            LoadDetail(_appTypeID, _robGroupID);
            LoadData(_appTypeID, _robGroupID);

            LoadStatuses(_appTypeID, _robGroupID);
            LoadScenarios(_robGroupID);
            LoadCommentList();
        }

        private void LoadStatuses(string appTypeID, string robGroupID)
        {
            Statuses = new ObservableCollection<Status>(_groupAccess.GetWorkflowStatuses(appTypeID, robGroupID).Result);
        }

        private void LoadScenarios(string robGroupID)
        {
            if (Scenarios == null || !Scenarios.Items.Any())
            {
                Scenarios.SetItems(_groupAccess.GetScenarios(string.Empty, robGroupID).Result);
            }
        }

        private void LoadDetail(string appTypeID, string robGroupID)
        {
            Detail = _groupAccess.GetDetail(appTypeID, robGroupID).Result;
            if (Detail.ID == null)
            {
                Detail.ID = robGroupID;
            }
        }

        /// <summary>
        /// Use prepopualted TabbedViewModel to create a tabbed view and populate each control
        /// </summary>
        private void LoadChildControls()
        {
            // cycle through each row of data
            foreach (var col in RVM.Records.Where(p => p.DetailsViewModel != null).ToList())
            {
                // cycle through each itm (tab) of the tabbedViewModel
                GetTabContent(col);

            }
        }

        private void GetTabContent(Record col)
        {
            TabbedViewModel tvm = (TabbedViewModel) col.DetailsViewModel;

            foreach (var p in tvm.Records[0].Properties) //.Where(r => r.TabContent != null)
            {
                var argument = col.ConvertDataSourceInput(p, null, _appTypeID, col.Item_Idx);
                // ad hoc call to DB with proc/xml
                p.TabContent = @WebServiceProxy.Call(p.DataSource, argument.ToString(), DisplayErrors.No);

                // convert each TabContent object into the correct type of control ready object
                switch (p.ControlType)
                {
                    case "HorizontalGrid":
                        var r = new RecordViewModel(XElement.Parse(p.TabContent.ToString()));
                        //@"<Results><RootItem><Item_Idx>16</Item_Idx><Item_Type>Promotion_PandL_Grid_Second</Item_Type><Item_RowSortOrder>16</Item_RowSortOrder><Attributes><Attribute><ColumnCode>TITLE</ColumnCode><HeaderText>Measure        </HeaderText><Value>Base</Value><Format></Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>1</ColumnSortOrder></Attribute><Attribute><ColumnCode>VOLUME</ColumnCode><HeaderText>Volume (Units)</HeaderText><Value>0</Value><Format>N0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>2</ColumnSortOrder></Attribute><Attribute><ColumnCode>TRADE_SPEND</ColumnCode><HeaderText>Trade Spend</HeaderText><Value>0</Value><Format>C0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>3</ColumnSortOrder></Attribute><Attribute><ColumnCode>NET_REVENUE</ColumnCode><HeaderText>Net Revenue</HeaderText><Value>0</Value><Format>C0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>4</ColumnSortOrder></Attribute><Attribute><ColumnCode>CM</ColumnCode><HeaderText>CM%</HeaderText><Value>0</Value><Format>P0</Format><ForeColour /><BorderColour>#FF0000</BorderColour><BackgroundColour>#FF0000</BackgroundColour><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>5</ColumnSortOrder></Attribute><Attribute><ColumnCode>SWP</ColumnCode><HeaderText>Customer Price</HeaderText><Value>0</Value><Format>C2</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>6</ColumnSortOrder></Attribute><Attribute><ColumnCode>SSP</ColumnCode><HeaderText>Consumer Price</HeaderText><Value>0</Value><Format>C2</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>7</ColumnSortOrder></Attribute><Attribute><ColumnCode>PROMO_ROI</ColumnCode><HeaderText>Promo ROI %</HeaderText><Value>0</Value><Format>P0</Format><ForeColour /><BorderColour>#FF0000</BorderColour><BackgroundColour>#FF0000</BackgroundColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>8</ColumnSortOrder></Attribute><Attribute><ColumnCode>RETAILER_REVENUE</ColumnCode><HeaderText>Retailer Sales</HeaderText><Value>0</Value><Format>C0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>9</ColumnSortOrder></Attribute><Attribute><ColumnCode>RM</ColumnCode><HeaderText>Retailer Margin</HeaderText><Value>0</Value><Format>P0</Format><ForeColour /><BorderColour>#FF0000</BorderColour><BackgroundColour>#FF0000</BackgroundColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>10</ColumnSortOrder></Attribute></Attributes></RootItem><RootItem><Item_Idx>16</Item_Idx><Item_Type>Promotion_PandL_Grid_Second</Item_Type><Item_RowSortOrder>16</Item_RowSortOrder><Attributes><Attribute><ColumnCode>TITLE</ColumnCode><HeaderText>Measure        </HeaderText><Value>Incremental</Value><Format></Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>1</ColumnSortOrder></Attribute><Attribute><ColumnCode>VOLUME</ColumnCode><HeaderText>Volume (Units)</HeaderText><Value>1.8e+006</Value><Format>N0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>2</ColumnSortOrder></Attribute><Attribute><ColumnCode>TRADE_SPEND</ColumnCode><HeaderText>Trade Spend</HeaderText><Value>18502.6</Value><Format>C0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>3</ColumnSortOrder></Attribute><Attribute><ColumnCode>NET_REVENUE</ColumnCode><HeaderText>Net Revenue</HeaderText><Value>831997</Value><Format>C0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>4</ColumnSortOrder></Attribute><Attribute><ColumnCode>CM</ColumnCode><HeaderText>CM%</HeaderText><Value>62.8404</Value><Format>P0</Format><ForeColour /><BorderColour>#00FF00</BorderColour><BackgroundColour>#00FF00</BackgroundColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>5</ColumnSortOrder></Attribute><Attribute><ColumnCode>SWP</ColumnCode><HeaderText>Customer Price</HeaderText><Value>0</Value><Format>C2</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>6</ColumnSortOrder></Attribute><Attribute><ColumnCode>SSP</ColumnCode><HeaderText>Consumer Price</HeaderText><Value>0</Value><Format>C2</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>7</ColumnSortOrder></Attribute><Attribute><ColumnCode>PROMO_ROI</ColumnCode><HeaderText>Promo ROI %</HeaderText><Value>0</Value><Format>P0</Format><ForeColour /><BorderColour>#FF0000</BorderColour><BackgroundColour>#FF0000</BackgroundColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>8</ColumnSortOrder></Attribute><Attribute><ColumnCode>RETAILER_REVENUE</ColumnCode><HeaderText>Retailer Sales</HeaderText><Value>18502.6</Value><Format>C0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>9</ColumnSortOrder></Attribute><Attribute><ColumnCode>RM</ColumnCode><HeaderText>Retailer Margin</HeaderText><Value>-4496.66</Value><Format>P0</Format><ForeColour /><BorderColour>#FF0000</BorderColour><BackgroundColour>#FF0000</BackgroundColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>10</ColumnSortOrder></Attribute></Attributes></RootItem><RootItem><Item_Idx>16</Item_Idx><Item_Type>Promotion_PandL_Grid_Second</Item_Type><Item_RowSortOrder>16</Item_RowSortOrder><Attributes><Attribute><ColumnCode>TITLE</ColumnCode><HeaderText>Measure        </HeaderText><Value>Planned Total</Value><Format></Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>1</ColumnSortOrder></Attribute><Attribute><ColumnCode>VOLUME</ColumnCode><HeaderText>Volume (Units)</HeaderText><Value>1.8e+006</Value><Format>N0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>2</ColumnSortOrder></Attribute><Attribute><ColumnCode>TRADE_SPEND</ColumnCode><HeaderText>Trade Spend</HeaderText><Value>18502.6</Value><Format>C0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>3</ColumnSortOrder></Attribute><Attribute><ColumnCode>NET_REVENUE</ColumnCode><HeaderText>Net Revenue</HeaderText><Value>831997</Value><Format>C0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>4</ColumnSortOrder></Attribute><Attribute><ColumnCode>CM</ColumnCode><HeaderText>CM%</HeaderText><Value>62.8404</Value><Format>P0</Format><ForeColour /><BorderColour>#00FF00</BorderColour><BackgroundColour>#00FF00</BackgroundColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>5</ColumnSortOrder></Attribute><Attribute><ColumnCode>SWP</ColumnCode><HeaderText>Customer Price</HeaderText><Value>0.567</Value><Format>C2</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>6</ColumnSortOrder></Attribute><Attribute><ColumnCode>SSP</ColumnCode><HeaderText>Consumer Price</HeaderText><Value>0.0102792</Value><Format>C2</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>7</ColumnSortOrder></Attribute><Attribute><ColumnCode>PROMO_ROI</ColumnCode><HeaderText>Promo ROI %</HeaderText><Value>2888.56</Value><Format>P0</Format><ForeColour /><BorderColour>#00FF00</BorderColour><BackgroundColour>#00FF00</BackgroundColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>8</ColumnSortOrder></Attribute><Attribute><ColumnCode>RETAILER_REVENUE</ColumnCode><HeaderText>Retailer Sales</HeaderText><Value>18502.6</Value><Format>C0</Format><ForeColour /><BorderColour /><BackgroundColour /><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>9</ColumnSortOrder></Attribute><Attribute><ColumnCode>RM</ColumnCode><HeaderText>Retailer Margin</HeaderText><Value>0</Value><Format>P0</Format><ForeColour /><BorderColour>#FF0000</BorderColour><BackgroundColour>#FF0000</BackgroundColour><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><DataSource /><DependentColumn /><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>10</ColumnSortOrder></Attribute></Attributes></RootItem></Results>"
                        r.Records.Do(t =>
                        {
                            foreach (var v in t.Properties)
                            {
                                if (v.ControlType.ToLower().Contains("down"))
                                {
                                    t.InitialDropdownLoad(v);
                                }
                            }
                        });
                        p.TabContent = r;

                        break;
                    case "VerticalGrid":

                        var row = new RowViewModel((XElement.Parse(p.TabContent.ToString())));
                        row.Records.Do(t =>
                        {
                            foreach (var v in t.Properties)
                            {
                                if (v.ControlType.ToLower().Contains("down"))
                                {
                                    t.LoadDependentDrops(v);
                                }

                                if (v.ControlType.ToLower().Contains("grid"))
                                {
                                    //you b'stard what do you mean we need a grid in verticaal grid thats inside a tab inside a grid?
                                }
                            }
                        });

                        p.TabContent = row;
                        break;
                    case "Chart":

                        break;

                    case "ScheduleGrid":

                        var scheduleViewModel = new ScheduleViewModel((XElement.Parse(p.TabContent.ToString())));
                        scheduleViewModel.StartDate = new DateTime(2014, 01, 01);
                        scheduleViewModel.EndDate = new DateTime(2016, 01, 01);

                        p.TabContent = scheduleViewModel;

                        break;

                    default:

                        break;
                }
            }
        }

        /// <summary>
        /// Load the main wrapper grid and then call each row detail proc and populate the TabbedViewModel
        /// </summary>
        /// <param name="appTypeID"></param>
        /// <param name="robGroupID"></param>
        private void LoadData(string appTypeID, string robGroupID)
        {
            _groupAccess.LoadDynamicGrid(appTypeID, robGroupID).ContinueWith(res =>
            {
                RVM = new RecordViewModel(res.Result);

                if (RVM.Records != null)
                {
                    foreach (var x in RVM.Records.Where(t => t.DetailsViewModel == null))
                    {
                        foreach (var p in x.Properties.Where(c => c.ColumnCode == "TabbedView"))
                        {
                            p.IsDisplayed = false;
                            var argument = x.ConvertDataSourceInput(p, null, appTypeID, x.Item_Idx);

                            var t = WebServiceProxy.Call(p.DataSource, argument.ToString(),
                                DisplayErrors.No);

                            x.DetailsViewModel = new TabbedViewModel(t);
                            GetTabContent(x);
                        }
                    }
                    //LoadChildControls(); 
                }
            });


        }

 

        #region Commands

        public ICommand AddCommentCommand
        {
            get { return new ViewCommand(CanAddComment, AddComment); }
        }

        public ICommand SaveCommand
        {
            get { return new ViewCommand(CanSave, Save); }
        }

        public ICommand AddGroupCommand
        {
            get { return new ViewCommand(CanAdd, AddGroup); }
        }

    

        public ICommand CancelCommand
        {
            get { return new ViewCommand(Cancel); }
        }

        private void Cancel(object obj)
        {
            App.Navigator.EnableNavigation(true);
            MessageBus.Instance.Publish(NavigateMessage.Back);
        }

        #endregion

        #region Save

        //save fund
        // [app].[Procast_SP_FUND_SaveFund]
        private void Save(object obj)
        {
            // serialise wrapper
            var xdoc3 = new XDocument(
                               new XElement("Robs",
                                   from rt in RVM.Records.Where(r => r.HasChanges != 2)
                                   select new XElement("Rob",
                                       new XElement("ROB_Name", Detail.Name),
                                       new XElement("ROB_Idx", rt.Item_Idx),                                       
                                           GetChangedData(rt)
                                           )
                                       )                                 
                               );
             
            var xml = new XDocument(
                new XElement("SaveROBGroup",
                      new XElement("User_Idx", User.CurrentUser.ID),
                      new XElement("SalesOrg_Idx", User.CurrentUser.SalesOrganisationID),
                      new XElement("ROBGroup_Idx", Detail.ID),
                      new XElement("AppType_Idx", _appTypeID),
                      new XElement("ROBGroup_Name", Detail.Name),
                      new XElement("Status_Idx", SelectedStatus.ID),
                      new XElement("Scenarios",
                          from x in Scenarios.SelectedItems
                          select new XElement("Scen_Idx", x.Idx)
                          ), 
                                NewComment != null ? new XElement("Comment", new XElement("Value", NewComment)) : null,
                                     new XElement("DeletedROBs",
                                          from x in RVM.Records.Where(r=>r.HasChanges == 2)
                                          select new XElement("ROB_Idx", x.Item_Idx)
                                          ),

                         XElement.Parse(xdoc3.ToString())
                    )
                    
                 );
             
            var res = _groupAccess.Save(xml.ToString());

            if (res.Element("Message") != null)
            {
                var message = res.Element("Message");

                if (message.Element("OutcomeMsg").Value == "Success")
                { 
                    var idx = message.Element("ROBGroup_Idx").Value;
                    var amend = message.Element("IsAmendable").Value == "1";

                    Detail.ID = _robGroupID = idx;
                    Detail.IsAmendable = amend;

                    //<Results>
                    //  <Message>
                    //    <OutcomeMsg>Success</OutcomeMsg>
                    //    <ROBGroup_Idx>2</ROBGroup_Idx>
                    //    <IsAmendable>1</IsAmendable>
                    //  </Message>
                    //</Results>
                     
                    Init();
                }

            }
             
        }

        private XElement  GetChangedData(Record rt)
        {
            if (rt.DetailsViewModel != null)
            {
                TabbedViewModel tvm = (TabbedViewModel)rt.DetailsViewModel;

                var res = from pp in tvm.Records[0].Properties.Where(y => y.ControlType != "Chart")
                    select new XElement("Attribute",
                        new XElement("ColumnCode", pp.ColumnCode),
                        DeserialiseTabContent(pp.TabContent, pp.ControlType)                     
                        );
                 
                return new XElement("Attributes", res);
            }
            return null;
        }

        private XElement DeserialiseTabContent(object p, string controlType)
        {
          //deserailsie the tab content based on the control type for the tab             
            switch (controlType.ToLower() )
            {
                case "horizontalgrid":
                    var r = p as RecordViewModel;

                    if (r.HasChanged)
                    {
                        var docH = new XDocument(
                            new XElement("Value",
                                from rt in r.Records
                                select new XElement("RootItem",
                                    new XElement("Item_Type", rt.Item_Type),
                                    new XElement("Item_Idx", rt.Item_Idx),
                                    new XElement("Attributes",
                                        from pp in rt.Properties
                                        select new XElement("Attribute",
                                            new XElement("ColumnCode", pp.ColumnCode),
                                            new XElement("Value", pp.Value)
                                            )
                                        )
                                    )
                                )
                            );

                        return XElement.Parse(docH.ToString());
                    }
                    else
                    {
                        return null;
                    }
                case "verticalgrid":
                    var v = p as RowViewModel;

                    if (v != null)//&& v.HasChanged
                    {

                        var docV = v.ToAttributeXml();
                        //var docV = new XDocument(
                        //    new XElement("Attributes",
                        //        from rt in v.Records
                        //        select new XElement("RootItem",
                        //            new XElement("Item_Type", rt.Item_Type),
                        //            new XElement("Item_Idx", rt.Item_Idx),
                        //            new XElement("Attributes",
                        //                from pp in rt.Properties
                        //                select new XElement("Attribute",
                        //                    new XElement("ColumnCode", pp.ColumnCode),
                        //                    pp.GetXmlSelectedItems()
                        //                    )
                        //                )
                        //            )
                        //        )
                        //    );

                        return XElement.Parse(docV.ToString());
                    }
                    else
                    {
                        return null;
                    }
                default:
                    return null;
            }

 

        }
 
        private bool CanSave(object obj)
        {
            return IsValid;
        }

        public bool IsValid
        {
            get
            {
                return (!string.IsNullOrEmpty(Detail.Name)
                        && SelectedStatus != null
                        && Scenarios.SelectedItems.Any());
            }
        }

        #endregion

        #region AddGroup
        private void AddGroup(object obj)
        {
            // create new rob group with a negative ID and add send get tab layout to DB for tab content

            var ids = RVM.Records.Select(t => Convert.ToInt32(t.Item_Idx));

            //need to send the DB a unique ID that is negative
            var min = ids.Any() ?  ids.Min() : 0;

            if (min == 0 || min > 0)
            {
                min = -1;
            }
            else
            {
                min -= 1;
            }

            var args = XElement.Parse(string.Format(@"<DataSourceInput>
                                      <User_Idx>{0}</User_Idx>
                                      <SalesOrg_Idx>{1}</SalesOrg_Idx>
                                      <AppType_Idx>{2}</AppType_Idx>
                                      <ROB_Idx>{3}</ROB_Idx>
                                    </DataSourceInput>",
                                                       User.CurrentUser.ID, 
                                                       User.CurrentUser.SalesOrganisationID, 
                                                       _appTypeID, 
                                                       min)
                                     );

            var x = @WebServiceProxy.Call(StoredProcedure.RobGroup.GetTabLayout, args, DisplayErrors.No);
            var tvm = new TabbedViewModel(x);

            RVM.AddRecord(RVM.Records[0].Clone().ToList(), "", min, tvm);
            GetTabContent(RVM.Records.Single(y=>y.Item_Idx== min.ToString()));
            NotifyPropertyChanged(this, vm => vm.RVM);
         
        }

       

        private bool CanAdd(object obj)
        {
            return Detail.IsAmendable;
        }
        #endregion

        #region Comments
        private ObservableCollection<Note> _notes;
        private ObservableCollection<Note> _commentList;
        private string _newComment;         


        public string NewComment
        {
            get
            {
                return _newComment;

            }
            set
            {
                _newComment = value;
                NotifyPropertyChanged(this, vm => vm.NewComment);
            }
        }

        public bool CanAddComment(object param)
        {
            return HasID();
        }

        public ObservableCollection<Note> Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                NotifyPropertyChanged(this, vm => vm.Notes);
            }
        }


        public void LoadCommentList()
        {
            _groupAccess.GetNotes(Detail.ID).ContinueWith(y =>
            {
                Notes = new ObservableCollection<Note>(y.Result);
            });
        }

        public void AddComment(object param)
        {
            if (NewComment.Trim() == "")
            {
                MessageBoxShow("Please enter a value.", "Warning");
                return;
            }

            if (HasID() == false)
            {
                MessageBoxShow("Comments can not be added until this promotion has been saved", "Warning");
                return;
            }

            try
            {
                string res = _groupAccess.AddNote(Detail.ID, NewComment);
                //MessageBoxShow(res); 
                NewComment = "";

                LoadCommentList();

            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message, "Error");
            }
        }

        private bool HasID()
        {
            return (!string.IsNullOrWhiteSpace(Detail.ID) && Detail.ID != "-1");
        }

        #endregion

        #region detail

        private GroupBase _detail;

        public GroupBase Detail
        {
            get { return _detail; }
            set
            {
                _detail = value;
                NotifyPropertyChanged(this, vm => vm.Detail);
            }
        }
 

        #endregion


        #region Statuses
        private ObservableCollection<Status> _statuses;
        private Status _selectedStatus;
        public ObservableCollection<Status> Statuses
        {
            get { return _statuses; }
            set
            {
                _statuses = value;
                NotifyPropertyChanged(this, vm => vm.Statuses);

                SelectedStatus = Statuses.FirstOrDefault(t=>t.IsSelected);

            }
        }
        public Status SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {

                if (_selectedStatus != value)
                {
                    _selectedStatus = value;
                    NotifyPropertyChanged(this, vm => vm.SelectedStatus);

                }
            }
        }

        #endregion

        #region Scenarios
        private MultiSelectViewModel _scenarios = new MultiSelectViewModel();
        public MultiSelectViewModel Scenarios
        {
            get { return _scenarios; }
            set
            {
                _scenarios = value;
                NotifyPropertyChanged(this, vm => vm.Scenarios);
            }
        }

        #endregion

        #region Private Properties

        private static RecordViewModel _RVM;

        public RecordViewModel RVM
        {
            get { return _RVM; }
            set
            {
                if (_RVM != value)
                {
                    _RVM = value;
                 
                    NotifyPropertyChanged(this, vm => vm.RVM);
                }
            }
        }
         
        #endregion
 

//        #region XML

//        const string g1 = @"
//<Results>
//  <RootItem>
//    <Item_Idx>1</Item_Idx>
//    <Item_Type>Scenario</Item_Type>
//    <Item_RowSortOrder>1</Item_RowSortOrder>
//    <Attributes>
//      <Attribute>
//        <ColumnCode>IsSelected</ColumnCode>
//        <HeaderText />
//        <Value>false</Value>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>1</IsEditable>
//        <ControlType>Checkbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Scen_Name</ColumnCode>
//        <HeaderText>Scenario Name</HeaderText>
//        <Value>(LIVE) - Live</Value>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Hyperlink</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>COUNT</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Author</ColumnCode>
//        <HeaderText>Author</HeaderText>
//        <Value>Exceedra, Admin</Value>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Start_Day</ColumnCode>
//        <HeaderText>Start Date</HeaderText>
//        <Value>2015-01-02</Value>
//        <Format>ShortDate</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>End_Day</ColumnCode>
//        <HeaderText>End Date</HeaderText>
//        <Value>2017-01-01</Value>
//        <Format>ShortDate</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Create_Day</ColumnCode>
//        <HeaderText>Created On</HeaderText>
//        <Value>2015-06-06</Value>
//        <Format>ShortDate</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Type_Name</ColumnCode>
//        <HeaderText>Type</HeaderText>
//        <Value>System Controlled, Non-User Editable</Value>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Status_Name</ColumnCode>
//        <HeaderText>Status</HeaderText>
//        <Value>System Controlled, Non-User Editable</Value>
//        <Format />
//        <ForeColour>#000000</ForeColour>
//        <BorderColour>#A9A9A9</BorderColour>
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Num_Customers</ColumnCode>
//        <HeaderText>Customers</HeaderText>
//        <Value>193</Value>
//        <Format>N0</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Num_Products</ColumnCode>
//        <HeaderText>Products</HeaderText>
//        <Value>50</Value>
//        <Format>N0</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Num_Promotions</ColumnCode>
//        <HeaderText>Promotions</HeaderText>
//        <Value>0</Value>
//        <Format>N0</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>IsActiveBudget</ColumnCode>
//        <HeaderText>Active Budget</HeaderText>
//        <Value>false</Value>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>1</IsEditable>
//        <ControlType>CheckBox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//    </Attributes>
//  </RootItem>
//  <RootItem>
//    <Item_Idx>2</Item_Idx>
//    <Item_Type>Scenario</Item_Type>
//    <Item_RowSortOrder>2</Item_RowSortOrder>
//    <Attributes>
//      <Attribute>
//        <ColumnCode>IsSelected</ColumnCode>
//        <HeaderText />
//        <Value>false</Value>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>1</IsEditable>
//        <ControlType>Checkbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Scen_Name</ColumnCode>
//        <HeaderText>Scenario Name</HeaderText>
//        <Value>(CH-1) - test</Value>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Hyperlink</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>COUNT</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Author</ColumnCode>
//        <HeaderText>Author</HeaderText>
//        <Value>Hogan, Craig</Value>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Start_Day</ColumnCode>
//        <HeaderText>Start Date</HeaderText>
//        <Value>2015-01-02</Value>
//        <Format>ShortDate</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>End_Day</ColumnCode>
//        <HeaderText>End Date</HeaderText>
//        <Value>2017-01-01</Value>
//        <Format>ShortDate</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Create_Day</ColumnCode>
//        <HeaderText>Created On</HeaderText>
//        <Value>2015-06-08</Value>
//        <Format>ShortDate</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Type_Name</ColumnCode>
//        <HeaderText>Type</HeaderText>
//        <Value>User Scenario</Value>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Status_Name</ColumnCode>
//        <HeaderText>Status</HeaderText>
//        <Value>Open</Value>
//        <Format />
//        <ForeColour>#000000</ForeColour>
//        <BorderColour>#FFFFFF</BorderColour>
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Num_Customers</ColumnCode>
//        <HeaderText>Customers</HeaderText>
//        <Value>17</Value>
//        <Format>N0</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Num_Products</ColumnCode>
//        <HeaderText>Products</HeaderText>
//        <Value>42</Value>
//        <Format>N0</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Num_Promotions</ColumnCode>
//        <HeaderText>Promotions</HeaderText>
//        <Value>0</Value>
//        <Format>N0</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>IsActiveBudget</ColumnCode>
//        <HeaderText>Active Budget</HeaderText>
//        <Value>false</Value>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>1</IsEditable>
//        <ControlType>CheckBox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//    </Attributes>
//  </RootItem>
//  <RootItem>
//    <Item_Idx>3</Item_Idx>
//    <Item_Type>Scenario</Item_Type>
//    <Item_RowSortOrder>3</Item_RowSortOrder>
//    <Attributes>
//      <Attribute>
//        <ColumnCode>IsSelected</ColumnCode>
//        <HeaderText />
//        <Value>false</Value>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>1</IsEditable>
//        <ControlType>Checkbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Scen_Name</ColumnCode>
//        <HeaderText>Scenario Name</HeaderText>
//        <Value>(CH-2) - test</Value>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Hyperlink</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>COUNT</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Author</ColumnCode>
//        <HeaderText>Author</HeaderText>
//        <Value>Hogan, Craig</Value>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Start_Day</ColumnCode>
//        <HeaderText>Start Date</HeaderText>
//        <Value>2015-01-02</Value>
//        <Format>ShortDate</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>End_Day</ColumnCode>
//        <HeaderText>End Date</HeaderText>
//        <Value>2017-01-01</Value>
//        <Format>ShortDate</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Create_Day</ColumnCode>
//        <HeaderText>Created On</HeaderText>
//        <Value>2015-06-08</Value>
//        <Format>ShortDate</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Type_Name</ColumnCode>
//        <HeaderText>Type</HeaderText>
//        <Value>User Scenario</Value>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Status_Name</ColumnCode>
//        <HeaderText>Status</HeaderText>
//        <Value>Open</Value>
//        <Format />
//        <ForeColour>#000000</ForeColour>
//        <BorderColour>#FFFFFF</BorderColour>
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Num_Customers</ColumnCode>
//        <HeaderText>Customers</HeaderText>
//        <Value>17</Value>
//        <Format>N0</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Num_Products</ColumnCode>
//        <HeaderText>Products</HeaderText>
//        <Value>42</Value>
//        <Format>N0</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>Num_Promotions</ColumnCode>
//        <HeaderText>Promotions</HeaderText>
//        <Value>0</Value>
//        <Format>N0</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Textbox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>IsActiveBudget</ColumnCode>
//        <HeaderText>Active Budget</HeaderText>
//        <Value>false</Value>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>1</IsEditable>
//        <ControlType>CheckBox</ControlType>
//        <DataSource />
//        <DependentColumn />
//        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
//      </Attribute>
//    </Attributes>
//  </RootItem>
//</Results>";

//        const string v1 = @"<Results>
//  <RootItem>
//    <Item_Idx>26</Item_Idx>
//    <Item_Type>Promotion</Item_Type>
//    <Item_RowSortOrder>1</Item_RowSortOrder>
//    <Attributes>
//      <Attribute>
//        <ColumnCode>MECHANIC</ColumnCode>
//        <HeaderText>Mechanic</HeaderText>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>1</IsEditable>
//        <ControlType>Dropdown</ControlType>
//        <DataSource>app.Procast_SP_PROMO_Wizard_Attributes_PopulateDropdowns</DataSource>
//        <DataSourceInput>&lt;User_Idx /&gt;&lt;Promo_Idx /&gt;&lt;ColumnCode /&gt;</DataSourceInput>
//        <DependentColumns />
//        <AttributeGroup_Sort>0</AttributeGroup_Sort>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>FEATURE</ColumnCode>
//        <HeaderText>Feature</HeaderText>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>1</IsEditable>
//        <ControlType>Dropdown</ControlType>
//        <DataSource>app.Procast_SP_PROMO_Wizard_Attributes_PopulateDropdowns</DataSource>
//        <DataSourceInput>&lt;User_Idx /&gt;&lt;Promo_Idx /&gt;&lt;ColumnCode /&gt;</DataSourceInput>
//        <DependentColumns />
//        <AttributeGroup_Sort>1</AttributeGroup_Sort>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>SUPPORT</ColumnCode>
//        <HeaderText>Support</HeaderText>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>MultiSelectDropdown</ControlType>
//        <DataSource>app.Procast_SP_PROMO_Wizard_Attributes_PopulateDropdowns</DataSource>
//        <DataSourceInput>&lt;User_Idx /&gt;&lt;Promo_Idx /&gt;&lt;ColumnCode /&gt;</DataSourceInput>
//        <DependentColumns />
//        <AttributeGroup_Sort>2</AttributeGroup_Sort>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>OBJECTIVES</ColumnCode>
//        <HeaderText>Objectives</HeaderText>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>MultiSelectDropdown</ControlType>
//        <DataSource>app.Procast_SP_PROMO_Wizard_Attributes_PopulateDropdowns</DataSource>
//        <DataSourceInput>&lt;User_Idx /&gt;&lt;Promo_Idx /&gt;&lt;ColumnCode /&gt;</DataSourceInput>
//        <DependentColumns />
//        <AttributeGroup_Sort>3</AttributeGroup_Sort>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>PROMO_FUNDS</ColumnCode>
//        <HeaderText>Funding</HeaderText>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Dropdown</ControlType>
//        <DataSource>app.Procast_SP_PROMO_Wizard_Attributes_PopulateDropdowns</DataSource>
//        <DataSourceInput>&lt;User_Idx /&gt;&lt;Promo_Idx /&gt;&lt;ColumnCode /&gt;</DataSourceInput>
//        <DependentColumns />
//        <AttributeGroup_Sort>4</AttributeGroup_Sort>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>HIERARCHY</ColumnCode>
//        <HeaderText>Use Hierarchy Level Planning</HeaderText>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>1</IsEditable>
//        <ControlType>Dropdown</ControlType>
//        <DataSource>app.Procast_SP_PROMO_Wizard_Attributes_PopulateDropdowns</DataSource>
//        <DataSourceInput>&lt;User_Idx /&gt;&lt;Promo_Idx /&gt;&lt;ColumnCode /&gt;</DataSourceInput>
//        <DependentColumns />
//        <AttributeGroup_Sort>6</AttributeGroup_Sort>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>ADD_SPEND_REQ</ColumnCode>
//        <HeaderText>Additional Spend Request</HeaderText>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Dropdown</ControlType>
//        <DataSource>app.Procast_SP_PROMO_Wizard_Attributes_PopulateDropdowns</DataSource>
//        <DataSourceInput>&lt;User_Idx /&gt;&lt;Promo_Idx /&gt;&lt;ColumnCode /&gt;</DataSourceInput>
//        <DependentColumns />
//        <AttributeGroup_Sort>10</AttributeGroup_Sort>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>USE_UPLIFT</ColumnCode>
//        <HeaderText>Use Promotion Uplift Method</HeaderText>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Dropdown</ControlType>
//        <DataSource>app.Procast_SP_PROMO_Wizard_Attributes_PopulateDropdowns</DataSource>
//        <DataSourceInput>&lt;User_Idx /&gt;&lt;Promo_Idx /&gt;&lt;ColumnCode /&gt;</DataSourceInput>
//        <DependentColumns />
//        <AttributeGroup_Sort>12</AttributeGroup_Sort>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>MAINTAIN_BASE</ColumnCode>
//        <HeaderText>Keep Promotion Base Volume Updated</HeaderText>
//        <Format />
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>0</IsEditable>
//        <ControlType>Dropdown</ControlType>
//        <DataSource>app.Procast_SP_PROMO_Wizard_Attributes_PopulateDropdowns</DataSource>
//        <DataSourceInput>&lt;User_Idx /&gt;&lt;Promo_Idx /&gt;&lt;ColumnCode /&gt;</DataSourceInput>
//        <DependentColumns />
//        <AttributeGroup_Sort>13</AttributeGroup_Sort>
//      </Attribute>
//    </Attributes>
//  </RootItem>
//</Results>";

//        private const string c1 = @"<Results><ChartType>Categorical</ChartType><xAxisType>Categorical</xAxisType><yAxisType>Linear</yAxisType><RightClickMenu_IsExportEnabled>1</RightClickMenu_IsExportEnabled><RightClickMenu_IsCategoryEnabled>0</RightClickMenu_IsCategoryEnabled><RightClickMenu_IsLinearEnabled>0</RightClickMenu_IsLinearEnabled><yAxisTitle>Contribution</yAxisTitle><xAxisTitle>Month</xAxisTitle><xAxisMin>201501</xAxisMin><xAxisMax>201601</xAxisMax><yAxisMin>-452807</yAxisMin><yAxisMax>6212064</yAxisMax>-<Series><SeriesType>Bar</SeriesType>-<Datapoints>-<Datapoint><Tooltip_Header1>January 2015</Tooltip_Header1><Tooltip_Header2>AP_Contribution</Tooltip_Header2><Tooltip_Header3/><Tooltip_Color/><X>201501.00</X><Y>1356708.40</Y></Datapoint>-<Datapoint><Tooltip_Header1>February 2015</Tooltip_Header1><Tooltip_Header2>AP_Contribution</Tooltip_Header2><Tooltip_Header3/><Tooltip_Color/><X>201502.00</X><Y>2768089.05</Y></Datapoint></Datapoints></Series>-<Series><SeriesType>Bar</SeriesType>-<Datapoints>-<Datapoint><Tooltip_Header1>January 2015</Tooltip_Header1><Tooltip_Header2>AP_Contribution</Tooltip_Header2><Tooltip_Header3/><Tooltip_Color/><X>201501.00</X><Y>2356708.40</Y></Datapoint>-<Datapoint><Tooltip_Header1>February 2015</Tooltip_Header1><Tooltip_Header2>AP_Contribution</Tooltip_Header2><Tooltip_Header3/><Tooltip_Color/><X>201502.00</X><Y>1768089.05</Y></Datapoint></Datapoints></Series>-<Series><SeriesType>Line</SeriesType>-<Datapoints>-<Datapoint><Tooltip_Header1/><Tooltip_Header2/><Tooltip_Header3/><Tooltip_Color/><X>201501.00</X><Y>2356708.40</Y></Datapoint>-<Datapoint><Tooltip_Header1/><Tooltip_Header2/><Tooltip_Header3/><Tooltip_Color/><X>201502.00</X><Y>1768089.05</Y></Datapoint></Datapoints></Series></Results>";

//        #endregion
    }
     
    }


//<Attribute>
//     <ColumnCode>MECHANIC</ColumnCode>
//     <HeaderText>Mechanic</HeaderText>
//     <Format />
//     <Values>
//         <Item>
//             <Item_Idx>1</Item_Idx>
//             <Item_Name>A</Item_Name>
//             <IsSelected>0</IsSelected>
//         </Item>
//         <Item>
//             <Item_Idx>2</Item_Idx>
//             <Item_Name>B</Item_Name>
//             <IsSelected>0</IsSelected>
//         </Item>
//         <Item>
//             <Item_Idx>3</Item_Idx>
//             <Item_Name>C</Item_Name>
//             <IsSelected>1</IsSelected>
//         </Item>
//          <Item>
//             <Item_Idx>25</Item_Idx>
//             <Item_Name>Yes</Item_Name>
//             <IsSelected>0</IsSelected>               
//           </Item>
//           <Item>
//             <Item_Idx>26</Item_Idx>
//             <Item_Name>No</Item_Name>
//             <IsSelected>1</IsSelected>                
//           </Item>
//     </Values>
//     <ForeColour />
//     <BorderColour />
//     <IsDisplayed>1</IsDisplayed>
//     <IsEditable>1</IsEditable>
//     <ControlType>Dropdown</ControlType>
//     <DataSource/>
//     <DataSourceInput/>
//     <DependentColumns />
//     <AttributeGroup_Sort>0</AttributeGroup_Sort>
//   </Attribute>
