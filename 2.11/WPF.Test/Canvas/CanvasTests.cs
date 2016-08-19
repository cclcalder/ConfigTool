//using System.Collections.Generic;
//using System.Linq;
//using System.Xml.Linq;
//using Exceedra.Controls.DynamicRow.Models;
//using Exceedra.Controls.DynamicRow.ViewModels;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Model;
//using Model.Entity.Canvas;
//using Model.Utilities;
//using Moq;
//using WPF.ViewModels.Canvas;

//namespace WPF.Test.Canvas
//{
//    [TestClass]
//    public class CanvasTests
//    {
//        /// <summary>
//        /// Returns a fake menu insights list - everytime creating a NEW one
//        /// </summary>
//        private IList<Insight> GetFakeMenuInsights()
//        {
//            // As we need a list of menu insights that can't be amended between separate tests
//            // there is no use to create a list just once and keep returning it 
//            // because then we would need to return a deep copy of this list (to make it non-amendable)
//            // which takes approximately the same time as creating new list from scratch

//            return new List<Insight>
//            {
//                new Insight
//                {
//                    GroupId = "group1",
//                    Id = "menuInsight1"
//                },
//                new Insight
//                {
//                    GroupId = "group2"
//                },
//                new Insight
//                {
//                    GroupId = "group1"
//                },
//                new Insight
//                {
//                    GroupId = "group2"
//                },
//                new Insight
//                {
//                    GroupId = "group1"
//                },
//                new Insight
//                {
//                    GroupId = "group3"
//                },
//                new Insight
//                {
//                    GroupId = "group1"
//                },
//                new Insight
//                {
//                    GroupId = "group4"
//                },
//                new Insight
//                {
//                    GroupId = "group2"
//                },
//                new Insight
//                {
//                    GroupId = "group1"
//                }
//            };
//        }

//        [TestMethod]
//        public void LoadMenuInsightsCommand()
//        {
//            // Arrange
//            IList<Insight> fakeMenuInsightsReturnedFromDb = GetFakeMenuInsights();

//            var canvasAccessMock = new Mock<ICanvasAccessor>();
//            canvasAccessMock.Setup(x => x.GetInsights()).ReturnsAsync(fakeMenuInsightsReturnedFromDb);

//            CanvasViewModel canvasViewModel = new CanvasViewModel(canvasAccessMock.Object, null);

//            // Act
//            canvasViewModel.LoadInsightsCommand.Execute(null);

//            // Assert

//            // Is calling fake database to load menu insights during initialisation
//            canvasAccessMock.Verify(x => x.GetInsights(), "Not calling mocked db to get menu insights during initialisation");

//            // Is number of loaded menu insights the same as the number of menu insights returned by the fake database
//            Assert.AreEqual(fakeMenuInsightsReturnedFromDb.Count, canvasViewModel.Insights.SelectMany(x => x).Count(), "The number of loaded menu insights is different from number of insights returned from mocked db");

//            // Is number of created groups the same as number of distinct groups ids of menu insights returned by the fake database
//            int expectedNumberOfGroups = fakeMenuInsightsReturnedFromDb.Select(x => x.GroupId).Distinct().Count();
//            Assert.AreEqual(expectedNumberOfGroups, canvasViewModel.Insights.Count, "Wrong number of groups in the menu");

//            // Do menu insights in each menu group have the same group id
//            var firstMenuInsightFromEachGroup = new List<Insight>();
//            foreach (var menuInsightsGroup in canvasViewModel.Insights)
//            {
//                Assert.IsFalse(menuInsightsGroup.Any(x => x.GroupId != menuInsightsGroup[0].GroupId), "Menu insights in the same menu group have different menu group ids");
//                firstMenuInsightFromEachGroup.Add(menuInsightsGroup.First());
//            }

//            // Do first menu insights from each group have different group id
//            Assert.AreEqual(expectedNumberOfGroups, firstMenuInsightFromEachGroup.Select(x => x.GroupId).Distinct().Count(), "Some menu insights of the same menu group id belong to different groups");
//        }

//        [TestMethod]
//        public void LoadingFiltersAndDividingThemIntoColumns()
//        {
//            // Arrange
//            Insight fakeSelectedMenuInsight = GetFakeMenuInsights().FirstOrDefault();

