using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Model;
using Model.Entity.Listings;
using WPF.ViewModels;


namespace WPF.UserControls.Listings
{
    public class SingleSelectionDataTemplateSelector : DataTemplateSelector
    {
        private HierarchicalDataTemplate _parentTemplate;

        private DataTemplate _nodeTemplate;

        public SingleSelectionDataTemplateSelector()
        {
            ParentTemplate = new HierarchicalDataTemplate();
            NodeTemplate = new DataTemplate();
        }

        public override DataTemplate SelectTemplate(Object item, DependencyObject container = null)
        {
            if (item is TreeViewHierarchy)
            {
                if ((item as TreeViewHierarchy).Children != null && (item as TreeViewHierarchy).Children.Any())
                {
                    
                    return _parentTemplate;
                }
                if ((item as TreeViewHierarchy).Children == null || !(item as TreeViewHierarchy).Children.Any())
                {
                    return _nodeTemplate;
                }
            }
            return null;
        }

        public HierarchicalDataTemplate ParentTemplate
        {
            get { return _parentTemplate; }
            set { _parentTemplate = value; }
        }

        public DataTemplate NodeTemplate
        {
            get { return _nodeTemplate; }
            set { _nodeTemplate = value; }
        }

    }
}