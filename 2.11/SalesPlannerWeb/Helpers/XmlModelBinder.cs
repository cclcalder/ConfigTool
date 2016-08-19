using System.IO;
using System.Web.Mvc;
using System.Xml.Linq;

namespace SalesPlannerWeb.Helpers
{
    // http://www.mikedellanoce.com/2009/12/aspnet-mvc-xml-model-binder.html
    public class XmlModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // contains all the post data (eg. arg1=123&arg2=text&arg3=<xml></xml>)
            var stream = controllerContext.HttpContext.Request.InputStream;
            StreamReader reader = new StreamReader(stream);

            // the entire post data converted into string
            string input = reader.ReadToEnd();

            // the post data chopped into pieces (eg. splittedInput[0]=arg1=123  splittedInput[1]=arg2=text  splittedInput[2]=arg3=<xml></xml>
            string[] splittedInput = input.Split('&');

            // looking for the argument that was decorated with the [BindXml] attribute
            for (int i = 0; i < splittedInput.Length; i++)
            {
                // if a piece starts with the argument name and directly after has the equal sign - that's what we're looking for
                if (splittedInput[i].StartsWith(bindingContext.ModelName)
                    && splittedInput[i][bindingContext.ModelName.Length] == '=')
                {
                    // get the value of the argument and parse it into xml
                    string value = splittedInput[i].Substring(bindingContext.ModelName.Length + 1);
                    return XElement.Parse(value);
                }
            }

            return null;
        }
    }
}