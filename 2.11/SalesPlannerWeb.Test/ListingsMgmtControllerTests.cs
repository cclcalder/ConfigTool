using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Xml.Linq;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.DynamicRow.ViewModels;
using Exceedra.SingleSelectCombo.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Entity;
using Model.Entity.Listings;
using Moq;
using SalesPlannerWeb.Accessors;
using SalesPlannerWeb.Controllers;
using SalesPlannerWeb.Models;
using Listing = SalesPlannerWeb.Models.Listing;

namespace SalesPlannerWeb.Test
{
    [TestClass]
    public class ListingsMgmtControllerTests
    {
        private const string ActionResponseKey = "ActionResponse";

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            User.CurrentUser = new User { ID = "1", SalesOrganisationID = 1, LanguageCode = "en-GB" };
            WebConfiguration.Configuration = new ClientConfiguration(new List<Feature>(), new List<ROBScreen>(), new List<Screen>(), null, new Dictionary<string, string>(), new Dictionary<string, string>());
        }

        [TestMethod]
        public void Index_Invoked_CallsAccessorToGetListings()
        {
            // Arrange
            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.GetListings()).Returns(() => null);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            target.Index();

            // Assert
            dbMock.Verify(mock => mock.GetListings());
        }

        [TestMethod]
        public void Index_Invoked_ReturnsViewThatIsNotNull()
        {
            // Arrange
            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.GetListings()).Returns(() => null);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            var result = target.Index();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Index_Invoked_ReturnsViewWithModelThatIsNotNull()
        {
            // Arrange
            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.GetListings()).Returns(() => new RecordViewModel());

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            var result = target.Index();

            // Assert
            Assert.IsNotNull(result.Model);
        }

        [TestMethod]
        public void Index_Invoked_ReturnsIndexView()
        {
            // Arrange
            // As we want the Index action to render the default view the view doesn't have any name.
            var expectedViewName = string.Empty;

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.GetListings()).Returns(() => new RecordViewModel());

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            var result = target.Index();

            // Assert
            Assert.AreEqual(expectedViewName, result.ViewName);
        }

        [TestMethod]
        public void Index_Invoked_ReturnsIndexViewWithModelOfTypeRecordViewModel()
        {
            // Arrange
            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.GetListings()).Returns(() => new RecordViewModel());

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            var result = target.Index();

            // Assert
            Assert.IsInstanceOfType(result.Model, typeof(RecordViewModel));
        }

        [TestMethod]
        public void Index_Invoked_ReturnsIndexViewWithListingsAsModel()
        {
            // Arrange
            var expectedListings = new RecordViewModel();

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.GetListings()).Returns(() => expectedListings);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            var result = target.Index();

            // Assert
            Assert.AreEqual(expectedListings, result.Model);
        }

        [TestMethod]
        public void GetList_Invoked_CallsAccessorToGetListingsSendingFilters()
        {
            // Arrange
            var xFilters = new XElement("Filters", string.Empty);

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.GetListings(xFilters)).Returns(() => null);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            target.GetList(xFilters);

            // Assert
            dbMock.Verify(mock => mock.GetListings(xFilters));
        }

        [TestMethod]
        public void GetList_Invoked_ReturnsIndexView()
        {
            // Arrange
            var expectedViewName = "Index";

            var xFilters = new XElement("Filters", string.Empty);

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.GetListings(xFilters)).Returns(() => new RecordViewModel());

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            var result = target.GetList(xFilters);

            // Assert
            Assert.AreEqual(expectedViewName, result.ViewName);
        }

        [TestMethod]
        public void GetList_Invoked_ReturnsIndexViewWithListingsAsModel()
        {
            // Arrange
            var expectedListings = new RecordViewModel();

            var xFilters = new XElement("Filters", string.Empty);

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.GetListings(xFilters)).Returns(() => expectedListings);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            var result = target.GetList(xFilters);

            // Assert
            Assert.AreEqual(expectedListings, result.Model);
        }

        [TestMethod]
        public void NewOrEdit_InvokedWithExistingListingIdx_CallsAccessorToGetListingDetails()
        {
            // Arrange 
            string listingIdx = "20039$15088";

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.GetListingDetails(listingIdx)).Returns(() => null);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            target.NewOrEdit(listingIdx);

            // Assert           
            dbMock.Verify(mock => mock.GetListingDetails(listingIdx));
        }

        [TestMethod]
        public void NewOrEdit_InvokedWithExistingListingIdx_CallsAccessorToGetListingCustomer()
        {
            // Arrange 
            string listingIdx = "20039$15088";

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.GetListingCustomerDropdown(listingIdx)).Returns(() => null);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            target.NewOrEdit(listingIdx);

            // Assert           
            dbMock.Verify(mock => mock.GetListingCustomerDropdown(listingIdx));
        }

        [TestMethod]
        public void NewOrEdit_InvokedWithExistingListingIdx_CallsAccessorToGetListingProduct()
        {
            // Arrange 
            string listingIdx = "20039$15088";

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.GetListingProductsRootNode(listingIdx)).Returns(() => null);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            target.NewOrEdit(listingIdx);

            // Assert           
            dbMock.Verify(mock => mock.GetListingProductsRootNode(listingIdx));
        }

        [TestMethod]
        public void NewOrEdit_InvokedWithExistingListingIdx_ReturnsPartialViewThatsIsNotNull()
        {
            // Arrange
            string listingIdx = "20039$15088";

            var dbMock = new Mock<IListingsMgmtAccessor>();

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            var result = target.NewOrEdit(listingIdx);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void NewOrEdit_InvokedWithExistingListingIdx_ReturnsPartialViewWithModelThatIsNotNull()
        {
            // Arrange
            string listingIdx = "20039$15088";

            var dbMock = new Mock<IListingsMgmtAccessor>();

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            var result = target.NewOrEdit(listingIdx);

            // Assert
            Assert.IsNotNull(result.Model);
        }

        [TestMethod]
        public void NewOrEdit_InvokedWithExistingListingIdx_ReturnsListingEditor()
        {
            // Arrange
            string listingIdx = "20039$15088";

            var dbMock = new Mock<IListingsMgmtAccessor>();

            var expectedPartialViewName = "_ListingsMgmtEditorView";
            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            var result = target.NewOrEdit(listingIdx);
            var resultPartialViewName = result.ViewName;

            // Assert
            Assert.AreEqual(expectedPartialViewName, resultPartialViewName);
        }

        [TestMethod]
        public void NewOrEdit_InvokedWithExistingListingIdx_ReturnsListingEditorWithModelOfTypeListing()
        {
            // Arrange
            string listingIdx = "20039$15088";

            var dbMock = new Mock<IListingsMgmtAccessor>();

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            var result = target.NewOrEdit(listingIdx);

            // Assert
            Assert.IsInstanceOfType(result.Model, typeof(Listing));
        }

        [TestMethod]
        public void NewOrEdit_InvokedWithExistingListingIdx_ReturnsListingEditorWithModelFilledWithData()
        {
            // Arrange
            string listingIdx = "20039$15088";

            var expectedListingDetails = new RowViewModel();
            var expectedListingCustomer = new SingleSelectViewModel();
            var expectedListingProducts = new TreeViewHierarchy();

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.GetListingDetails(listingIdx)).Returns(expectedListingDetails);
            dbMock.Setup(mock => mock.GetListingCustomerDropdown(listingIdx)).Returns(expectedListingCustomer);
            dbMock.Setup(mock => mock.GetListingProductsRootNode(listingIdx)).Returns(expectedListingProducts);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            var result = target.NewOrEdit(listingIdx);

            // Assert
            Assert.AreEqual(expectedListingDetails, ((Listing)result.Model).Details);
            Assert.AreEqual(expectedListingCustomer, ((Listing)result.Model).Customers);
            Assert.AreEqual(expectedListingProducts, ((Listing)result.Model).ProductsRoot);
        }

        [TestMethod]
        public void NewOrEdit_InvokedWithExistingListingIdx_AssignsListingIdxToViewBag()
        {
            // Arrange
            string listingIdx = "20039$15088";

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.GetListingDetails(listingIdx)).Returns(() => null);
            dbMock.Setup(mock => mock.GetListingCustomerDropdown(listingIdx)).Returns(() => null);
            dbMock.Setup(mock => mock.GetListingProductsRootNode(listingIdx)).Returns(() => null);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            target.NewOrEdit(listingIdx);

            // Assert
            Assert.AreEqual(target.ViewBag.ListingIdx, listingIdx);
        }

        [TestMethod]
        public void NewOrEdit_InvokedWithEmptyListingIdx_CallsAccessorToGetEmptyListingDetails()
        {
            // Arrange 
            string listingIdx = string.Empty;

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.GetListingDetails(listingIdx)).Returns(() => null);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            target.NewOrEdit(listingIdx);

            // Assert
            dbMock.Verify(mock => mock.GetListingDetails(listingIdx));
        }

        [TestMethod]
        public void NewOrEdit_InvokedWithEmptyListingIdx_CallsAccessorToGetEmptyListingCustomerDropdown()
        {
            // Arrange 
            string listingIdx = string.Empty;

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.GetListingCustomerDropdown(listingIdx)).Returns(() => null);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            target.NewOrEdit(listingIdx);

            // Assert
            dbMock.Verify(mock => mock.GetListingCustomerDropdown(listingIdx));
        }

        [TestMethod]
        public void NewOrEdit_InvokedWithEmptyListingIdx_CallsAccessorToGetEmptyListingProductsTree()
        {
            // Arrange 
            string listingIdx = string.Empty;

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.GetListingProductsRootNode(listingIdx)).Returns(() => null);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            target.NewOrEdit(listingIdx);

            // Assert
            dbMock.Verify(mock => mock.GetListingProductsRootNode(listingIdx));
        }

        [TestMethod]
        public void NewOrEdit_InvokedWithEmptyListingIdx_ReturnsListingEditor()
        {
            // Arrange
            string listingIdx = string.Empty;

            var dbMock = new Mock<IListingsMgmtAccessor>();

            var expectedPartialViewName = "_ListingsMgmtEditorView";
            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            var result = target.NewOrEdit(listingIdx);

            // Assert
            Assert.AreEqual(expectedPartialViewName, result.ViewName);
        }

        [TestMethod]
        public void NewOrEdit_InvokedWithEmptyListingIdx_ReturnsListingEditorWithModelOfTypeListing()
        {
            // Arrange
            string listingIdx = string.Empty;

            var dbMock = new Mock<IListingsMgmtAccessor>();

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            var result = target.NewOrEdit(listingIdx);

            // Assert
            Assert.IsInstanceOfType(result.Model, typeof(Listing));
        }

        [TestMethod]
        public void Remove_InvokedWithExistingListingIdx_CallsAccessorToRemoveListing()
        {
            // Arrange 
            string listingIdx = "20039$15088";

            var dbResponse = new Message
            {
                Type = MessageType.Success,
                Text = "Listing has been successfully removed!"
            };

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.RemoveListing(listingIdx)).Returns(() => dbResponse);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            target.Remove(listingIdx);

            // Assert           
            dbMock.Verify(mock => mock.RemoveListing(listingIdx));
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void Remove_InvokedWithNullListingIdx_ThrowsInvalidOperationException()
        {
            // Arrange 
            string listingIdx = null;

            var dbMock = new Mock<IListingsMgmtAccessor>();
            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull
            target.Remove(listingIdx);

            // Assert           
            Assert.Fail();
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void Remove_InvokedWithEmptyListingIdx_ThrowsInvalidOperationException()
        {
            // Arrange 
            string listingIdx = string.Empty;

            var dbMock = new Mock<IListingsMgmtAccessor>();
            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            target.Remove(listingIdx);

            // Assert           
            Assert.Fail();
        }

        [TestMethod]
        public void Remove_ListingSuccessfullyRemoved_AssignsDbResponseIntoTempData()
        {
            // Arrange 
            string listingIdx = "20039$15088";

            var expectedDbResponse = new Message
            {
                Type = MessageType.Success,
                Text = "Listing has been successfully removed!"
            };

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.RemoveListing(listingIdx)).Returns(() => expectedDbResponse);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            target.Remove(listingIdx);

            // Assert           
            Assert.AreSame(expectedDbResponse, target.TempData[ActionResponseKey]);
        }

        [TestMethod]
        public void Remove_ListingFailedToRemove_DoesNotAssignDbResponseIntoTempData()
        {
            // Arrange 
            string listingIdx = "20039$15088";

            var expectedDbResponse = new Message
            {
                Type = MessageType.Warning,
                Text = "Failed to remove listing!"
            };

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.RemoveListing(listingIdx)).Returns(() => expectedDbResponse);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            target.Remove(listingIdx);

            // Assert           
            Assert.IsNull(target.TempData[ActionResponseKey]);
        }

        [TestMethod]
        public void Remove_ListingSuccessfullyRemoved_ReturnsDbResponseAsJson()
        {
            // Arrange 
            string listingIdx = "20039$15088";

            var expectedDbResponse = new Message
            {
                Type = MessageType.Success,
                Text = "Listing has been successfully removed!"
            };

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(mock => mock.RemoveListing(listingIdx)).Returns(() => expectedDbResponse);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            var result = target.Remove(listingIdx);

            // Assert           
            Assert.AreSame(expectedDbResponse, result.Data);
        }

        [TestMethod]
        public void Save_ValidListingSent_CallsAccessorToSaveListing()
        {
            // Arrange
            Listing listing = new Listing();

            var expectedDbResponse = new Message
            {
                Type = MessageType.Success,
                Text = "Saved successfully!"
            };

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(x => x.SaveListing(listing)).Returns(expectedDbResponse);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            target.Save(listing);

            // Assert
            dbMock.Verify(mock => mock.SaveListing(listing));
        }

        [TestMethod]
        public void Save_ListingSavedSuccessfully_AssignsSaveResponseInTempData()
        {
            // Arrange
            Listing listing = new Listing();

            var expectedDbResponse = new Message
            {
                Type = MessageType.Success,
                Text = "Saved successfully!"
            };

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(x => x.SaveListing(listing)).Returns(expectedDbResponse);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            target.Save(listing);

            // Assert
            Assert.AreSame(expectedDbResponse, target.TempData[ActionResponseKey]);
        }

        [TestMethod]
        public void Save_ListingFailedToSave_DoesNotAssignSaveResponseInTempData()
        {
            // Arrange
            Listing listing = new Listing();

            var expectedDbResponse = new Message
            {
                Type = MessageType.Warning,
                Text = "Save failed!"
            };

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(x => x.SaveListing(listing)).Returns(expectedDbResponse);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            target.Save(listing);

            // Assert
            Assert.IsNull(target.TempData[ActionResponseKey]);
        }

        [TestMethod]
        public void Save_Invoked_ReturnsNonEmptyResult()
        {
            // Arrange
            Listing listing = new Listing();

            var expectedDbResponse = new Message
            {
                Type = MessageType.Success,
                Text = "Saved successfully!"
            };

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(x => x.SaveListing(listing)).Returns(expectedDbResponse);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            var result = target.Save(listing);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Save_Invoked_ReturnsJson()
        {
            // Arrange
            Listing listing = new Listing();

            var expectedDbResponse = new Message
            {
                Type = MessageType.Success,
                Text = "Saved successfully!"
            };

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(x => x.SaveListing(listing)).Returns(expectedDbResponse);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            var result = target.Save(listing);

            // Assert
            Assert.IsInstanceOfType(result, typeof(JsonResult));
        }

        [TestMethod]
        public void Save_ValidListingSent_ReturnsSaveResponseAsJson()
        {
            // Arrange
            Listing listing = new Listing();

            var expectedDbResponse = new Message
            {
                Type = MessageType.Success,
                Text = "Saved successfully!"
            };

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(x => x.SaveListing(listing)).Returns(expectedDbResponse);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            // Act
            var result = target.Save(listing);

            // Assert
            Assert.AreSame(expectedDbResponse, result.Data);
        }

        [TestMethod]
        public void Save_InvalidListingSent_DoesNotCallAccessorToSaveListing()
        {
            // Arrange
            Listing listing = new Listing();

            var dbMock = new Mock<IListingsMgmtAccessor>();
            dbMock.Setup(x => x.SaveListing(listing)).Returns(() => null);

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);
            target.ModelState.AddModelError("ValidationError", "Validation error description");

            // Act
            target.Save(listing);

            // Assert
            dbMock.Verify(mock => mock.SaveListing(listing), Times.Never);
        }

        [TestMethod]
        public void Save_InvalidListingSent_DoesNotAssignSaveResponseInTempData()
        {
            // Arrange
            Listing listing = new Listing();

            var dbMock = new Mock<IListingsMgmtAccessor>();

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);
            target.ModelState.AddModelError("ValidationError", "Validation error description");

            // Act
            target.Save(listing);

            // Assert
            Assert.IsNull(target.TempData[ActionResponseKey]);
        }

        [TestMethod]
        public void Save_InvalidListingSent_ReturnsJsonWithDataOfTypeMessage()
        {
            // Arrange
            Listing listing = new Listing();

            var dbMock = new Mock<IListingsMgmtAccessor>();

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);
            target.ModelState.AddModelError("ValidationError", "Validation error description");

            // Act
            var result = target.Save(listing);

            // Assert
            Assert.IsInstanceOfType(result.Data, typeof(Message));
        }

        [TestMethod]
        public void Save_InvalidListingSent_ReturnsWarningMessageAsJson()
        {
            // Arrange
            Listing listing = new Listing();

            var dbMock = new Mock<IListingsMgmtAccessor>();

            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);
            target.ModelState.AddModelError("ValidationError", "Validation error description");

            // Act
            var result = target.Save(listing);

            // Assert
            var resultMessage = result.Data as Message;
            if (resultMessage == null)
                Assert.Fail();

            Assert.AreEqual(MessageType.Warning, resultMessage.Type);
        }

        [TestMethod]
        public void Save_InvalidListingSent_ReturnsErrorsDescriptionsAsJson()
        {
            // Arrange
            Listing listing = new Listing();

            var dbMock = new Mock<IListingsMgmtAccessor>();
            ListingsMgmtController target = new ListingsMgmtController(dbMock.Object);

            var expectedErrorDescription = "Validation error description";
            target.ModelState.AddModelError("ValidationError", expectedErrorDescription);

            // Act
            var result = target.Save(listing);

            // Assert
            var resultMessage = result.Data as Message;
            if (resultMessage == null)
                Assert.Fail();

            Assert.AreEqual(expectedErrorDescription, resultMessage.Text);
        }
    }
}
