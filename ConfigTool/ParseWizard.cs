using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

//adds children [ TWICE --> WHERE?
//and deal with ending subsections as well as creating

//variable called 'end node' or something
//containing closing brackets AND saving table name (always in end node..)


namespace ConfigTool
{
    public static class ParseWizard
    {
        //load wizard.txt and parse to json
        public static JObject TextToJson()
        {
            //json object always starts and ends with '['
            var jsonString = new StringBuilder();
            int n = 0;
            var level = new List<int>(); //at what section level (1-2)? so know when to drop back a level?
            jsonString.Append('[');
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\CosimaCalder\Documents\Visual Studio 2015\Projects\ConfigTool\ConfigToolTests\wizard.txt");

            foreach (string line in lines)
            {
                //what level node
                String[] splitArray = line.Split(' ');
                level.Add(splitArray[0].Length / 2);
            }

            foreach (string line in lines)
            {
                n++;
                Parse(line, jsonString, level, n);
            }
            jsonString.Append(']');
            JObject json = JObject.Parse(jsonString.ToString());

            return json;
        }
        public static void Parse(string line, StringBuilder jsonString, List<int> level, int n)
        {
            int i = 0;
            var info = "show: false,\ncomplete: false";
            var child = ",\nchildren: [\n";
            //var rootNode = "\ntableName:";

            if (char.IsDigit(line[i]))
            {
                if (line[i + 1].Equals('.'))
                {
                    if (line[i + 2].Equals(' '))
                    {
                        char[] trim = { line[i], line[i + 1] };
                        var data = line.TrimStart(trim).Trim();
                        jsonString.Append("{\ndata : '" + data + "',\n" + info);
                    }
                    if (n > 1 && level[n - 1] < level[n - 2])
                    {
                        char[] trim = { line[i], line[i + 1] };
                        var data = line.TrimStart(trim).Trim();
                        jsonString.Append(child);
                        jsonString.Append("{\ndata : '" + data + "',\n" + info);
                    }
                    //if root node?
                    if (n > 1 && level[n] < level[n - 1])
                    {
                        char[] trim = { line[i], line[i + 1] };
                        var data = line.TrimStart(trim).Trim();
                        jsonString.Append(child);
                        jsonString.Append("{\ndata : '" + data + "',\n" + info +"\n}\n]");
                    }
                    else
                    {
                        line = line.Substring(2);
                        Parse(line, jsonString.Append(child), level, n); //adds children twice?
                    }
                }
                else
                {

                }

            }
            else
            {

            }

        }
        public static List<string> TablesPresent()
        {
            var tablesPresent = new List<string>();
            return tablesPresent;
        }


    }
}
