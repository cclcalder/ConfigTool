using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Exceedra.DynamicGrid.Models
{
    [Serializable]
    public abstract class RecordBase
    {
        #region events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region properties

        /* Code in the DB */
        public string Item_Code { get; set; }
        /* Name displayed to the user */
        public string Item_Name { get; set; }

        public string Item_Type { get; set; }

        public string Item_Idx { get; set; }

        public int Item_RowSortOrder { get; set; }

        //New for planning
        public string Item_AggrType { get; set; }

        //New for planning
        public string Item_Colour { get; set; }

        #endregion


        public abstract bool ArePropertiesFulfilled();

    }
}