//            #region filters returned from db
//            XElement fakeFiltersReturnedFromDb = XElement.Parse(@"
//<Results>
//  <RootItem>
//    <Item_Idx>1</Item_Idx>
//    <Item_Type>Type1</Item_Type>
//    <HeaderText>FirstFilterColumn</HeaderText>
//    <Item_RowSortOrder>1</Item_RowSortOrder>
//    <Attributes>
//      <Attribute>
//        <ColumnCode>FirstFilter</ColumnCode>
//        <HeaderText>First filter</HeaderText>
//        <Format>dd/MM/yyyy</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>1</IsEditable>
//        <ControlType>DatePicker</ControlType>
//        <DataSource />
//        <DataSourceInput />
//        <DependentColumns/>
//        <Values>
//          <Value />
//        </Values>
//        <SortOrder>1</SortOrder>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>SecondFilter</ColumnCode>
//        <HeaderText>Second filter</HeaderText>
//        <Format>dd/MM/yyyy</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>1</IsEditable>
//        <ControlType>DatePicker</ControlType>
//        <DataSource />
//        <DataSourceInput />
//        <DependentColumns/>
//        <Values>
//          <Value />
//        </Values>
//        <SortOrder>2</SortOrder>
//      </Attribute>
//    </Attributes>
//  </RootItem>
//  <RootItem>
//    <Item_Idx>2</Item_Idx>
//    <Item_Type>Type2</Item_Type>
//    <HeaderText>SecondFilterColumn</HeaderText>
//    <Item_RowSortOrder>2</Item_RowSortOrder>
//    <Attributes>
//      <Attribute>
//        <ColumnCode>ThirdFilter</ColumnCode>
//        <HeaderText>Third filter</HeaderText>
//        <Format>dd/MM/yyyy</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>1</IsEditable>
//        <ControlType>DatePicker</ControlType>
//        <DataSource />
//        <DataSourceInput />
//        <Values>
//          <Value />
//        </Values>
//        <SortOrder>1</SortOrder>
//      </Attribute>
//    </Attributes>
//  </RootItem>
//</Results>
//            ");
//            #endregion

//            var insightsAccessMock = new Mock<ICanvasAccessor>();
//            insightsAccessMock.Setup(x => x.GetFilters(fakeSelectedMenuInsight.Id)).ReturnsAsync(fakeFiltersReturnedFromDb);

//            var rowViewModelProviderMock = new Mock<IRowViewModelProvider>();

//            #region mocking row view model provider

//            XElement fakeFirstFiltersColumns = XElement.Parse(@"
//<Results>
//  <RootItem>
//    <Item_Idx>1</Item_Idx>
//    <Item_Type>Type1</Item_Type>
//    <HeaderText>FirstFilterColumn</HeaderText>
//    <Item_RowSortOrder>1</Item_RowSortOrder>
//    <Attributes>
//      <Attribute>
//        <ColumnCode>FirstFilter</ColumnCode>
//        <HeaderText>First filter</HeaderText>
//        <Format>dd/MM/yyyy</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>1</IsEditable>
//        <ControlType>DatePicker</ControlType>
//        <DataSource />
//        <DataSourceInput />
//        <DependentColumns />
//        <Values>
//          <Value />
//        </Values>
//        <SortOrder>1</SortOrder>
//      </Attribute>
//      <Attribute>
//        <ColumnCode>SecondFilter</ColumnCode>
//        <HeaderText>Second filter</HeaderText>
//        <Format>dd/MM/yyyy</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>1</IsEditable>
//        <ControlType>DatePicker</ControlType>
//        <DataSource />
//        <DataSourceInput />
//        <DependentColumns />
//        <Values>
//          <Value />
//        </Values>
//        <SortOrder>2</SortOrder>
//      </Attribute>
//    </Attributes>
//  </RootItem>
//</Results>
//");

//            XElement fakeSecondFiltersColumns = XElement.Parse(@"
//<Results>
//  <RootItem>
//    <Item_Idx>2</Item_Idx>
//    <Item_Type>Type2</Item_Type>
//    <HeaderText>SecondFilterColumn</HeaderText>
//    <Item_RowSortOrder>2</Item_RowSortOrder>
//    <Attributes>
//      <Attribute>
//        <ColumnCode>ThirdFilter</ColumnCode>
//        <HeaderText>Third filter</HeaderText>
//        <Format>dd/MM/yyyy</Format>
//        <ForeColour />
//        <BorderColour />
//        <IsDisplayed>1</IsDisplayed>
//        <IsEditable>1</IsEditable>
//        <ControlType>DatePicker</ControlType>
//        <DataSource />
//        <DataSourceInput />
//        <Values>
//          <Value />
//        </Values>
//        <SortOrder>1</SortOrder>
//      </Attribute>
//    </Attributes>
//  </RootItem>
//</Results>
//");

