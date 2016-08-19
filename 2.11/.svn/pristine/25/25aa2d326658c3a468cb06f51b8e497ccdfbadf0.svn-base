using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.DataAccess;
using Model.Entity.ROBs;
using Moq;

namespace WPF.Test.RobContracts
{
    [TestClass]
    public class RobContractsEditorTests
    {
        public string TestAppTypeId = "9999";
        public string NewRobId = string.Empty;

        //[ClassInitialize]
        //public static void RobContractsEditorTestsInitialize(TestContext testContext)
        //{
        //    User.CurrentUser = new User { ID = "47" };
        //}

        [TestMethod]
        public void GetFilterCustomersProc_GetAvailableCustomersInvoked_Called()
        {
            // Arrange 
            var robAccessMock = new Mock<IRobAccess>();

            RobContractsEditorModel target = new RobContractsEditorModel(robAccessMock.Object, TestAppTypeId, NewRobId);

            // Act 
            target.GetAvailableCustomers();

            // Assert
            robAccessMock.Verify(rob => rob.GetFilterCustomers(TestAppTypeId, NewRobId));
        }

        [TestMethod]
        public void AvailableCustomers_GetAvailableCustomersInvoked_Assigned()
        {
            // Arrange 
            var robAccessMock = new Mock<IRobAccess>();

            IList<Customer> expected = new List<Customer>
            {
                new Customer{ID = "1"},
                new Customer{ID = "2"},
                new Customer{ID = "3"}
            };

            robAccessMock.Setup(rob => rob.GetFilterCustomers(TestAppTypeId, NewRobId))
                .Returns(Task.FromResult(expected));

            RobContractsEditorModel target = new RobContractsEditorModel(robAccessMock.Object, TestAppTypeId, NewRobId);

            // Act 
            var result = target.GetAvailableCustomers();

            // Assert
            result.ContinueWith(t =>
            {
                CollectionAssert.AreEquivalent(expected.ToList(), target.AvailableCustomers);
            }).Wait();
        }

        [TestMethod]
        public void GetTypesProc_GetAvailableCustomersInvoked_Called()
        {
            // Arrange 
            var robAccessMock = new Mock<IRobAccess>();

            RobContractsEditorModel target = new RobContractsEditorModel(robAccessMock.Object, TestAppTypeId, NewRobId);

            // Act
            target.GetAvailableTermTypes();

            // Assert
            robAccessMock.Verify(rob => rob.GetTypes(TestAppTypeId, NewRobId));
        }

        [TestMethod]
        public void AvailableTermTypes_GetAvailableTermTypesInvoked_Assigned()
        {
            // Arrange 
            var robAccessMock = new Mock<IRobAccess>();

            IList<ROBType> expected = new List<ROBType>
            {
                new ROBType{ID = "0", Name = "Cool rob"},
                new ROBType{ID = "1", Name = "Even more cooler rob"},
                new ROBType{ID = "2", Name = "The rob that is so cool that I hardly believe in it"}
            };

            robAccessMock.Setup(rob => rob.GetTypes(TestAppTypeId, NewRobId))
                .Returns(Task.FromResult(expected));

            RobContractsEditorModel target = new RobContractsEditorModel(robAccessMock.Object, TestAppTypeId, NewRobId);

            // Act 
            var result = target.GetAvailableTermTypes();

            // Assert
            result.ContinueWith(t =>
            {
                CollectionAssert.AreEquivalent(expected.ToList(), target.AvailableTermTypes);
            }).Wait();
        }

        [TestMethod]
        public void GetSubTypesProc_GetAvailableTermSubTypesInvoked_Called()
        {
            // Arrange 
            var robAccessMock = new Mock<IRobAccess>();

            IList<ROBSubType> expected = new List<ROBSubType>
            {
                new ROBSubType{ID = "0", Name = "Cool rob"},
                new ROBSubType{ID = "1", Name = "Even more cooler rob"},
                new ROBSubType{ID = "2", Name = "The rob that is so cool that I hardly believe in it"}
            };

            var selectedTermType = new ROBType { ID = "0" };

            robAccessMock.Setup(rob => rob.GetSubTypes(TestAppTypeId, selectedTermType.ID, NewRobId))
                .Returns(Task.FromResult(expected));

            RobContractsEditorModel target = new RobContractsEditorModel(robAccessMock.Object, TestAppTypeId, NewRobId)
            {
                SelectedTermType = selectedTermType
            };

            // Act
            target.GetAvailableTermSubTypes();

            // Assert
            robAccessMock.Verify(rob => rob.GetSubTypes(TestAppTypeId, target.SelectedTermType.ID, NewRobId));
        }

