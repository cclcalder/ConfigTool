using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace Coder.UI.WPF
{
    public abstract class DataTreeNode 
{
        #region constants

        public static string IsParent = "IsParent";
        public static string DataTreeNodeId = "NodeId";
        public static string DataTreeParentNodeId = "ParentNodeId";
        public static string IsSelected   = "IsSelected";
        public static string IsDeleteVisible = "CanDelete";
        
        #endregion

        protected IDataTreeViewNodeEventsConsumer EventsConsumer { get; set; }
        internal IDataTreeViewNodeEventsConsumer HostingDataTreeView { get; set; }
        public IDataTreeViewNodeEventsConsumer HostingTreeView { get; set; }

        public DataTreeNode Parent { get; protected set; }
        public DataTreeNode(IDataTreeViewNodeEventsConsumer eventsConsumer, int parentNodeId = 0, DataTreeNode parent = null)
        {
            EventsConsumer = eventsConsumer;
            Parent = parent;
            ParentNodeId = parentNodeId;
        }

        public abstract int NodeId { get; }
        public abstract int TreeLevel { get; } 
        public virtual IEnumerable<DataTreeNode> Children { get; set; }
        public abstract DataTable DataTreeNodeSource { get; set; }
        public abstract Visibility HeaderVisibility { get; set; }
        public int ParentNodeId { get; set; } 
    }

}
