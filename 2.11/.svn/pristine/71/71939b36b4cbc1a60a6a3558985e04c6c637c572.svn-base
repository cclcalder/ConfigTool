using System.Web.Mvc;

namespace SalesPlannerWeb.Helpers
{
    /// <summary>
    /// This class exists just to use [BindXml] instead of [ModelBinder(typeof(XmlModelBinder))]
    /// http://www.mikedellanoce.com/2009/12/aspnet-mvc-xml-model-binder.html
    /// </summary>
    public class BindXmlAttribute : CustomModelBinderAttribute
    {
        public override IModelBinder GetBinder()
        {
            return new XmlModelBinder();
        }
    }
}