//            List<RowViewModel> fakeColumnsReturnedFromTheDb = new List<RowViewModel>
//            {
//                new RowViewModel
//                {
//                    Records = new RangeObservableCollection<RowRecord>
//                    {
//                        new RowRecord
//                        {
//                            Item_Idx = "1",
//                            Item_Type = "Type1",
//                            HeaderText = "FirstFilterColumn",
//                            Item_RowSortOrder = 1,
//                            Properties = new RangeObservableCollection<RowProperty>
//                            {
//                                new RowProperty
//                                {
//                                    ColumnCode = "FirstFilter",
//                                    HeaderText = "First filter",
//                                    StringFormat = "dd/MM/yyyy",
//                                    IsDisplayed = true,
//                                    IsEditable = true,
//                                    ControlType = "DatePicker"
//                                },
//                                new RowProperty
//                                {
//                                    ColumnCode = "SecondFilter",
//                                    HeaderText = "Second filter",
//                                    StringFormat = "dd/MM/yyyy",
//                                    IsDisplayed = true,
//                                    IsEditable = true,
//                                    ControlType = "DatePicker"
//                                }
//                            }
//                        }
//                    }
//                },
//                new RowViewModel
//                {
//                    Records = new RangeObservableCollection<RowRecord>
//                    {
//                        new RowRecord
//                        {
//                            Item_Idx = "2",
//                            Item_Type = "Type2",
//                            HeaderText = "SecondFilterColumn",
//                            Item_RowSortOrder = 2,
//                            Properties = new RangeObservableCollection<RowProperty>
//                            {
//                                new RowProperty
//                                {
//                                    ColumnCode = "ThirdFilter",
//                                    HeaderText = "Third filter",
//                                    StringFormat = "dd/MM/yyyy",
//                                    IsDisplayed = true,
//                                    IsEditable = true,
//                                    ControlType = "DatePicker"
//                                }
//                            }
//                        }
//                    }
//                }
//            };

//            XmlComparer xmlComparer = new XmlComparer();

//            // if the test invoke RowViewModelProvider.Get(column, selectedMenuInsight) with the column semantically equal to "fakeFirstFiltersColumns" ...
//            rowViewModelProviderMock.Setup(
//                x => x.Get(
//                    It.Is<XElement>(arg => xmlComparer.DeepEquals(arg, fakeFirstFiltersColumns, null)),
//                    fakeSelectedMenuInsight.Id
//                    ))
//                    .Returns(fakeColumnsReturnedFromTheDb[0]);

//            // if the test invoke RowViewModelProvider.Get(column, selectedMenuInsight) with the column semantically equal to "fakeSecondFiltersColumns" ...
//            rowViewModelProviderMock.Setup(
//                x => x.Get(
//                    It.Is<XElement>(arg => xmlComparer.DeepEquals(arg, fakeSecondFiltersColumns, null)),
//                    fakeSelectedMenuInsight.Id))
//                    .Returns(fakeColumnsReturnedFromTheDb[1]);

//            #endregion

//            CanvasViewModel insightsViewModel = new CanvasViewModel(insightsAccessMock.Object, rowViewModelProviderMock.Object);

//            // Act
//            insightsViewModel.LoadFiltersCommand.Execute(fakeSelectedMenuInsight);

//            // Assert

//            // Is selected menu insight assigned to SelectedMenuInsight property
//            Assert.AreEqual(fakeSelectedMenuInsight, insightsViewModel.SelectedInsight, "Selected menu insight is not assigned to SelectedMenuInsight property");

//            // Is calling fake database to load filters
//            insightsAccessMock.Verify(x => x.GetFilters(fakeSelectedMenuInsight.Id), "Insights page is not calling mock db to get filters (at least not with correct arg)");

//            // Is number of loaded columns the same as number of root items of the xml returned from the fake database
//            var expectedNumberOfColumns = fakeFiltersReturnedFromDb.Elements("RootItem").Count();
//            var numberOfLoadedColumns = insightsViewModel.FiltersColumns.Count;
//            Assert.AreEqual(expectedNumberOfColumns, numberOfLoadedColumns, "Incorrect number of loaded filters columns");

//            // Is number of loaded filters the same as the number of attribute items of the xml returned from the fake database
//            var expectedNumberOfFilters = fakeFiltersReturnedFromDb.Elements("RootItem").Elements("Attributes").Elements("Attribute").Count();
//            var numberOfLoadedFilters = insightsViewModel.FiltersColumns.SelectMany(x => x.Records).SelectMany(y => y.Properties).Count();
//            Assert.AreEqual(expectedNumberOfFilters, numberOfLoadedFilters, "Incorrect number of loaded filters");

//            // Is calling fake row view model provider to load filters data
//            rowViewModelProviderMock.Verify(x => x.Get(
//                It.Is<XElement>(arg => xmlComparer.DeepEquals(arg, fakeFirstFiltersColumns, null)),
//                    fakeSelectedMenuInsight.Id), 
//                    "");

//            rowViewModelProviderMock.Verify(x => x.Get(
//                It.Is<XElement>(arg => xmlComparer.DeepEquals(arg, fakeSecondFiltersColumns, null)),
//                    fakeSelectedMenuInsight.Id), 
//                    "");

//            //TODO: test below
//            //ImplementInterColumnDependency(columnsViewModels);
//        }
//    }
//}
