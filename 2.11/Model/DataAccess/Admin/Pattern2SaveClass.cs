using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.DataAccess.Admin
{
    public class Pattern2SaveClass 
    {
        private List<string> m_ID;
        public List<string> ID
        {
            get {return m_ID ;}
            set 
            { 
                m_ID = value; 
            }
        }

        private List<int> m_isSelected;
        public List<int> isSelected
        {
            get { return m_isSelected; }
            set 
            { 
                m_isSelected = value; 
            }
        }

        public Pattern2SaveClass()
        { 
            if (ID == null)
            {
                ID = new List<string>();
            }
            if (isSelected == null)
            {
                isSelected = new List<int>();
            }
        }

        public Pattern2SaveClass pattern2SaveClass(List<string> id, List<int> selected)
        {
            Pattern2SaveClass thisPattern2SaveClass = new Pattern2SaveClass();
            thisPattern2SaveClass.ID = id;
            thisPattern2SaveClass.isSelected = selected;

            return thisPattern2SaveClass;
        }



    }
}
