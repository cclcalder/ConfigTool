using System.Linq;
using Exceedra.Controls.DynamicRow.Models;
using Exceedra.Controls.DynamicRow.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Entity.Listings;
using SalesPlannerWeb.Models;

namespace SalesPlannerWeb.Test
{
    [TestClass]
    public class ListingTests
    {
        [TestMethod]
        public void Validate_ProvidedValidListing_ReturnsNoErrors()
        {
            // Arrange
            var target = new Listing
            {
                Details = new RowViewModel
                {
                    Records = new ObservableRangeCollection<RowRecord>
                    {
                        new RowRecord
                        {
                            Properties = new ObservableRangeCollection<RowProperty>
                            {
                                // ControlType mustn't be null otherwise there are exceptions being thrown
                                new RowProperty { ControlType = string.Empty, IsRequired = false, Value = "Random property value" },
                                new RowProperty { ControlType = string.Empty, IsRequired = false, Value = string.Empty },
                                new RowProperty { ControlType = string.Empty, IsRequired = true, Value = "Another property value" }
                            }
                        }
                    }
                },
                ProductsRoot = new TreeViewHierarchy
                {
                    IsSelectedBool = false,
                    Children = new MTObservableCollection<TreeViewHierarchy>
                    {
                        new TreeViewHierarchy { IsSelectedBool = false },
                        new TreeViewHierarchy { IsSelectedBool = false },
                        new TreeViewHierarchy { IsSelectedBool = true },
                        new TreeViewHierarchy { IsSelectedBool = false }
                    }
                }
            };

            // Act
            var result = target.Validate(null);

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Validate_ProvidedDetailsWithOneRequiredPropertyEmpty_ReturnsRequiredDetailsUnfulfilledErrorMessage()
        {
            // Arrange
            var target = new Listing
            {
                Details = new RowViewModel
                {
                    Records = new ObservableRangeCollection<RowRecord>
                    {
                        new RowRecord
                        {
                            Properties = new ObservableRangeCollection<RowProperty>
                            {
                                // ControlType mustn't be null otherwise there are exceptions being thrown
                                new RowProperty { ControlType = string.Empty, IsRequired = true, Value = string.Empty }
                            }
                        }
                    }
                },
                ProductsRoot = new TreeViewHierarchy()
            };

            // Act
            var result = target.Validate(null);

            // Assert
            Assert.IsTrue(result.Any(x => x.ErrorMessage == target.RequiredDetailsUnfulfilledErrorMessage));
        }

        [TestMethod]
        public void Validate_ProvidedDetailsWithOneRequiredPropertyFulfilled_DoesNotReturnRequiredDetailsUnfulfilledErrorMessage()
        {
            // Arrange
            var target = new Listing
            {
                Details = new RowViewModel
                {
                    Records = new ObservableRangeCollection<RowRecord>
                    {
                        new RowRecord
                        {
                            Properties = new ObservableRangeCollection<RowProperty>
                            {
                                new RowProperty { ControlType = string.Empty, IsRequired = true, Value = "Random property value" }
                            }
                        }
                    }
                },
                ProductsRoot = new TreeViewHierarchy()
            };

            // Act
            var result = target.Validate(null);

            // Assert
            Assert.IsFalse(result.Any(x => x.ErrorMessage == target.RequiredDetailsUnfulfilledErrorMessage));
        }

        [TestMethod]
        public void Validate_ProvidedDetailsWithOneNonRequiredPropertyEmpty_DoesNotReturnRequiredDetailsUnfulfilledErrorMessage()
        {
            // Arrange
            var target = new Listing
            {
                Details = new RowViewModel
                {
                    Records = new ObservableRangeCollection<RowRecord>
                    {
                        new RowRecord
                        {
                            Properties = new ObservableRangeCollection<RowProperty>
                            {
                                new RowProperty { ControlType = string.Empty, IsRequired = false, Value = string.Empty }
                            }
                        }
                    }
                },
                ProductsRoot = new TreeViewHierarchy()
            };

            // Act
            var result = target.Validate(null);

            // Assert
            Assert.IsFalse(result.Any(x => x.ErrorMessage == target.RequiredDetailsUnfulfilledErrorMessage));
        }

        [TestMethod]
        public void Validate_ProvidedProductsWithoutSelectedProduct_ReturnsNoSelectedProductErrorMessage()
        {
            // Arrange
            var target = new Listing
            {
                Details = new RowViewModel(),
                ProductsRoot = new TreeViewHierarchy { IsSelectedBool = false }
            };

            // Act
            var result = target.Validate(null);

            // Assert
            Assert.IsTrue(result.Any(x => x.ErrorMessage == target.NoSelectedProductErrorMessage));
        }

        [TestMethod]
        public void Validate_ProvidedProductsWithSelectedProduct_DoesNotReturnNoSelectedProductErrorMessage()
        {
            // Arrange
            var target = new Listing
            {
                Details = new RowViewModel(),
                ProductsRoot = new TreeViewHierarchy { IsSelectedBool = true }
            };

            // Act
            var result = target.Validate(null);

            // Assert
            Assert.IsFalse(result.Any(x => x.ErrorMessage == target.NoSelectedProductErrorMessage));
        }
    }
}
