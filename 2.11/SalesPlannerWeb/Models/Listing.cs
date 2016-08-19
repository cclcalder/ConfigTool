using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Exceedra.Controls.DynamicRow.ViewModels;
using Exceedra.SingleSelectCombo.ViewModel;
using Model.Entity.Listings;

namespace SalesPlannerWeb.Models
{
    public class Listing : IValidatableObject
    {
        public RowViewModel Details { get; set; }
        public SingleSelectViewModel Customers { get; set; }
        public TreeViewHierarchy ProductsRoot { get; set; }

        public string RequiredDetailsUnfulfilledErrorMessage =
            "All required fields (the ones with asterisks) must be fulfilled. Please, provide values where necessary.";

        public string NoSelectedProductErrorMessage =
            "No product selected. Please, select one product from the products tree.";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Details.AreRecordsFulfilled())
                yield return new ValidationResult(RequiredDetailsUnfulfilledErrorMessage);

            var selectedProduct = TreeViewHierarchy.GetFlatTree(ProductsRoot).SingleOrDefault(node => node.IsSelectedBool == true);
            if (selectedProduct == null)
                yield return new ValidationResult(NoSelectedProductErrorMessage);
        }
    }
}