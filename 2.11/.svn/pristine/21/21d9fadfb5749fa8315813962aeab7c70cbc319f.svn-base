using System.Xml.Linq;
using Exceedra.Common;

namespace Model
{
    public class PlanningItem
    {
        public PlanningItem(XElement xml)
        {
            BuildFromXml(xml);
        }

        /* From xml method for intial obj construction */
        private void BuildFromXml(XElement xml)
        {
            Idx = xml.Element("Idx").MaybeValue();
            DisplayName = xml.Element("Name").MaybeValue();
            ParentIdx = xml.Element("ParentIdx").MaybeValue();
            IsSelected = xml.Element("IsSelected").MaybeValue() == "1";
            IsSelectedString = xml.Element("IsSelected").MaybeValue();
        }

        public PlanningItem()
        {
        }

        #region Idx
        /// <summary>
        /// Gets or sets the Id of this Product.
        /// </summary>
        public string Idx { get; set; }
        #endregion

        #region DisplayName
        /// <summary>
        /// Gets or sets the DisplayName of this Product.
        /// </summary>
        public string DisplayName { get; set; }
        #endregion

        #region ParentIdx
        /// <summary>
        /// Gets or sets the ParentId of this Product.
        /// </summary>
        public string ParentIdx { get; set; }
        #endregion

        #region IsSelected
        /// <summary>
        /// Gets or sets the IsSelected of this Product.
        /// </summary>
        public bool IsSelected { get; set; }
        #endregion

        #region IsSelectedString

        public string IsSelectedString;
        #endregion

    }   
}
