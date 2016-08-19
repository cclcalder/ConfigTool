using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Common;
using Model.DataAccess;
using Model.Entity.Listings;

namespace Model
{
    [Serializable()]
    public class PromotionDisplayUnit
    {
        public PromotionDisplayUnit(XElement el)
        {          
            ProductId = el.GetValue<string>("ID");
            _unitName = el.Element("Name").MaybeValue();
             
            var products = el.Element("Measures");

            _products = products != null
                          ? products.Elements().Select(m => new DynamicMeasure(m)).ToList()
                          : new List<DynamicMeasure>();
        }

        public string DisplayName 
        {
            get { return _unitName ?? "Unit name not set"; }
        }

        #region ProductId
        /// <summary>
        /// Gets or sets the ProductId of this PromotionVolume.
        /// </summary>
        public string ProductId { get; set; }
        #endregion

        #region Product
        /// <summary>
        /// Gets or sets the Product of this PromotionVolume.
        /// </summary>
        private TreeViewHierarchy _product;
        public TreeViewHierarchy Product
        {
            get {
                if (_product != null) return _product;
                var access = new PromotionAccess();
                _product = access.GetAddPromotionProducts(null).FlatTree.FirstOrDefault(p => p.Idx == ProductId)
                           ?? access.GetAddPromotionProducts(null, false).FlatTree.FirstOrDefault(p => p.Idx == ProductId)
                           ?? new TreeViewHierarchy();
                return _product;
            }
            set { _product = value; }
        }
        #endregion

        //#region VolBase
        ///// <summary>
        ///// Gets or sets the VolBase of this PromotionVolume.
        ///// </summary>
        ////public decimal VolBase { get; set; }
        //#endregion

        //#region VolTotal
        ///// <summary>
        ///// Gets or sets the VolTotal of this PromotionVolume.
        ///// </summary>
        ////public decimal VolTotal { get; set; }
        //#endregion

        #region Measures
        /// <summary>
        /// Gets or sets the Measures of this PromotionVolumes.
        /// </summary>
        private readonly List<DynamicMeasure> _products;

        private string _unitName;

        public List<DynamicMeasure> Measures
        {
            get
            {
                return _products;
            }
        }
        #endregion

        //public PromotionMeasure GetMeasure(string name)
        //{
        //    return GetMeasure(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        //}
        //public PromotionMeasure GetMeasure(Func<PromotionMeasure,bool> predicate)
        //{
        //    if (_measures == null) throw new InvalidOperationException("No measures loaded for PromotionVolume.");

        //    return _measures.FirstOrDefault(predicate);
        //}

        //public PromotionMeasure StealBaseVolume
        //{
        //    get { return GetMeasure("Steal Base Vol"); }
        //}

        //public PromotionMeasure StealCannibalisationVolume
        //{
        //    get { return GetMeasure("Steal Can Vol"); }
        //}
    }
}
