using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Exceedra.Common.Utilities;
using Exceedra.DynamicGrid.Models;
using EventHandlerEx = Exceedra.Common.Utilities.EventHandlerEx;
using Model.Entity.Generic;
using Model;
using Exceedra.Common;
using System.Windows.Media;

namespace Exceedra.Controls.DynamicGrid.Models
{
    [Serializable]

    public class Property : PropertyBase, INotifyPropertyChanged
    {
        private static int counter;

        public Property()
        {
            //CommentList = new List<ListboxComment> { new ListboxComment { TimeStamp = "24/2/16", UserName = "EW", Value = (counter++).ToString() } };
            
            //Set default order. If order is null then the grid won't show.
            ColumnSortOrder = 0;
        }

        public Property(Property p)
        {
            ColumnCode = p.ColumnCode;
            ControlType = p.ControlType;
            ForeColour = p.ForeColour;
            HeaderText = p.HeaderText;
            IsEditable = p.IsEditable;
            IsExpandable = p.IsExpandable;
            IsExpanded = p.IsExpanded;
            IsRequired = p.IsRequired;
            IsDisplayed = p.IsDisplayed;
            StringFormat = p.StringFormat;
            Value = p.Value;
            Date = p.Date;
            Width = p.Width;
            FitWidth = p.FitWidth;
            Alignment = p.Alignment;
            IDX = p.IDX;
            Type = p.Type;
        }

        public int ColumnSortOrder { get; set; }

        [XmlIgnore]
        public string DependentColumn { get; set; }
        public string UpdateToCell { get; set; }

        [XmlElement("ExternalData")]
        public string External_Data { get; set; }
        public string SalesOrg_Idx { get; set; }

        public string IDX { get; set; }
        public string Type { get; set; }

        public HeaderRow HeaderRowData { get; set; }

        public string _innerValue;

        private object _tabContent;
        public object TabContent
        {
            get { return _tabContent; }
            set
            {
                _tabContent = value;
                EventHandlerEx.Raise(PropertyChanged, this, "TabContent");
            }
        }

        /// <summary>
        /// Gets or sets the Value of this PromotionExtraData.
        /// Values can be either actual or calculated
        /// values that start with '=' are calculated
        /// values are formatted using the StringFormat value
        /// </summary>
        public override string Value
        {
            get { return _innerValue; }
            set
            {
                SetValue(value);
            }
        }

        /* Could change this to an event? Maybe be cleaner that way. */
        public string NonInternalValue
        {
            get
            {
                return _innerValue;
            }
            set
            {
                EventHandlerEx.Raise(PropertyChanged, this, "NonInternalValue");
            }
        }

        /* Always update Value to update the UI
         * If not Locked, then also update the NonInternalValue. Using this you can action on the NonInternalValue Property Changed. 
         * Original implementation was to allow TreeGrid (dis)aggregation to push changes up/down the tree without invoking circular changes.
         * e.g. Change a cell, lock the cells above and below, push the new value up and down the tree, since the above and below cells are locked
         * their Value will change (updates UI) but they wont invoke further changes up and down the tree.
         */
        public void SetValue(string newValue)
        {
            _innerValue = FormatValue(newValue);

            if (OriginalValue == null)
                OriginalValue = Value;

            HasChanged = Value != OriginalValue;

            EventHandlerEx.Raise(PropertyChanged, this, "Value");

            if (!Locked)
                NonInternalValue = newValue;
        }

        private string _originalValue;
        public string OriginalValue { get { return _originalValue; } set { _originalValue = FormatValue(value); } }

        private bool _hasChanged;
        public bool HasChanged
        {
            get
            {
                return _hasChanged;
            }
            set
            {
                if (HasChanged == value) return;

                _hasChanged = value;
                PropertyChanged.Raise(this, "HasChanged");
            }
        }

        public string OriginalValue2 { get; set; }

