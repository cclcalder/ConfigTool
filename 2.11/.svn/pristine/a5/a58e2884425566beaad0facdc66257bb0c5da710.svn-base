using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model
{
    [Serializable()]
    public class PromotionProductPrice
    {
        public PromotionProductPrice() { }
        public PromotionProductPrice(XElement el)
        {
            Name = el.GetValue<string>("Name");
            ID = el.GetValue<string>("ID");
            Volume = 100; // el.GetValue<string>("Volume");

            if (el.Element("Measures") != null)
                Measures = el.Element("Measures").Elements().Select(m => new PromotionMeasure(m)).ToList();

            IsFOC = el.Element("IsFOC") != null && el.Element("IsFOC").Value == "1";
            IsDisplay = el.Element("IsDisplay") != null && el.Element("IsDisplay").Value == "1";

        }

        //public PromotionProductPrice(XElement el)
        //    : this(el.GetValue<string>("Name"),
        //        el.GetValue<string>("ID"),
        //        "100", // el.GetValue<string>("Volume");
        //        el.GetValue<string>("Format")

        //        )
        //{

        //    if (el.Element("Measures") != null)
        //        Measures = el.Element("Measures").Elements().Select(m => new PromotionMeasure(m)).ToList();

        //    IsFOC = el.Element("IsFOC") != null && el.Element("IsFOC").Value == "1";
        //    IsDisplay = el.Element("IsDisplay") != null && el.Element("IsDisplay").Value == "1";

        //}



        //public PromotionProductPrice(string name, string id, string volume, string format)
        //{
        //    int intVolume;
        //    Name = name;
        //    ID = id;
        //    int.TryParse(volume, out intVolume);
        //    Volume = intVolume;
        //    Format = format;


        //}

       // public string Format { get; set; }

        /// <summary>
        /// Gets or sets the Name of this PromotionProduct.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Id of this PromotionProduct.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether this Promotion Product is Free of Charge.
        /// </summary>
        public bool IsFOC { get; set; }


        public int Volume { get; set; }

        /// <summary>
        /// Gets or sets the Measures of this PromotionProduct.
        /// </summary>
        private List<PromotionMeasure> _measures;
        public List<PromotionMeasure> Measures
        {
            get
            {
                return _measures;
            }
            set
            {
                if (_measures != value)
                    _measures = value.ToList();
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether this Promotion Product is a display containter       
        /// </summary>
        public bool IsDisplay { get; set; }

    }

}
