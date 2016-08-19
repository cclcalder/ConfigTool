using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.DataAccess;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.Utilities;

namespace Model
{
    /// <summary>
    /// Generic GenericStatus class for use anywhere in app where we need a Hierachical list of GenericStatuses
    /// </summary>
    public class GenericStatus
    {
        private readonly Func<IEnumerable<GenericStatus>> _getGenericStatuses;

        public GenericStatus() { }
        /// <summary>
        /// Creates a new GenericStatus instance.
        /// </summary>
        public GenericStatus(XElement c)
            : this(c, null)
        {

            ID = c.GetValue<string>("ID");
            ParentId = c.GetValue<string>("Parent");
            DisplayName = c.GetValue<string>("Name");
            if (c.GetValue<int>("IsSelected") == 1)
            {
                IsSelected = true;
            }
            else if (c.GetValue<int>("IsSelected") == 0)
            {
                IsSelected = false;
            }
            else if (c.GetValue<int>("IsSelected") == 2)
            {
                IsSelected = null;
            }

            IsEnabled = c.GetValue<int>("IsEnabled") == 1 ? true : false;
            Colour = c.GetValue<string>("Colour");
            Sort = c.GetValue<string>("Sort");

        }
        /// <summary>
        /// Creates a new GenericStatus instance.
        /// </summary>
        public GenericStatus(XElement c, Func<IEnumerable<GenericStatus>> getGenericStatuses)
        {
            _getGenericStatuses = getGenericStatuses;
            ID = c.GetValue<string>("ID");
            DisplayName = c.GetValue<string>("Name");
            ParentId = c.GetValue<string>("Parent");

            if (c.GetValue<int>("IsSelected") == 1)
            {
                IsSelected = true;
            }
            else if (c.GetValue<int>("IsSelected") == 0)
            {
                IsSelected = false;
            }
            else if (c.GetValue<int>("IsSelected") == 2)
            {
                IsSelected = null;
            }

            IsSelectedString = c.GetValue<string>("IsSelected");

            IsEnabled = c.GetValue<int>("IsEnabled") == 1 ? true : false;
            Colour = c.GetValue<string>("Colour");
            Sort = c.GetValue<string>("Sort");

            ItemType = c.GetValue<string>("Item_Type");
        }

        public GenericStatus(string id, string displayName, GenericStatus parent, Func<IEnumerable<GenericStatus>> getGenericStatuses)
        {
            _getGenericStatuses = getGenericStatuses;
            ID = id;
            DisplayName = displayName;
            if (parent != null)
            {
                Parent = parent;
                ParentId = parent.ID;
            }
        }

        #region ID
        /// <summary>
        /// Gets or sets the Id of this GenericStatus.
        /// </summary>
        public string ID { get; set; }
        #endregion

        #region DisplayName
        /// <summary>
        /// Gets or sets the DisplayName of this GenericStatus.
        /// </summary>
        public string DisplayName { get; set; }
        #endregion

        #region ParentId
        /// <summary>
        /// Gets or sets the ParentId of this GenericStatus.
        /// </summary>
        public string ParentId { get; set; }
        #endregion

        #region IsSelected

        private bool? _isSelected;

        /// <summary>
        /// Gets or sets the IsSelected of this GenericStatus.
        /// </summary>
        public bool? IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    if (_isSelected == true)
                        Selected.Raise(this);
                }
            }
        }

        public string _isSelectedString;

        public string IsSelectedString
        {
            get { return _isSelectedString; }
            set { _isSelectedString = value; }
        }

        public event EventHandler Selected;

        #endregion

        #region Children
        /// <summary>
        /// Gets or sets the Children of this GenericStatus.
        /// </summary>
        protected List<GenericStatus> _Children;
        public IEnumerable<GenericStatus> Children
        {
            get
            {
                if (_Children == null)
                {
                    if (_getGenericStatuses == null)
                    {
                        _Children = new List<GenericStatus>();
                    }
                    else
                    {
                        _Children = _getGenericStatuses().Where(p => p.ParentId == ID).ToList();
                    }
                }
                return _Children;
            }
        }
        #endregion

        #region Parent
        /// <summary>
        /// Gets or sets the Parent of this GenericStatus.
        /// </summary>
        protected GenericStatus _parent;
        public GenericStatus Parent
        {
            get
            {
                try
                {
                    if (_parent == null && ParentId != null)
                        _parent = _getGenericStatuses().SingleOrDefault(p => p.ID == ParentId);
                }
                catch (Exception ex)
                {

                }


                return _parent;
            }
            set { _parent = value; }
        }
        #endregion

        #region IsEnabled
        /// <summary>
        /// Gets or sets the IsEnabled of this PromotionGenericStatus.
        /// </summary>
        public bool IsEnabled { get; set; }
        #endregion

        #region Item type

        /// <summary>
        /// Sets the item type to be returned to the DB
        /// </summary>
        public string ItemType { get; set; }

        #endregion

        public string Colour { get; set; }

        public string Sort { get; set; }

        public static IEnumerable<GenericStatus> FromXml(XElement xml)
        {
            var dictionary = xml.Elements().Select(e => new GenericStatus(e)).ToDictionary(GenericStatus => GenericStatus.ID);

            foreach (var GenericStatus in dictionary.Values)
            {
                if (!string.IsNullOrWhiteSpace(GenericStatus.ParentId))
                {
                    GenericStatus parent;
                    if (dictionary.TryGetValue(GenericStatus.ParentId, out parent))
                    {
                        GenericStatus.Parent = parent;
                        parent._Children = parent._Children ?? new List<GenericStatus>();
                        parent._Children.Add(GenericStatus);
                    }
                }
            }

            return dictionary.Values;
        }

        public IEnumerable<GenericStatus> GenericListFromXML(XElement xml)
        {
            var dictionary = xml.Elements().Select(e => new GenericStatus(e)).ToDictionary(GenericStatus => GenericStatus.ID);

            foreach (var GenericStatus in dictionary.Values)
            {
                if (!string.IsNullOrWhiteSpace(GenericStatus.ParentId))
                {
                    GenericStatus parent;
                    if (dictionary.TryGetValue(GenericStatus.ParentId, out parent))
                    {
                        GenericStatus.Parent = parent;
                        parent._Children = parent._Children ?? new List<GenericStatus>();
                        parent._Children.Add(GenericStatus);
                    }
                }
            }

            return dictionary.Values;
        }

        public static GenericStatus FromXmlSingleReturn(XElement element)
        {
            const string idElement = "ID";
            const string displayNameElement = "Name";

            return new GenericStatus(element)
            {
                ID = element.GetValue<string>(idElement),
                DisplayName = element.GetValue<string>(displayNameElement)
            };
        }

        public static bool HasNoParent(GenericStatus GenericStatus)
        {
            return string.IsNullOrWhiteSpace(GenericStatus.ParentId);
        }

        public GenericStatus NonStaticFromXML(XElement xml)
        {
            return new GenericStatus
            {
                ID = xml.Element("ID").MaybeValue(),
                DisplayName = xml.Element("Name").MaybeValue(),
                IsEnabled = xml.Element("IsEnabled").MaybeValue() == "1",
                IsSelected = xml.Element("IsSelected").MaybeValue() == "1",
                ParentId = xml.GetValue<string>("Parent"),
                Colour = xml.Element("Colour").MaybeValue()
            };
        }
    }
}
