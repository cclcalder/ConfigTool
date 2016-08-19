using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model
{
    public class StagedPromotion
    {
        public StagedPromotion(XElement el)
        {
            Products = new List<StagedProduct>();
            PromotionId = el.GetValue<string>("PromoID");
            StageType = el.GetValue<string>("TimeLevel");

            var products = el.Element("Products");

            if (products != null)
            {
                foreach (var product in products.Elements())
                {
                    Products.Add(new StagedProduct(product));
                }
            }
        }

        public string PromotionId { get; set; }
        public string StageType { get; set; }
        public bool IsEditable { get; set; }
        public bool IsReadOnly { get { return !IsEditable; } }
        public List<StagedProduct> Products { get; set; }

        public XElement CreateSaveArgument()
        {
            var promoArg = new XElement("SavePromotionVolumeDaily");
            promoArg.Add(new XElement("UserID", User.CurrentUser.ID));
            promoArg.Add(new XElement("PromoID", PromotionId));
            promoArg.Add(new XElement("TimeLevel", StageType));

            var productsArg = new XElement("Products");

            foreach (var product in Products)
            {
                var productNode = product.CreateSaveArgument();
                productsArg.Add(productNode);
            }

            promoArg.Add(productsArg);

            return promoArg;
        }
    }
}
