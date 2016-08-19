using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Common.Utilities;

namespace Model.Entity.NPD
{
    public class NPDUser
    {
        public NPDUser()
        {
        }

        public NPDUser(XElement c)
        {
            ID = c.GetValue<string>("Idx");
            DisplayName = c.GetValue<string>("Name");
            ParentId = c.GetValue<string>("ParentIdx");
            IsSelectedString = c.GetValue<string>("IsSelected");
            IsSelected = IsSelectedString == "1";
        }

        #region ID

        /// <summary>
        /// Gets or sets the Id of this Customer.
        /// </summary>
        public string ID { get; set; }

        #endregion

        #region DisplayName

        /// <summary>
        /// Gets or sets the DisplayName of this Customer.
        /// </summary>
        public string DisplayName { get; set; }

        #endregion

        #region ParentId

        /// <summary>
        /// Gets or sets the ParentId of this Customer.
        /// </summary>
        public string ParentId { get; set; }

        #endregion

        #region IsSelected

        private bool _isSelected;

        /// <summary>
        /// Gets or sets the IsSelected of this Customer.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    if (_isSelected)
                        Selected.Raise(this);
                }
            }
        }

        public string IsSelectedString { get; set; }

        public event EventHandler Selected;

        #endregion

        #region Children

        /// <summary>
        /// Gets or sets the Children of this Customer.
        /// </summary>
        protected List<NPDUser> _Children;

        public IEnumerable<NPDUser> Children
        {
            get
            {
                if (_Children == null)
                {
                    _Children = new List<NPDUser>();
                }
                return _Children;
            }
        }

        #endregion

        #region Parent

        public NPDUser Parent { get; set; }

        #endregion

        public static IEnumerable<NPDUser> FromXml(XElement xml)
        {
            var dictionary = xml.Elements().Select(e => new NPDUser(e)).ToDictionary(user => user.ID);

            foreach (var user in dictionary.Values)
            {
                if (!string.IsNullOrWhiteSpace(user.ParentId))
                {
                    NPDUser parent;
                    if (dictionary.TryGetValue(user.ParentId, out parent))
                    {
                        user.Parent = parent;
                        parent._Children = parent._Children ?? new List<NPDUser>();
                        parent._Children.Add(user);
                    }
                }
            }

            return dictionary.Values;
        }

        public static bool HasNoParent(Customer customer)
        {
            return string.IsNullOrWhiteSpace(customer.ParentId);
        }
    }
}