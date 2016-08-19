using Telerik.Windows.Controls;

namespace Coder.WPF.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    public class ReadOnlyStateManager : Decorator
    {
        private static readonly Dictionary<Type, Func<DependencyProperty>> TypesWithIsReadOnly = new Dictionary<Type, Func<DependencyProperty>>
                                                                                               {
                                                                                                   {typeof(TextBox), () => TextBoxBase.IsReadOnlyProperty},
                                                                                                   {typeof(DataGrid), () => DataGrid.IsReadOnlyProperty},
                                                                                                   {typeof(ComboBox), () => ComboBox.IsReadOnlyProperty},
                                                                                                   {typeof(SearchableTreeView), () => SearchableTreeView.IsReadOnlyProperty},
                                                                                               };
        private static readonly HashSet<Type> TypesWithoutIsReadOnly = new HashSet<Type>
        {
            typeof(RadComboBox)
        };

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(ReadOnlyStateManager), new UIPropertyMetadata(false, IsReadOnlyChanged));

        public static readonly DependencyProperty DisableComboBoxesProperty =
            DependencyProperty.Register("DisableComboBoxes", typeof(bool), typeof(ReadOnlyStateManager), new PropertyMetadata(default(bool)));

        public bool DisableComboBoxes
        {
            get { return (bool)GetValue(DisableComboBoxesProperty); }
            set { SetValue(DisableComboBoxesProperty, value); }
        }

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var manager = d as ReadOnlyStateManager;
            if (manager != null) manager.OnIsReadOnlyChanged((bool)e.NewValue);
        }

        private void OnIsReadOnlyChanged(bool newValue)
        {
            DependencyProperty isReadOnlyProperty = null;

            var descendants = Child.GetDescendants(typeof(ReadOnlyStateManager)).ToList();
            foreach (var descendant in descendants.Where(e => TryGetIsReadOnlyProperty(e.GetType(), out isReadOnlyProperty)))
            {
                descendant.SetValue(isReadOnlyProperty, newValue);
            }

            if (DisableComboBoxes)
            {
                foreach (var comboBox in descendants.OfType<ComboBox>())
                {
                    comboBox.IsEnabled = false;                                           
                }
            }

            foreach (var dataGrid in descendants.OfType<DataGrid>())
            {
                foreach (var column in dataGrid.Columns)
                {
                    column.IsReadOnly = newValue;
                }
            }

            foreach (var datePicker in descendants.OfType<DatePicker>())
            {
                datePicker.IsEnabled = !newValue;
            }
        }

        private static bool TryGetIsReadOnlyProperty(Type controlType, out DependencyProperty property)
        {
            Func<DependencyProperty> getDependencyPropertyFunc;
            if (TypesWithIsReadOnly.TryGetValue(controlType, out getDependencyPropertyFunc))
            {
                property = getDependencyPropertyFunc();
                return true;
            }
            if (TypesWithoutIsReadOnly.Contains(controlType))
            {
                property = null;
                return false;
            }

            CheckTypeForIsReadOnlyProperty(controlType);

            return TryGetIsReadOnlyProperty(controlType, out property);
        }

        private static void CheckTypeForIsReadOnlyProperty(Type controlType)
        {
            var dependencyPropertyField = controlType.GetField("IsReadOnlyProperty", BindingFlags.Public | BindingFlags.Static);
            var dependencyProperty = dependencyPropertyField != null
                                         ? dependencyPropertyField.GetValue(null) as DependencyProperty
                                         : null;

            lock (((ICollection)TypesWithIsReadOnly).SyncRoot)
            {
                if (dependencyProperty == null)
                {
                    if (!TypesWithoutIsReadOnly.Contains(controlType))
                    {
                        TypesWithoutIsReadOnly.Add(controlType);
                    }
                }
                else
                {
                    if (!TypesWithIsReadOnly.ContainsKey(controlType))
                    {
                        TypesWithIsReadOnly.Add(controlType, () => dependencyProperty);
                    }
                }
            }
        }
    }
}
