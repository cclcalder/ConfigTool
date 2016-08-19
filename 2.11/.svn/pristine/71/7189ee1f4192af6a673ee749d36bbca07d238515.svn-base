using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Model.DataAccess
{
    public   class LanguageAccess
    {
        public static  async void SaveActiveMessages(string cultureCode, string messageCode, string message) //App.CurrentLang.LanguageCode
        {
            //if (cultureCode.ToLower() != "en-gb")
            //{
                string argument = @"<SaveSQLMessage>
                            <Language_Code>{0}</Language_Code>
                            <Message_Code>{1}</Message_Code>
                            <Message_Text>{2}</Message_Text>
                          </SaveSQLMessage>".FormatWith(cultureCode, messageCode, message);

                // -- save message - fire and forget
                WebServiceProxy.CallAsync(StoredProcedure.Language.SqlMessageSave, XElement.Parse(argument), DisplayErrors.No);
            //}
           
        }

        public XElement GetAllMessages(string code)
        { 
            string argument = @"<SQLMessages>
                                    <LanguageCode>{0}</LanguageCode> 
                                </SQLMessages>".FormatWith(code);

            return WebServiceProxy.Call(StoredProcedure.Language.LoadAll, XElement.Parse(argument), DisplayErrors.No);
             
        }

        private   SQLMessage GetAllMessagesContinuation(Task<XElement> obj)
        {
            if (obj.IsFaulted) return null;
            if (obj.IsCanceled) return null;
            if (obj.Result == null) return null;
            var userElement = obj.Result.Elements().FirstOrDefault();
            if (userElement == null) return null;
            return new SQLMessage(userElement);
        }

        public class SQLMessage
        {
            public string Message_Code { get; set; }
            public string Message_Text { get; set; }

            public SQLMessage(XElement el)
            {
                Message_Code = el.GetValue<string>("Message_Code");
                Message_Text = el.GetValue<string>("Message_Text");
            }
        }
    }
}
