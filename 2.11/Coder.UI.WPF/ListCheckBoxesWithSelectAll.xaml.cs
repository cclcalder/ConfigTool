using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Coder.UI.WPF
{
    /// <summary>
    /// Interaction logic for ListCheckBoxesWithSelectall.xaml
    /// </summary>
    public partial class ListCheckBoxesWithSelectall
    {
        public const string SelectAllItem = "[Select All]";
        public ListCheckBoxesWithSelectall()
        {
            InitializeComponent();
        }
        private bool _ignoreCollectionChange;

        public static readonly DependencyProperty BindableItemListProperty =
            DependencyProperty.Register("BindableItemList", typeof(BindableItemList), typeof(ListCheckBoxesWithSelectall));

        public BindableItemList BindableItemList
        {
            get { return (BindableItemList)GetValue(BindableItemListProperty); }
            set { SetValue(BindableItemListProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ListCheckBoxesWithSelectall),
                                        new UIPropertyMetadata(ItemSourcePropertyChanged));

        // Using a DependencyProperty as the backing store for DefaultText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultTextProperty =
            DependencyProperty.Register("DefaultText", typeof(string), typeof(ListCheckBoxesWithSelectall),
                                        new UIPropertyMetadata(string.Empty));

        /// <summary>
        ///     Gets or sets a collection used to generate the content of the ComboBox
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary> 
        ///     Gets or sets the text displayed in the ComboBox if there are no selected items
        /// </summary>
        public string DefaultText
        {
            get { return (string)GetValue(DefaultTextProperty); }
            set { SetValue(DefaultTextProperty, value); }
        }

        #region DisplayMemberPath (Dependency Property)

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(ListCheckBoxesWithSelectall));

        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        #endregion

        #region SelectedItems (Dependency Property)

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IEnumerable), typeof(ListCheckBoxesWithSelectall),
                                        new PropertyMetadata(OnSelectedItemsChanged));

        public IEnumerable SelectedItems
        {
            get { return (IEnumerable)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        #endregion

        #region DisplaySeparator (Dependency Property)

        public static readonly DependencyProperty DisplaySeparatorProperty =
            DependencyProperty.Register("DisplaySeparator", typeof(String), typeof(ListCheckBoxesWithSelectall),
                                        new FrameworkPropertyMetadata(","));

        public String DisplaySeparator
        {
            get { return (String)GetValue(DisplaySeparatorProperty); }
            set { SetValue(DisplaySeparatorProperty, value); }
        }

        #endregion

        public static readonly DependencyProperty DisableMultiSelectProperty =
            DependencyProperty.Register("DisableMultiSelect", typeof(bool?), typeof(ListCheckBoxesWithSelectall), new PropertyMetadata(default(bool?)));

        public bool? DisableMultiSelect
        {
            get { return (bool?)GetValue(DisableMultiSelectProperty); }
            set { SetValue(DisableMultiSelectProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(ListCheckBoxesWithSelectall), new PropertyMetadata(default(bool), IsReadOnlyPropertyChangedCallback));

        private static void IsReadOnlyPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((ListCheckBoxesWithSelectall)dependencyObject).OnPropertyChanged("IsNotReadOnly");
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public bool IsNotReadOnly
        {
            get { return !IsReadOnly; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Whenever a CheckBox is checked, change the text displayed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxClick(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null && (string)checkBox.Content == SelectAllItem)
            {
                if (checkBox.IsChecked.GetValueOrDefault())
                {
                    foreach (var item in BindableItemList)
                    {
                        item.IsSelected = true;
                    }
                }
                else
                {
                    foreach (var item in BindableItemList)
                    {
                        item.IsSelected = false;
                    }
                }
            }
            DoSingleSelect(sender);

            _ignoreCollectionChange = true;
            SetItems();
            _ignoreCollectionChange = false;


            MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
            args.RoutedEvent = MouseLeftButtonUpEvent;
            args.Source = this;
           RaiseEvent(args);

        }

        private void DoSingleSelect(object sender)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                if (DisableMultiSelect.GetValueOrDefault())
                {
                    if (checkBox.IsChecked.GetValueOrDefault())
                    {
                        var bindableItem = checkBox.DataContext as BindableItem;
                        if (bindableItem != null)
                        {
                            foreach (var item in BindableItemList)
                            {
                                if (ReferenceEquals(bindableItem, item)) continue;
                                item.IsSelected = false;
                            }
                        }
                    }
                }
            }
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var combo = d as ListCheckBoxesWithSelectall;
            if (combo == null) return;

            var newCollection = e.NewValue as INotifyCollectionChanged;

            if (newCollection == null)
            {
                foreach (var bindableItem in combo.BindableItemList)
                {
                    bindableItem.IsSelected = false;
                }
                return;
            }

            var oldCollection = e.OldValue as INotifyCollectionChanged;
            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= combo.CollectionChanged;
            }
            newCollection.CollectionChanged += combo.CollectionChanged;

            SetSelectedItems(combo);
        }

        void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!_ignoreCollectionChange)
            {
                SetSelectedItems(this);
            }
        }

        private static void SetSelectedItems(ListCheckBoxesWithSelectall combo)
        {
            if (combo.SelectedItems != null)
            {
                foreach (var bindableItem in combo.BindableItemList)
                {
                    bindableItem.IsSelected = combo.SelectedItems.Cast<object>().Contains(bindableItem.Item);
                    //var bindableItem = combo.BindableItemList.FirstOrDefault(bi => item.Equals(bi.Item));
                    //if (bindableItem != null)
                    //{
                    //    bindableItem.IsSelected = true;
                    //}
                }
            }

            if (combo.SelectedItems != null && combo.SelectedItems.Cast<object>().Any())
            {
                combo.SetItems();
            }
        }

        private void ItemsSourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                BindableItemList.Clear();
                BindableItemList.Add(new BindableItem { Title = SelectAllItem });
                BindableItemList.AddRange(ItemsSource.Cast<object>().Select(o => CreateBindableItem(o, this)));
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems)
                {
                    if (!BindableItemList.Where(items => !items.Title.Equals(SelectAllItem)).Any(bi => bi.Item.Equals(newItem)))
                    {
                        BindableItemList.Add(CreateBindableItem(newItem, this));
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems)
                {
                    var bindableItem = BindableItemList.Where(items => !items.Title.Equals(SelectAllItem)).FirstOrDefault(bi => bi.Item.Equals(oldItem));
                    if (bindableItem != null)
                    {
                        BindableItemList.Remove(bindableItem);
                    }
                }
            }

            SetSelectedItems(this);
            ReseteSelectAll();
        }
        /// <summary>
        ///     Set the text property of this control (bound to the ContentPresenter of the ComboBox)
        /// </summary>
        private void SetItems()
        {
            if (ItemsSource == null) return;

            foreach (object eachItem in ItemsSource)
            {
                //if(((BindableItem) eachItem).Title==SelectAllItem) continue;
                BindableItem itemFound =
                    BindableItemList.FirstOrDefault(item => !item.Title.Equals(SelectAllItem) && item.Item.Equals(eachItem) && item.IsSelected);

                if (itemFound != null)
                {
                    var list = SelectedItems as IList;
                    if (list != null)
                    {
                        if (!list.Contains(eachItem))
                            list.Add(eachItem);
                    }
                }
                else
                {
                    var list = SelectedItems as IList;
                    if (list != null)
                    {
                        if (list.Contains(eachItem))
                        {
                            list.Remove(eachItem);
                        }
                    }
                }
            }
            ReseteSelectAll();
        } 

        private void ReseteSelectAll()
        {
            var resetSelectAllItem = BindableItemList.FirstOrDefault(bi => bi.Title == SelectAllItem);
            if (BindableItemList.Count > 0 && resetSelectAllItem !=null )
            {
                if (BindableItemList.Where(items => !items.Title.Equals(SelectAllItem)).All(x => x.IsSelected))
                    resetSelectAllItem.IsSelected = true;
                if (BindableItemList.Where(items => !items.Title.Equals(SelectAllItem)).Any(x => !x.IsSelected))
                    resetSelectAllItem.IsSelected = false;
            }
        }

        private static void ItemSourcePropertyChanged(DependencyObject src, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            var combo = src as ListCheckBoxesWithSelectall;
            if (combo == null) return;

            var collection = e.OldValue as INotifyCollectionChanged;
            if (collection != null)
            {
                collection.CollectionChanged -= combo.ItemsSourceOnCollectionChanged;
            }

            collection = e.NewValue as INotifyCollectionChanged;
            if (collection != null)
            {
                collection.CollectionChanged += combo.ItemsSourceOnCollectionChanged;
            }

            var bindableItemList = new BindableItemList(combo.DisplaySeparator);

            if (e.NewValue != null)
            {
                bindableItemList.Add(new BindableItem { Title = SelectAllItem });
                foreach (object eachItem in (IEnumerable)e.NewValue)
                {
                    var bindableItem = CreateBindableItem(eachItem, combo);

                    bindableItemList.Add(bindableItem);
                }
            }
            combo.SetValue(BindableItemListProperty, bindableItemList);
        }

        private static BindableItem CreateBindableItem(object eachItem, ListCheckBoxesWithSelectall source)
        {
            var bindableItem = new BindableItem { Item = eachItem };

            if (String.IsNullOrEmpty(source.DisplayMemberPath))
            {
                source.DisplayMemberPath = "Name";
            }

            if (!String.IsNullOrEmpty(source.DisplayMemberPath) &&
                eachItem.GetType().GetProperty(source.DisplayMemberPath) != null)
            {
                bindableItem.Title =
                    eachItem.GetType().GetProperty(source.DisplayMemberPath).GetValue(eachItem, null).ToString();

                var c = eachItem.GetType().GetProperty("Colour").GetValue(eachItem, null);

                bindableItem.Colour = (c == null ? "#FFFFFF" : c.ToString());
                    
            }
            else
            {
                bindableItem.Title = "**Missing DisplayMemberPath property**";
            }
            return bindableItem;
        }
    }
}