        [TestMethod]
        public void AvailableTermSubTypes_GetAvailableTermSubTypesInvoked_Assigned()
        {
            // Arrange 
            var robAccessMock = new Mock<IRobAccess>();

            IList<ROBSubType> expected = new List<ROBSubType>
            {
                new ROBSubType{ID = "0", Name = "Cool rob"},
                new ROBSubType{ID = "1", Name = "Even more cooler rob"},
                new ROBSubType{ID = "2", Name = "The rob that is so cool that I hardly believe in it"}
            };

            var selectedTermType = new ROBType { ID = "0" };

            robAccessMock.Setup(rob => rob.GetSubTypes(TestAppTypeId, selectedTermType.ID, NewRobId))
                .Returns(Task.FromResult(expected));

            RobContractsEditorModel target = new RobContractsEditorModel(robAccessMock.Object, TestAppTypeId, NewRobId)
            {
                SelectedTermType = selectedTermType
            };

            // Act 
            var result = target.GetAvailableTermSubTypes();

            // Assert
            result.ContinueWith(t =>
            {
                CollectionAssert.AreEquivalent(expected.ToList(), target.AvailableTermSubTypes);
            }).Wait();
        }

        [TestMethod]
        public void AvailableTermSubTypes_TermTypeSelected_Loaded()
        {
            // Arrange 
            var robAccessMock = new Mock<IRobAccess>();

            ROBType selectedTermType = new ROBType
            {
                ID = "0",
                IsSelected = true,
                Name = "Coca-Cola. Since 1886"
            };

            IList<ROBSubType> expected = new List<ROBSubType>
            {
                new ROBSubType{ID = "0", Name = "Cool rob"},
                new ROBSubType{ID = "1", Name = "Even more cooler rob"},
                new ROBSubType{ID = "2", Name = "The rob that is so cool that I hardly believe in it"}
            };

            robAccessMock.Setup(rob => rob.GetSubTypes(TestAppTypeId, selectedTermType.ID, NewRobId))
                .Returns(Task.FromResult(expected));

            RobContractsEditorModel target = new RobContractsEditorModel(robAccessMock.Object, TestAppTypeId, NewRobId);

            // Act 
            var act = Task.Factory.StartNew(() =>
            {
                target.SelectedTermType = selectedTermType;
            });

            // Assert
            act.ContinueWith(t =>
            {
                CollectionAssert.AreEquivalent(expected.ToList(), target.AvailableTermSubTypes);
            }).Wait();
        }

        [TestMethod]
        public void GetProductsProc_GetAvailableProductsInvoked_Called()
        {
            // Arrange 
            var robAccessMock = new Mock<IRobAccess>();

            List<string> customersId = new List<string> { "0", "1" };

            RobContractsEditorModel target = new RobContractsEditorModel(robAccessMock.Object, TestAppTypeId, NewRobId)
            {
                SelectedCustomers = new List<Customer>
                {
                    new Customer{ID = customersId[0]},
                    new Customer{ID = customersId[1]}
                }
            };

            // Act 
            target.GetAvailableProducts();

            // Assert
            robAccessMock.Verify(rob => rob.GetFilterProducts(TestAppTypeId, customersId));
        }