        //Used for holding checkbox value on the dp seasonal grid where we have 2 controls in one cell.
        private string _value2;
        public string Value2
        {
            get { return _value2; }
            set
            {
                _value2 = value;

                if (string.IsNullOrWhiteSpace(OriginalValue2))
                    OriginalValue2 = Value2;

                HasValue2Changed = OriginalValue2 != Value2;

                EventHandlerEx.Raise(PropertyChanged, this, "Value2");
            }
        }

        private bool _hasValue2Changed;
        public bool HasValue2Changed
        {
            get
            {
                return _hasValue2Changed;
            }
            set
            {
                if (HasValue2Changed == value) return;

                _hasValue2Changed = value;
                PropertyChanged.Raise(this, "HasValue2Changed");
            }
        }

        private ObservableCollection<Option> _values;
        public override ObservableCollection<Option> Values
        {
            get
            {
                return _values;
            }
            set
            {
                _values = value;
                // SelectedItems = new ObservableCollection<Option>();
                var newlist = _values.Where(r => r.IsSelected == true).ToList();
                SelectedItems = new ObservableCollection<Option>(newlist); ;

                if (newlist.Count > 0)
                {
                    if (this.ControlType.ToLower() == "dropdown")
                    {
                        SelectedItem = newlist.FirstOrDefault();
                        EventHandlerEx.Raise(PropertyChanged, this, "SelectedItem");
                    }

                    if (this.ControlType == "MultiSelectDropdown")
                    {

                        EventHandlerEx.Raise(PropertyChanged, this, "SelectedItems");
                    }

                }
                else
                {

                }

                EventHandlerEx.Raise(PropertyChanged, this, "Values");

                foreach (var child in _values)
                {

                }

            }
        }


