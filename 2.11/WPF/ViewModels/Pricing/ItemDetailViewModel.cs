namespace WPF.ViewModels.Pricing
{
    using System;
    using Model.Entity;

    public class ItemDetailViewModel : PropertyChangedBase
    {
        private readonly ItemDetail _itemDetail;

        public ItemDetailViewModel(ItemDetail itemDetail)
        {
            _itemDetail = itemDetail;
        }

        public ItemDetail ItemDetail
        {
            get { return _itemDetail; }
        }

        public decimal ModifiedPrice
        {
            get { return _itemDetail.ModifiedPrice; }
            set
            {
                if (_itemDetail.ModifiedPrice != value)
                {
                    _itemDetail.ModifiedPrice = value;
                    RaisePropertyChanged(() => ModifiedPrice);
                }
            }
        }

        public string ProductId
        {
            get { return _itemDetail.ProductId; }
        }

        public DateTime FromDate
        {
            get { return _itemDetail.FromDate; }
            set
            {
                if (_itemDetail.FromDate != value)
                {
                    _itemDetail.FromDate = value;
                    RaisePropertyChanged(() => FromDate);
                }
            }
        }

        public DateTime ToDate
        {
            get { return _itemDetail.ToDate; }
            set
            {
                if (_itemDetail.ToDate != value)
                {
                    _itemDetail.ToDate = value;
                    RaisePropertyChanged(() => ToDate);
                }
            }
        }

        public DateTime OriginalFromDate
        {
            get { return _itemDetail.OriginalFromDate; }
        }

        public DateTime OriginalToDate
        {
            get { return _itemDetail.OriginalToDate; }
        }

        public decimal OriginalPrice
        {
            get { return _itemDetail.OriginalPrice; }
        }

        public string ProductName
        {
            get { return _itemDetail.ProductName; }
        }

        public string ProductCode
        {
            get { return _itemDetail.ProductCode; }
        }

        public bool HasChanges
        {
            get { return _itemDetail.HasChanges; }
        }

        public string Url
        {
            get { return _itemDetail.Url; }
        }
    }
}