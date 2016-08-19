using Exceedra.Common;

namespace Model.Entity
{
    using System.Xml.Linq;

    public class PromoPhasing
    {
        public string PromotionID { get; set; }
        public string DayAPhaseID { get; set; }
        public string DayBPhaseID { get; set; }
        public string DayCPhaseID { get; set; }
        public string WeekPhaseID { get; set; }
        public string PostPromoPhaseID { get; set; }

        public static PromoPhasing Parse(XElement xml)
        {
            var phasing = new PromoPhasing {PromotionID = xml.Element("ID").MaybeValue()};
            foreach (var element in xml.Element("PhaseTypes").MaybeElements("PhaseType"))
            {
                switch (element.Element("Code").MaybeValue())
                {
                    case "DayAPhaseID":
                        phasing.DayAPhaseID = element.Element("Phase_Idx").MaybeValue();
                        break;
                    case "DayBPhaseID":
                        phasing.DayBPhaseID = element.Element("Phase_Idx").MaybeValue();
                        break;
                    case "DayCPhaseID":
                        phasing.DayCPhaseID = element.Element("Phase_Idx").MaybeValue();
                        break;
                    case "WeekPhaseID":
                        phasing.WeekPhaseID = element.Element("Phase_Idx").MaybeValue();
                        break;
                    case "PostPromoPhaseID":
                        phasing.PostPromoPhaseID = element.Element("Phase_Idx").MaybeValue();
                        break;
                }
            }
            return phasing;
        }
    }
}