using System;
using System.Linq;
using Xunit;

namespace WPF.Test
{
    using System.Threading;
    using ViewModels.Pricing;

    public class PricingViewModelTest
    {
        [Fact]
        public void NewViewModelShouldHaveCustomerList()
        {
            var target = PricingViewModel.New(new StubPricingAccess());
            Assert.NotNull(target.Customers);
            Assert.Equal(3, target.Customers.Count());
        }

        [Fact]
        public void NewViewModelShouldHaveItemList()
        {
            var target = PricingViewModel.New(new StubPricingAccess());
            Assert.NotNull(target.Items);
            Assert.Equal(3, target.Items.Count());
        }

        [Fact]
        public void NewViewModelShouldHaveScenarioList()
        {
            var target = PricingViewModel.New(new StubPricingAccess());
            Assert.NotNull(target.Scenarios);
            Assert.Equal(3, target.Scenarios.Count());
        }


        [Fact]
        public void SettingSelectedItemShouldRaisePropertyChanged()
        {
            var target = PricingViewModel.New(new StubPricingAccess());
            var expectedItem = target.Items.First();

            Assert.True(ActionRaisesPropertyChanged(target, () => target.SelectedItem = expectedItem, "SelectedItem"));
            Assert.Same(expectedItem, target.SelectedItem);
        }

        [Fact]
        public void SettingSelectedScenarioShouldRaisePropertyChanged()
        {
            var target = PricingViewModel.New(new StubPricingAccess());
            var expectedScenario = target.Scenarios.First();

            Assert.True(ActionRaisesPropertyChanged(target, () => target.SelectedScenario = expectedScenario, "SelectedScenario"));
            Assert.Same(expectedScenario, target.SelectedScenario);
        }

        [Fact]
        public void BeforeSelectingCustomerProductsListShouldBeEmpty()
        {
            var target = PricingViewModel.New(new StubPricingAccess());
            Assert.NotNull(target.Products);
            Assert.Equal(0, target.Products.Count());
        }

        [Fact]
        public void SettingSelectedCustomerShouldPopulateProductsList()
        {
            var target = PricingViewModel.New(new StubPricingAccess());
            var customer = target.Customers.First(c => c.ID == "1");
            Assert.True(ActionRaisesPropertyChanged(target, () => target.SelectedCustomer = customer, "Products"));
            Assert.Equal(3, target.Products.Count());
        }

        [Fact]
        public void ClearingSelectedCustomerShouldEmptyProductsList()
        {
            var target = PricingViewModel.New(new StubPricingAccess());
            target.SelectedCustomer = target.Customers.First(c => c.ID == "1");
            Assert.True(ActionRaisesPropertyChanged(target, () => target.SelectedCustomer = null, "Products"));
            Assert.Equal(0, target.Products.Count());
        }

        [Fact]
        public void ApplyCommandNotAvailableWhenNoCustomerIsSelected()
        {
            var target = SetAllProperties();
            target.SelectedCustomer = null;
            Assert.False(target.ApplySelection.CanExecute(null));
        }

        [Fact]
        public void ApplyCommandNotAvailableWhenNoScenarioIsSelected()
        {
            var target = SetAllProperties();
            target.SelectedScenario = null;
            Assert.False(target.ApplySelection.CanExecute(null));
        }

        [Fact]
        public void ApplyCommandNotAvailableWhenNoItemIsSelected()
        {
            var target = SetAllProperties();
            target.SelectedItem = null;
            Assert.False(target.ApplySelection.CanExecute(null));
        }

        [Fact]
        public void ApplyCommandNotAvailableWhenNoProductIsSelected()
        {
            var target = SetAllProperties();
            foreach (var product in target.Products.Where(p => p.IsSelected))
            {
                product.IsSelected = false;
            }
            Assert.False(target.ApplySelection.CanExecute(null));
        }

