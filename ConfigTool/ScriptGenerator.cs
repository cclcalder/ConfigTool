using System;
using System.Collections.Generic;
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
            queryList.Add(query);
        }

        public void ClearScript()
        {

        }
    }
}
