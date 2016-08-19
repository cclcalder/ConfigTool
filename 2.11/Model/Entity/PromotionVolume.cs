using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Model.DataAccess;
using System.ComponentModel;
using Exceedra.Common;
using Exceedra.Common.Utilities;
using Model.Entity.Listings;

namespace Model
{
    [Serializable()]
    public class PromotionVolume : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        public PromotionVolume(XElement el)
        {
            _measures = new List<PromotionMeasure>();
            ProductId = el.GetValue<string>("ID");
            _productName = el.Element("Name").MaybeValue();
            HasTotal = el.GetValueOrDefault<string>("HasRowTotal").ToLower();

            // TO DO: Remove
            //VolBase = el.GetValue<decimal>("Vol_Base");
            //VolTotal = el.GetValue<decimal>("Vol_Total");

            var measures = el.Element("Measures");

            _measures = measures != null
                            ? measures.Elements().Select(m => new PromotionMeasure(m)).ToList()
                            : new List<PromotionMeasure>();
        }

        public string HasTotal;

        public string ProductName
        {
            get { return _productName ?? Product.Name; }
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

        #region VolBase
        /// <summary>
        /// Gets or sets the VolBase of this PromotionVolume.
        /// </summary>
        //public decimal VolBase { get; set; }
        #endregion

        /// <summary>
        /// Calcuated field for displaying total Display factor volume
        /// </summary>
       // public decimal AllowedForDisplay {get;set;}
        private decimal _rowTotal;
        public decimal RowTotal { get { return _rowTotal; } set { _rowTotal = value; PropertyChanged.Raise(this, "RowTotal"); } }

        #region VolTotal
        /// <summary>
        /// Gets or sets the VolTotal of this PromotionVolume.
        /// </summary>
        //public decimal VolTotal { get; set; }
        #endregion

        #region Measures
        /// <summary>
        /// Gets or sets the Measures of this PromotionVolumes.
        /// </summary>
        private readonly List<PromotionMeasure> _measures;

        private string _productName;

        public List<PromotionMeasure> Measures
        {
            get
            {
                return _measures;
            }
        }
        #endregion

        public PromotionMeasure GetMeasure(string name)
        {
            return GetMeasure(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        public PromotionMeasure GetMeasure(Func<PromotionMeasure,bool> predicate)
        {
            if (_measures == null) throw new InvalidOperationException("No measures loaded for PromotionVolume.");

            return _measures.FirstOrDefault(predicate);
        }

        public PromotionMeasure StealBaseVolume
        {
            get { return GetMeasure("Steal Base Vol"); }
        }

        public PromotionMeasure StealCannibalisationVolume
        {
            get { return GetMeasure("Steal Can Vol"); }
        }
    }
}
