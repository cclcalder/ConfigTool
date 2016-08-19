using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Model;
using Model.DataAccess;

namespace Exceedra.Web.Areas.app.Models
{
    public class PromotionDetailsModel
    {

        public Model.Promotion PromotionDetail { get; set; }
        private Model.DataAccess.PromotionAccess _promoAccess { get; set; }

        public Task<Promotion> GetPromotionAsync(string promoId)
        {
            XElement argument = new XElement("GetPromotion");
            argument.Add(new XElement("User_Idx","11"));
            argument.Add(new XElement("PromotionID", promoId));

            return WebServiceProxy.CallAsync(StoredProcedure.Promotion.GetPromotion, argument).ContinueWith(t => GetPromotionContinuation(t));
        }

        private Promotion GetPromotionContinuation(Task<XElement> task)
        { 
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new Promotion();

            var promotion = new Promotion()
            {
                Name = "Test"
            }; //new Promotion(task.Result.Elements().FirstOrDefault(), _promoAccess);

            return promotion; 
        }

        public   PromotionDetailsModel(string id)
        { 
            PromotionDetail = GetPromotionAsync(id).Result;
             
        }

    }
}