using System;
using System.Xml.Linq;

namespace Model
{   
    [Serializable]
    public class PromotionDate : DateRange
    {
        public PromotionDate() { }

        public PromotionDate(XElement el)
        {
            ID = el.GetValue<string>("ID");
            Description = el.GetValue<string>("Description");
            IsMandatory = el.GetValue<int>("IsMandatory") == 1;
            MustBeInFuture = el.GetValue<int>("MustBeInFuture") == 1;
            IsEditable = el.GetValue<int>("IsEditable") == 1;
            OffsetDays = el.GetValue<int>("OffSet");
            StartDate = el.GetValue<DateTime>("StartValue");
            EndDate = el.GetValue<DateTime>("EndValue");

        }

        public bool StopOffset { get; set; }

        #region ID
        /// <summary>
        /// Gets or sets the Id of this PromotionDate.
        /// </summary>
        public string ID { get; set; }
        #endregion

        #region Description
        /// <summary>
        /// Gets or sets the Description of this PromotionDate.
        /// </summary>
        public string Description { get; set; }
        #endregion

        #region IsMandatory
        /// <summary>
        /// Gets or sets the IsMandatory of this PromotionDate.
        /// </summary>
        public bool IsMandatory { get; set; }
        #endregion

        #region MustBeInFuture
        /// <summary>
        /// Gets or sets the MustBeInFuture of this PromotionDate.
        /// </summary>
        public bool MustBeInFuture { get; set; }
        #endregion

        #region IsEditable

        /// <summary>
        /// Is editable?
        /// </summary>
        public bool IsEditable { get; set; }
        #endregion

        #region OffsetDays
        public int OffsetDays { get; set; }
        #endregion

        #region EarliestStartDate
        public DateTime? EarliestStartDate
        {
            get
            {
                return (MustBeInFuture && IsEditable) ? DateTime.Today : default(DateTime?);
            }
            //get { return MustBeInFuture ? DateTime.Today.AddDays(10) : default(DateTime?); }
        }
        #endregion

        #region EarliestEndDate
        public DateTime? EarliestEndDate
        {
            get
            {
                var earliestStartDate = EarliestStartDate ?? StartDate;
                return earliestStartDate < StartDate ? StartDate : earliestStartDate;
            }
        }
        #endregion

    }
}
