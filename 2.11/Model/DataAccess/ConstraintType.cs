using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Model.Annotations;

namespace Model
{

    //<Results>
    //  <ConstraintType>
    //    <ConstraintType_Idx>1</ConstraintType_Idx>
    //    <ConstraintType_Name>Products</ConstraintType_Name>
    //    <MissingData>1</MissingData>
    //  </ConstraintType>
    //  <ConstraintType>
    //    <ConstraintType_Idx>2</ConstraintType_Idx>
    //    <ConstraintType_Name>Dates</ConstraintType_Name>
    //    <MissingData>1</MissingData>
    //  </ConstraintType>
    //</Results>

    [Serializable()]
    public class ConstraintType : INotifyPropertyChanged
    {
        public ConstraintType(XElement c)
        {
            ID = c.GetValue<string>("ConstraintType_Idx");
            Name = c.GetValue<string>("ConstraintType_Name");
            MissingData = c.GetValue<string>("MissingData");
        }

        #region ID
        /// <summary>
        /// Gets or sets the Id of this Product.
        /// </summary>
        public string ID { get; set; }
        #endregion

        #region Name
        /// <summary>
        /// Gets or sets the DisplayName of this Product.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region MissingData
        /// <summary>
        /// Gets or sets the IsSelected of this Product.
        /// </summary>
        private string missingData;
        public string MissingData
        {
            get { return missingData; }
            set { missingData = value; }
        }
        #endregion

        #region IsSelected
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
