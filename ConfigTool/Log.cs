using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool
{
    public class Log : System.IO.TextWriter
    {
        public override void Write(char[] buffer, int index, int count)
        {
            System.Diagnostics.Debug.Write(new string(buffer, index, count));
        }
        public override void Write(string value)
        {
            System.Diagnostics.Debug.Write(value);
        }
        public override Encoding Encoding
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
