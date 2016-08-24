using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool
{
    //everytime make database change write it to here
    //this class make more efficient sql?
    class ScriptGenerator
    {
        List<string> queryList = new List<string>();
        public void Queries(string query)
        {
            var fileName = @"C:\Users\CosimaCalder\Documents\Visual Studio 2015\Projects\ConfigTool\ConfigTool\configScript"+DateTime.Now+".txt";
            queryList.Add(query);
            //add all queries until press 'create script for changes'
            //THIS CLICK WILL SEND QUERY 'GENERATE' i hope

            if (query.Equals("GENERATE"))
            {
                queryList = SimplifySQL(queryList); //only simplify/refactor once done
                File.WriteAllLines(fileName, queryList);
            }

        }

        public void ClearScript()
        {
            queryList.Clear();
        }

        public List<string> SimplifySQL(List<string> queries)
        {
            //here simplify it? speak to cristina
            return queries;
        }
    }
}
