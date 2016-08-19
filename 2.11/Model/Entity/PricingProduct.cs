using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Model.DataAccess;

namespace Model
{
    public class PricingProduct
    {
        private readonly IPricingAccess _pricingAccess;

        /// <summary>
        /// Creates a new Product instance.
        /// </summary>
        public PricingProduct(XElement el, IPricingAccess pricingAccess)
        {
            _pricingAccess = pricingAccess;
            ID = el.GetValue<string>("ID");
            DisplayName = el.GetValue<string>("DisplayName");
            ParentId = el.GetValue<string>("ParentID");
            
            if (el.Element("IsSelected") != null)
            {
                IsSelected = el.GetValue<int>("IsSelected") == 1 ? true : false;
            }

        }

        public PricingProduct(IPricingAccess pricingAccess)
        {
            _pricingAccess = pricingAccess;
        }

        #region ID
        /// <summary>
        /// Gets or sets the Id of this Product.
        /// </summary>
        public string ID { get; set; }
        #endregion

        #region DisplayName
        /// <summary>
        /// Gets or sets the DisplayName of this Product.
        /// </summary>
        public string DisplayName { get; set; }
        #endregion

        #region ParentId
        /// <summary>
        /// Gets or sets the ParentId of this Product.
        /// </summary>
        public string ParentId { get; set; }
        #endregion

        #region IsSelected
        /// <summary>
        /// Gets or sets the IsSelected of this Product.
        /// </summary>
        public bool IsSelected { get; set; }
        #endregion

        #region Children
        /// <summary>
        /// Gets or sets the Children of this Customer.
        /// </summary>
        private List<PricingProduct> _Children;
        public IEnumerable<PricingProduct> Children
        {
            get
            {
                if (_Children == null)
                {
                    if (_pricingAccess == null)
                    {
                        _Children = new List<PricingProduct>();
                    }
                    else
                    {
                        _Children = _pricingAccess.GetProducts(CustomerId).Where(p => p.ParentId == ID).ToList();
                    }
                }
                return _Children;
            }
            internal set { _Children = value.ToList(); }

        }
        #endregion

        #region Parent
        /// <summary>
        /// Gets or sets the Parent of this Customer.
        /// </summary>
        private PlanningItem _parent;
        public PlanningItem Parent
        {
            get
            {
                if (_parent == null)
                    _parent = PlanningAccess.GetPlanningProducts().SingleOrDefault(p => p.Idx == ParentId);

                return _parent;
            }
            set { _parent = value; }
        }
        #endregion

        public string CustomerId { get; set; }
    }
}