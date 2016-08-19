using System.Xml.Linq;

namespace Model.Entity
{
    public class ApplicableRobResult
    {
        private string _id;
        private string _name;
        private bool _isEditable;

        public ApplicableRobResult()
        { 
        
        }
        public ApplicableRobResult(XElement el)
        {
            ID = el.GetValue<string>("AppType_Idx");
            Name = el.GetValue<string>("AppType_Name");
        }

        /// <summary>
        ///     Gets or sets the ID of this Rob.
        /// </summary>
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        ///     Gets or sets the Name of this Rob.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

    }
}