        [TestMethod]
        public void AvailableProducts_GetAvailableProductsInvoked_Assigned()
        {
            // Arrange 
            var robAccessMock = new Mock<IRobAccess>();

            List<string> customersId = new List<string> { "0", "1" };

            IList<Product> expected = new List<Product>
            {
                new Product{ID = "0"},
                new Product{ID = "1"},
                new Product{ID = "2"}
            };

            robAccessMock.Setup(rob => rob.GetFilterProducts(TestAppTypeId, customersId))
                .Returns(Task.FromResult(expected));

            RobContractsEditorModel target = new RobContractsEditorModel(robAccessMock.Object, TestAppTypeId, NewRobId)
            {
                SelectedCustomers = new List<Customer>
                {
                    new Customer{ID = customersId[0]},
                    new Customer{ID = customersId[1]}
                }
            };

            // Act 
            var result = target.GetAvailableProducts();

            // Assert
            result.ContinueWith(t =>
            {
                CollectionAssert.AreEquivalent(expected.ToList(), target.AvailableProducts);
            }).Wait();
        }

        [TestMethod]
        public void AvailableProducts_SelectedCustomersChanged_LoadedFromTheDb()
        {
            // Arrange 
            var robAccessMock = new Mock<IRobAccess>();

            List<string> customersId = new List<string> { "0", "1" };

            List<Customer> selectedCustomers = new List<Customer>
            {
                new Customer{ID = customersId[0]},
                new Customer{ID = customersId[1]}
            };

            IList<Product> expected = new List<Product>
            {
                new Product{ID = "0"},
                new Product{ID = "1"},
                new Product{ID = "2"}
            };

            robAccessMock.Setup(rob => rob.GetFilterProducts(TestAppTypeId, customersId))
                .Returns(Task.FromResult(expected));

            RobContractsEditorModel target = new RobContractsEditorModel(robAccessMock.Object, TestAppTypeId, NewRobId);

            // Act 
            var act = Task.Factory.StartNew(() =>
            {
                target.SelectedCustomers = selectedCustomers;
            });

            // Assert
            act.ContinueWith(t =>
            {
                CollectionAssert.AreEquivalent(expected.ToList(), target.AvailableProducts);
            }).Wait();
        }

        [TestMethod]
        public void SelectedCustomers_AvailableCustomersAssigned_ReloadedBasingOnIsSelected()
        {
            // Arrange
            var robAccessMock = new Mock<IRobAccess>();

            Customer fakeCustomer1 = new Customer{ID = "0", IsSelected = true};
            Customer fakeCustomer2 = new Customer{ID = "0"};
            Customer fakeCustomer3 = new Customer{ID = "0", IsSelected = true};
            Customer fakeCustomer4 = new Customer{ID = "0"};

            List<Customer> fakeAvailableCustomers = new List<Customer>
            {
                fakeCustomer1, fakeCustomer2, fakeCustomer3, fakeCustomer4
            };

            List<Customer> expectedSelectedCustomers = new List<Customer>
            {
                fakeCustomer1, fakeCustomer3
            };

            RobContractsEditorModel target = new RobContractsEditorModel(robAccessMock.Object, TestAppTypeId, NewRobId);

            // Act
            target.AvailableCustomers = fakeAvailableCustomers;

            // Assert
            CollectionAssert.AreEquivalent(expectedSelectedCustomers, target.SelectedCustomers);

        }

        //[TestMethod]
        //public void SelectedTermType_GetAvailableTermTypesInvoked_SetToFirstAvailable()
        //{
        //    // Arrange 
        //    var robAccessMock = new Mock<IRobAccess>();

        //    ROBType expected = new ROBType { ID = "0", Name = "RobToBeSelected" };

        //    IList<ROBType> fakeRobs = new List<ROBType>
        //    {
        //        expected,
        //        new ROBType{ID = "1", Name = "Cool rob"},
        //        new ROBType{ID = "2", Name = "Even more cooler rob"},
        //        new ROBType{ID = "3", Name = "The rob that is so cool that I hardly believe in it"}
        //    };

        //    robAccessMock.Setup(rob => rob.GetTypes(TestAppTypeId, NewRobId))
        //        .Returns(Task.FromResult(fakeRobs));

        //    RobContractsEditorModel target = new RobContractsEditorModel(robAccessMock.Object, TestAppTypeId, NewRobId);

        //    // Act 
        //    var result = target.GetAvailableTermTypes();

        //    // Assert
        //    result.ContinueWith(t =>
        //    {
        //        Assert.AreEqual(expected, target.SelectedTermType);
        //    }).Wait();
        //}
    }
}
