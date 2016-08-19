using Newtonsoft.Json.Linq;
using System;
using System.Xml.Linq;

/* Standard format of .txt file to be uploaded to wizard
 * Deepest node in each 'chain' contains name of relevant table 
 * Check with DB that this table exists and load into Table.html if selected 
 * Headers: 'data', 'table', 'complete', 'show'
 * table, complete and show - boolean
 * complete and show always false
 * table true only when deepest node
 */

namespace ConfigTool
{
    public static class ParseWizard
    {
        //load wizard.txt and parse to json
        public static JObject textToJson()
        {
            string[] textWizard = System.IO.File.ReadAllLines(@"C:\Users\CosimaCalder\Documents\Visual Studio 2015\Projects\ConfigTool\ConfigTool\wizard.txt");
            foreach(string line in textWizard)
            {
                foreach(char item in line)
                {
                    //find .)
                }
            }
            var jsonString = "todo";
            JObject json = JObject.Parse(jsonString);

            return json;
        }


    }
}