        private Option _selectedItem;
        public override Option SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                EventHandlerEx.Raise(PropertyChanged, this, "SelectedItem");
            }
        }

        private ObservableCollection<Option> _selectedItems;
        public override ObservableCollection<Option> SelectedItems
        {
            get { return _selectedItems; }
            set
            {
                _selectedItems = value;
                EventHandlerEx.Raise(PropertyChanged, this, "SelectedItems");
            }
        }

        public override bool HasValue()
        {
            // If it's a simple control it will store its value into the "Value" property.
            // If it's something more complex (like dropdown or multi-select dropdown) it will have populated values which are only options to select.
            // Selected item (if it's single-select) or items (if it's multi-select) are stored into the SelectedItem or SelectedItems property respectively.
            // If the property doesn't have any value or selected item (or items) we assume that it doesn't have a value.
            // Otherwise it has.
            if (
                // property doesn't have a value,
                string.IsNullOrEmpty(Value)
                // doesn't have a selected item
                && SelectedItem == null
                // or items
                && (SelectedItems == null || !SelectedItems.Any())
                )
                // so it doesn't have the value!
                return false;

            return true;
        }

        public string ConvertDataSourceInput(Dictionary<string, object> inputsDictionary = null)
        {
            var ripProperty = XElement.Parse(DataSourceInput.Replace("&gt;", ">").Replace("&lt;", "<"));

            if (inputsDictionary != null)
                foreach (var node in ripProperty.Elements().Where(n => String.IsNullOrWhiteSpace(n.Value)))
                {
                    object value;
                    inputsDictionary.TryGetValue(node.Name.ToString(), out value);
                    node.Value = value != null ? value.ToString() : "";
                }
            return ripProperty.ToString();
        }

        [field: NonSerialized]
        public new event PropertyChangedEventHandler PropertyChanged;

        private bool _locked;
        /* Lock this property when you do not what a value change to fire off a property change */

        public bool Locked
        {
            get
            {
                return _locked;
            }
            set
            {
                _locked = value;
                //IsEditable = !Locked;

                //if (_unlockedColour == null) _unlockedColour = BorderColour;

                //if (BorderColour == BackgroundColour) BorderColour = Locked ? "#B2B2B2" : _unlockedColour;
                //BorderColour = Locked ? "#FF0000" : _unlockedColour;
                //PropertyChanged.Raise(this, "IsEditable");
                //PropertyChanged.Raise(this, "BackgroundColour");
                //IsReadOnly = Locked;
            }
        }

        private bool _leftRightLocked;
        /* Lock this property when you do not what a value change to fire off a property change to another control */

        public bool LeftRightLocked
        {
            get
            {
                return _leftRightLocked;
            }
            set
            {
                _leftRightLocked = value;
            }
        }

        private string _unlockedColour;

        /* Gets a new Property with the basic values of this Property.
         * Used when adding a new record so the cells maintain matching properties
         */
        public Property GetPropertyTemplate()
        {
            return new Property
            {
                StringFormat = StringFormat,
                ColumnCode = ColumnCode,
                IsDisplayed = IsDisplayed,
                HeaderText = HeaderText,
                ControlType = ControlType,
                IsEditable = IsEditable
            };
        }

        #region Comments

        private List<ListboxComment> _commentList = new List<ListboxComment>();
        public List<ListboxComment> CommentList
        {
            get { return _commentList; }
            set
            {
                _commentList = value;

                AssertCommentState();
            }
        }

        public void AssertCommentState()
        {
            if (OriginalCommentIdxs == null)
                OriginalCommentIdxs = _commentList.Any() ? _commentList.Select(c => c.Idx).ToList() : new List<int>();

            HasCommentChanged = CommentsChanged;

            if (!CommentsLocked)
                PropertyChanged.Raise(this, "NonInternalCommentsChange");

            HasComment = CommentList.Any();
            PropertyChanged.Raise(this, "CommentList");
        }

        public List<int> OriginalCommentIdxs { get; set; }


        public bool CommentsLocked { get; set; }

        /* Fire this event when commentList changes from user input */
        public string NonInternalCommentsChange { get; set; }

        private bool _hasComment = false;
        public bool HasComment
        {
            get { return _hasComment || (CommentList == null && CommentList.Any()); }
            set
            {
                _hasComment = value;
                PropertyChanged.Raise(this, "HasComment");
                PropertyChanged.Raise(this, "CommentColour");
            }
        }

        private bool _hasCommentChanged;
        public bool HasCommentChanged
        {
            get
            {
                return _hasCommentChanged;
            }
            set
            {
                if (HasCommentChanged == value) return;

                _hasCommentChanged = value;
                PropertyChanged.Raise(this, "HasCommentChanged");
            }
        }

        public bool CommentsChanged
        {
            get
            {
                var commentListIdxs = CommentList.Select(c => c.Idx).ToList();

                if (OriginalCommentIdxs.Any() || commentListIdxs.Any())
                    if (OriginalCommentIdxs.Any(c => !commentListIdxs.Contains(c)) || commentListIdxs.Any(c => !OriginalCommentIdxs.Contains(c)))
                        return true;

                return false;
                //return (OriginalCommentIdx != -1 && CommentList.FirstOrDefault() == null)//If there was an original comment, but now it has been deleted.
                //    || (OriginalCommentIdx == -1 && CommentList.FirstOrDefault() != null)//OR if there was no origial comment, but now there is.
                //  || (OriginalCommentIdx != CommentList.First().Idx); //OR if there was an original comment, but has now been overridden by a new one.
            }
        }

        /* If there is no comment then set the colour to be transparent */
        private string _commentColour = "#B20000";
        private Property p;

        public string CommentColour
        {
            get { return HasComment ? _commentColour : "#00FFFFFF"; }
            set
            {
                _commentColour = value;
                PropertyChanged.Raise(this, "CommentColour");
            }
        }

        private static int newCommentIdx { get; set; }

        internal void AddComment(string text)
        {
            if (newCommentIdx > -2) newCommentIdx = -2;

            CommentList.Add(new ListboxComment { Value = text, UserName = User.CurrentUser.DisplayName, TimeStamp = DateTime.Today.ToString(), Idx = newCommentIdx-- });
            AssertCommentState();

            PropertyChanged.Raise(this, "CommentList");
        }


        #endregion


    }


}



