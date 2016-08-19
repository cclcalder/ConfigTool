using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.DataAccess;
using System.Xml.Linq;

namespace Model
{
    [Serializable()]
    public class PromotionWizardCustomer
    {
        /// <summary>
        /// Creates a new Customer instance.
        /// </summary>
        //public PromotionWizardCustomer(XElement c)
        //{
        //    ID = c.GetValue<string>("ID");

        //    DisplayName = c.GetValue<string>("DisplayName");
        //    if (c.GetValue<string>("ParentID") != null)
        //    {
        //        ParentId = c.GetValue<string>("ParentID");
        //    }
        //    if (c.GetValue<string>("IsSelected") != null)
        //    {
        //        IsSelected = c.GetValue<int>("IsSelected") == 1 ? true : false;
        //    }
        //}

        public PromotionWizardCustomer(XElement c)
        {
            ID = c.GetValue<string>("Idx");

            DisplayName = c.GetValue<string>("Name");
            if (c.GetValue<string>("ParentIdx") != null)
            {
                ParentId = c.GetValue<string>("ParentIdx");
            }
            if (c.GetValue<string>("IsSelected") != null)
            {
                IsSelected = c.GetValue<int>("IsSelected") == 1 ? true : false;
            }
        }

        public PromotionWizardCustomer() { }

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
            set { _isSelected = value; }
        }

        #endregion

        #region Children
        /// <summary>
        /// Gets or sets the Children of this Customer.
        /// </summary>
        protected List<PromotionWizardCustomer> _Children;
        public virtual IEnumerable<PromotionWizardCustomer> Children
        {
            get
            {
                if (_Children == null)
                    _Children = new PromotionAccess().GetPromotionWizardCustomers(string.Empty).Where(p => p.ParentId == ID).ToList();
                return _Children;
            }
        }
        #endregion

        internal void AddChild(PromotionWizardCustomer child)
        {
            (_Children ?? (_Children = new List<PromotionWizardCustomer>())).Add(child);
        }

        #region Parent
        /// <summary>
        /// Gets or sets the Parent of this Customer.
        /// </summary>
        protected PromotionWizardCustomer _parent;
        public virtual PromotionWizardCustomer Parent
        {

            get
            {
                if (_parent == null)
                    _parent = new PromotionAccess().GetPromotionWizardCustomers(string.Empty).SingleOrDefault(p => p.ID == ParentId);

                return _parent;
            }
            set { _parent = value; }
        }
        #endregion
    }

    //[Serializable()]
    //public class PromotionTemplateCustomer
    //{
    //    /// <summary>
    //    /// Creates a new Customer instance.
    //    /// </summary>
    //    public PromotionTemplateCustomer(XElement c)
    //    {
    //        ID = c.GetValue<string>("ID");

    //        DisplayName = c.GetValue<string>("DisplayName");
    //        if (c.GetValue<string>("ParentID") != null)
    //        {
    //            ParentId = c.GetValue<string>("ParentID");
    //        }
    //        if (c.GetValue<string>("IsSelected") != null)
    //        {
    //            IsSelected = c.GetValue<int>("IsSelected") == 1 ? true : false;
    //        }
    //    }

    //    public PromotionTemplateCustomer() { }

    //    #region ID
    //    /// <summary>
    //    /// Gets or sets the Id of this Customer.
    //    /// </summary>
    //    public string ID { get; set; }
    //    #endregion

    //    #region DisplayName
    //    /// <summary>
    //    /// Gets or sets the DisplayName of this Customer.
    //    /// </summary>
    //    public string DisplayName { get; set; }
    //    #endregion

    //    #region ParentId
    //    /// <summary>
    //    /// Gets or sets the ParentId of this Customer.
    //    /// </summary>
    //    public string ParentId { get; set; }
    //    #endregion

    //    #region IsSelected

    //    private bool _isSelected;

    //    /// <summary>
    //    /// Gets or sets the IsSelected of this Customer.
    //    /// </summary>
    //    public bool IsSelected
    //    {
    //        get { return _isSelected; }
    //        set { _isSelected = value; }
    //    }

    //    #endregion

    //    #region Children
    //    /// <summary>
    //    /// Gets or sets the Children of this Customer.
    //    /// </summary>
    //    protected List<PromotionTemplateCustomer> _Children;
    //    public virtual IEnumerable<PromotionTemplateCustomer> Children
    //    {
    //        get
    //        {
    //            if (_Children == null)
    //                _Children = new PromotionTemplateAccess().GetPromotionTemplateCustomers(string.Empty).Where(p => p.ParentId == ID).ToList();
    //            return _Children;
    //        }
    //    }
    //    #endregion

    //    internal void AddChild(PromotionTemplateCustomer child)
    //    {
    //        (_Children ?? (_Children = new List<PromotionTemplateCustomer>())).Add(child);
    //    }

    //    #region Parent
    //    /// <summary>
    //    /// Gets or sets the Parent of this Customer.
    //    /// </summary>
    //    protected PromotionTemplateCustomer _parent;
    //    public virtual PromotionTemplateCustomer Parent
    //    {

    //        get
    //        {
    //            if (_parent == null)
    //                _parent = new PromotionTemplateAccess().GetPromotionTemplateCustomers(string.Empty).SingleOrDefault(p => p.ID == ParentId);

    //            return _parent;
    //        }
    //        set { _parent = value; }
    //    }
    //    #endregion
    //}
}
