using System.Globalization;
using Exceedra.Common;

namespace Model.Entity
{
    using System;
    using System.Xml.Linq;

    public class ItemDetail
    {
        private readonly string _productId;
        private readonly string _productName;
        private DateTime _fromDate;
        private readonly decimal _originalPrice;
        private decimal _modifiedPrice;
        private DateTime _toDate;
        private readonly string _productCode;
        private readonly DateTime _originalFromDate;
        private readonly DateTime _originalToDate;
        private readonly string _url;

        public ItemDetail(string productId, decimal price, string url)
            : this(productId, productId, productId, DateTime.Now, DateTime.Now, price, url)
        {
        }

        public ItemDetail(string productId, string productName, string productCode, DateTime fromDate, DateTime toDate, decimal price, string url)
        {
            _productId = productId;
            _productName = productName;
            _productCode = productCode;
            _url = url;
            _originalFromDate = _fromDate = fromDate;
            _originalToDate = _toDate = toDate;
            _originalPrice = _modifiedPrice = price;
        }

        public ItemDetail(XElement product)
        {
            _url = product.GetValueOrDefault<string>("URL");
            _productId = product.GetValue<string>("ProductID");
            _productName = product.Element("ProductName") != null ? product.GetValue<string>("ProductName") : _productId;
            _productCode = product.Element("ProductCode") != null ? product.GetValue<string>("ProductCode") : _productId;
            DateTime.TryParse(product.GetValue<string>("DateFrom"), out _fromDate);
            DateTime.TryParse(product.GetValue<string>("DateTo"), out _toDate);
            _originalFromDate = _fromDate;
            _originalToDate = _toDate;
            var value = product.GetValue<string>("Value");
            decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _originalPrice);
            _modifiedPrice = _originalPrice;
        }

        public string Url
        {
            get { return _url; }
        }

        public DateTime OriginalToDate
        {
            get { return _originalToDate; }
        }

        public string ProductName
        {
            get { return _productName; }
        }

        public string ProductCode
        {
            get { return _productCode; }
        }

        public decimal ModifiedPrice
        {
            get { return _modifiedPrice; }
            set { _modifiedPrice = value; }
        }

        public string ProductId
        {
            get { return _productId; }
        }

        public DateTime FromDate
        {
            get { return _fromDate; }
            set { _fromDate = value; }
        }

        public DateTime ToDate
        {
            get { return _toDate; }
            set { _toDate = value; }
        }

        public decimal OriginalPrice
        {
            get { return _originalPrice; }
        }

        public DateTime OriginalFromDate
        {
            get {
                return _originalFromDate;
            }
        }

        public bool HasChanges
        {
            get
            {
                return OriginalPrice != ModifiedPrice
                       || OriginalFromDate != FromDate
                       || OriginalToDate != ToDate;
            }
        }
    }
}