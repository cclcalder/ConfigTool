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
    public class Customer
    {
        private readonly Func<IEnumerable<Customer>> _getCustomers;

       public Customer() { }
        /// <summary>
        /// Creates a new Customer instance.
        /// </summary>
        public Customer(XElement c) : this(c, null)
        {
        }

        /// <summary>
        /// Creates a new Customer instance.
        /// </summary>
        public Customer(XElement c, Func<IEnumerable<Customer>> getCustomers)
        {
            _getCustomers = getCustomers;
            ID = c.GetValue<string>("Idx");
            DisplayName = c.GetValue<string>("Name");
            ParentId = c.GetValue<string>("ParentIdx");
            
            if (c.Element("IsSelected") != null)
            {
                IsSelected = c.GetValue<int>("IsSelected") == 1;
            }

            IsSelectedString = c.GetValue<string>("IsSelected");

            IsParentNode = c.Element("IsParentNode").MaybeValue() == "1";
        }

        //private Customer(XElement c, Func<IEnumerable<Customer>> getCustomers, bool badXML)
        //{
        //                _getCustomers = getCustomers;
        //    ID = c.GetValue<string>("Idx");
        //    DisplayName = c.GetValue<string>("DisplayName");
        //    ParentId = c.GetValue<string>("ParentIdx");
            
        //    if (c.Element("IsSelected") != null)
        //    {
        //        IsSelected = c.GetValue<int>("IsSelected") == 1;
        //    }

        //    IsSelectedString = c.GetValue<string>("IsSelected");
        //}

        public Customer(string id, string displayName, Customer parent, Func<IEnumerable<Customer>> getCustomers)
        {
            _getCustomers = getCustomers;
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
        /// Gets or sets the Children of this Customer.
        /// </summary>
        protected List<Customer> _Children;
        public  IEnumerable<Customer> Children
        {
            get
            {
                if (_Children == null)
                {
                    if (_getCustomers == null)
                    {
                        _Children = new List<Customer>();
                    }
                    else
                    {
                        _Children = _getCustomers().Where(p => p.ParentId == ID).ToList();
                    }
                }
                return _Children;
            }
        }
        #endregion

        #region Is Parent Node

        public bool IsParentNode { get; set; }

        #endregion 

        #region Parent
        /// <summary>
        /// Gets or sets the Parent of this Customer.
        /// </summary>
        protected Customer _parent;
        public Customer Parent
        {
            get
            {
                try
                {
                    if (_parent == null && ParentId != null)
                        _parent = _getCustomers().SingleOrDefault(p => p.ID == ParentId);
                }
                catch (Exception ex)
                {
                 
                }
             

                return _parent;
            }
            set { _parent = value; }
        }
        #endregion
        
        public static IEnumerable<Customer> FromXml(XElement xml)
        {
            if (xml == null) return null;

            var dictionary = xml.Elements().Select(e => new Customer(e)).ToDictionary(customer => customer.ID);

            foreach (var customer in dictionary.Values)
            {
                if (!string.IsNullOrWhiteSpace(customer.ParentId))
                {
                    Customer parent;
                    if (dictionary.TryGetValue(customer.ParentId, out parent))
                    {
                        customer.Parent = parent;
                        parent._Children = parent._Children ?? new List<Customer>();
                        parent._Children.Add(customer);
                    }
                }
            }

            return dictionary.Values;
        }

        //public static IEnumerable<Customer> FromBrokenXml(XElement xml)
        //{
        //    var dictionary = xml.Elements().Select(e => new Customer(e, null, false)).ToDictionary(customer => customer.ID);

        //    foreach (var customer in dictionary.Values)
        //    {
        //        if (!string.IsNullOrWhiteSpace(customer.ParentId))
        //        {
        //            Customer parent;
        //            if (dictionary.TryGetValue(customer.ParentId, out parent))
        //            {
        //                customer.Parent = parent;
        //                parent._Children = parent._Children ?? new List<Customer>();
        //                parent._Children.Add(customer);
        //            }
        //        }
        //    }

        //    return dictionary.Values;
        //}

        public static Customer FromXmlSingleReturn(XElement element)
        {
            const string idElement = "ID";
            const string displayNameElement = "Name";

            return new Customer(element)
            {
                ID = element.GetValue<string>(idElement),
                DisplayName = element.GetValue<string>(displayNameElement)
            };
        }

        public static bool HasNoParent(Customer customer)
        {
            return string.IsNullOrWhiteSpace(customer.ParentId);
        }
    }
}
