using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Xml.Linq;

/* Standard format of .txt file to be uploaded to wizard
 * Deepest node in each 'chain' contains name of relevant table 
 * Check with DB that this table exists and load into Table.html if selected 
 * Headers: 'data', 'table', 'complete', 'show'
 * table, complete and show - boolean
 * complete and show always false
 * table true only when deepest node
 * 
 * list subsections in 1, 1.1, 1.1.2, 2, 2.1, 2.2, 3 ... etc
 */

namespace ConfigTool
{
    public static class ParseWizard
    {
        //load wizard.txt and parse to json
        public static JObject textToJson()
        {
            //json object always starts and ends with '['
            var jsonString = new StringBuilder();
            jsonString.Append('[');
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\CosimaCalder\Documents\Visual Studio 2015\Projects\ConfigTool\ConfigTool\wizard.txt");

            foreach (string line in lines)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    if (char.IsDigit(line[i]))
                    {
                        if (line[i + 1].Equals("."))
                        { //if number. (section 'header')
                            char[] trim = { line[i], line[i + 1] };
                            var data = line.TrimStart(trim);
                            jsonString.Append("{ \"data\" : \"" + data + "\"");

                        }
                        else
                        {
                            continue;
                        }

                    }
                    else
                    {
                        continue;
                    }
                    //switch (item)
                    //{
                    //    case 
                    //}
                }
            }
            jsonString.Append(']');
            JObject json = JObject.Parse(jsonString.ToString());

            return json;
        }


    }
}