        [Fact]
        public void ApplyCommandIsAvailableWhenEverythingIsSelected()
        {
            var target = SetAllProperties();
            Assert.True(target.ApplySelection.CanExecute(null));
        }

        [Fact]
        public void ApplyCommandPopulatesItemDetails()
        {
            var target = SetAllProperties();
            target.ApplySelection.Execute(null);
            Assert.NotNull(target.ItemDetails);
            Assert.Equal(target.Products.Count(p => p.IsSelected), target.ItemDetails.Count());
            Assert.True(target.Products.Where(p => p.IsSelected).All(p => target.ItemDetails.Count(id => id.ProductId == p.ID) == 1));
        }

        [Fact]
        public void SettingFromDateSetsItForAllItemDetails()
        {
            var target = SetAllProperties();
            target.ApplySelection.Execute(null);
            var expected = new DateTime(2000, 1, 1);
            target.SetFromDate = expected;
            bool any = false;
            foreach (var itemDetail in target.ItemDetails)
            {
                any = true;
                Assert.Equal(expected, itemDetail.FromDate);
            }
            Assert.True(any);
        }

        [Fact]
        public void SettingToDateSetsItForAllItemDetails()
        {
            var target = SetAllProperties();
            target.ApplySelection.Execute(null);
            var expected = new DateTime(2000, 1, 1);
            target.SetToDate = expected;
            bool any = false;
            foreach (var itemDetail in target.ItemDetails)
            {
                any = true;
                Assert.Equal(expected, itemDetail.ToDate);
            }
            Assert.True(any);
        }

        [Fact]
        public void SettingValueSetsPriceForAllItemDetails()
        {
            var target = SetAllProperties();
            target.ApplySelection.Execute(null);
            const decimal expected = 2m;
            target.SetAllValues = expected;
            bool any = false;
            foreach (var itemDetail in target.ItemDetails)
            {
                any = true;
                Assert.Equal(expected, itemDetail.ModifiedPrice);
            }
            Assert.True(any);
        }

        [Fact]
        public void SettingAdjustValueAdjustsPriceForAllItemDetails()
        {
            var target = SetAllProperties();
            target.ApplySelection.Execute(null);
            const decimal expected = 2.5m;
            target.AdjustAllValues = 150m; // 150% of original value
            bool any = false;
            foreach (var itemDetail in target.ItemDetails)
            {
                any = true;
                Assert.Equal(expected, itemDetail.ModifiedPrice);
            }
            Assert.True(any);
        }

        [Fact]
        public void SettingAdjustValueSecondTimeAdjustsPriceForAllItemDetailsUsingOriginalPrice()
        {
            var target = SetAllProperties();
            target.ApplySelection.Execute(null);
            const decimal expected = 2.75m;
            target.AdjustAllValues = 150m; // 150% of original value
            target.AdjustAllValues = 175m; // 175% of original value
            bool any = false;
            foreach (var itemDetail in target.ItemDetails)
            {
                any = true;
                Assert.Equal(expected, itemDetail.ModifiedPrice);
            }
            Assert.True(any);
        }

        [Fact]
        public void SettingAdjustAllValuesEnablesSave()
        {
            var target = SetAllProperties();
            target.ApplySelection.Execute(null);
            target.AdjustAllValues = 150m; // 150% of original value
            Assert.True(target.Save.CanExecute(null));
        }

        [Fact]
        public void SettingSetAllValuesEnablesSave()
        {
            var target = SetAllProperties();
            target.ApplySelection.Execute(null);
            target.SetAllValues = 54;
            Assert.True(target.Save.CanExecute(null));
        }

        [Fact]
        public void SettingSetFromDateEnablesSave()
        {
            var target = SetAllProperties();
            target.ApplySelection.Execute(null);
            target.SetFromDate = DateTime.MinValue;
            Assert.True(target.Save.CanExecute(null));
        }

        [Fact]
        public void SettingSetToDateEnablesSave()
        {
            var target = SetAllProperties();
            target.ApplySelection.Execute(null);
            target.SetToDate = DateTime.MaxValue;
            Assert.True(target.Save.CanExecute(null));
        }

        [Fact]
        public void SettingAnyValueRaisesSaveCanExecuteChangedEvent()
        {
            var target = SetAllProperties();
            target.ApplySelection.Execute(null);
            bool raised = false;
            target.Save.CanExecuteChanged += (o, e) => raised = true;
            target.AdjustAllValues = 150m; // 150% of original value

            SpinWait.SpinUntil(() => raised, TimeSpan.FromMilliseconds(100));
            Assert.True(raised);
        }

        [Fact]
        public void SaveMethodIsAvailableWhenAllPropertiesSet()
        {
            var pricingAccess = new StubPricingAccess();
            var target = PricingViewModel.New(pricingAccess);
            SetAllProperties(target);
            target.ApplySelection.Execute(null);
            target.SetFromDate = new DateTime(2011,9,1);
            target.SetToDate = new DateTime(2011,9,30);
            target.SetAllValues = 42;
            Assert.True(target.Save.CanExecute(null));
        }

        [Fact]
        public void SaveCommandCallsSaveItemDetails()
        {
            var pricingAccess = new StubPricingAccess();
            var target = PricingViewModel.New(pricingAccess);
            SetAllProperties(target);
            target.ApplySelection.Execute(null);
            target.Save.Execute(null);
            Assert.True(pricingAccess.SaveItemDetailsWasCalled);
        }

        [Fact]
        public void SettingSetAllValuesSetsRadioButtons()
        {
            var target = CreateTargetWithSelectionApplied();

            target.SetAllValues = 1M;
            Assert.True(target.IsSetAllValuesSelected);
            Assert.False(target.IsAdjustAllValuesSelected);
        }

        [Fact]
        public void SettingAdjustAllValuesSetsRadioButtons()
        {
            var target = CreateTargetWithSelectionApplied();

            target.AdjustAllValues = 1M;
            Assert.True(target.IsAdjustAllValuesSelected);
            Assert.False(target.IsSetAllValuesSelected);
        }

        [Fact]
        public void SelectingSetAllValuesClearsAdjust()
        {
            var target = CreateTargetWithSelectionApplied();

            target.AdjustAllValues = 200M;

            target.IsSetAllValuesSelected = true;
            Assert.False(target.IsAdjustAllValuesSelected);
           // Assert.False(target.AdjustAllValues.HasValue);
        }

        [Fact]
        public void SelectingAdjustAllValuesClearsSet()
        {
            var target = CreateTargetWithSelectionApplied();

            target.SetAllValues = 200M;

            target.IsAdjustAllValuesSelected = true;
            Assert.False(target.IsSetAllValuesSelected);
           // Assert.False(target.SetAllValues.HasValue);
        }

        private static PricingViewModel CreateTargetWithSelectionApplied()
        {
            var pricingAccess = new StubPricingAccess();
            var target = PricingViewModel.New(pricingAccess);
            SetAllProperties(target);
            target.ApplySelection.Execute(null);
            return target;
        }

        private static PricingViewModel SetAllProperties()
        {
            var target = PricingViewModel.New(new StubPricingAccess());
            SetAllProperties(target);
            return target;
        }

        private static void SetAllProperties(PricingViewModel target)
        {
            target.SelectedCustomer = target.Customers.First();
            target.SelectedScenario = target.Scenarios.First();
            target.SelectedItem = target.Items.First();
            if (target.Products.Any())
                target.Products.First().IsSelected = true;
        }

        private static bool ActionRaisesPropertyChanged(PricingViewModel target, Action setProperty, string propertyName)
        {
            bool handled = false;
            target.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == propertyName)
                    handled = true;
            };

            setProperty();
            SpinWait.SpinUntil(() => handled, TimeSpan.FromSeconds(0.25));
            return handled;
        }
    }
}
