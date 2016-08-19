using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using Coder.UI.WPF;
using Model.DTOs;
using Model.Entity;
using System.ComponentModel;

namespace WPF.ViewModels.Claims
{
    public class ClaimReturnedMatchesViewModel : DataTreeNode
    {
        # region private
        private IDataTreeViewNodeEventsConsumer _eventsConsumer;
        private bool _isSingleSelectMode;
        private List<ClaimReturnedMatchesViewModel> _children;
        private int _nodeId;
        private MatchItemEvent _matchEvent;
        private DataTable _dataTreeNodeSource;
        private ReturnedMatches _returnedMatches;
        #endregion

        #region Constructors

        public ClaimReturnedMatchesViewModel(IDataTreeViewNodeEventsConsumer eventsConsumer, ClaimReturnedMatchesViewModel parent,
                                              DataTable dataTreeNodeSource, MatchItemEvent matchEvent, ReturnedMatches returnedMatches, bool isItemSaved, bool isSingleSelectMode)
            : base(eventsConsumer, 0, parent)
        {
            _isSingleSelectMode = isSingleSelectMode;
            _eventsConsumer = eventsConsumer;
            _returnedMatches = returnedMatches;
            _dataTreeNodeSource = dataTreeNodeSource;
            _matchEvent = matchEvent;
            _nodeId = _matchEvent != null ? int.Parse(_matchEvent.GetEventId()) : 0;
            _treeLevel = parent != null ? 1 : 0;
            ParentNodeId = parent != null ? parent.NodeId : 0;
            InitChildren();
            _isItemSaved = isItemSaved; 
        }

        private void InitChildren()
        {
            _claimEventMatches = null;
            _children = null;
            if (_matchEvent != null)
                ProcessClaimsList();
        }

        # endregion

        #region Properties
        private IList<ClaimEventMatch> _claimEventMatches;
        public IList<ClaimEventMatch> ClaimEventMatches
        {
            get { return _claimEventMatches; }
        }

        private bool _isItemSaved=false;
        public bool IsItemSaved
        {
            get { return _isItemSaved; }
            set { _isItemSaved = value; }
        }

        private int _treeLevel;
        public override int TreeLevel
        {
            get { return _treeLevel; }
        }

        public MatchItemEvent MatchEvent
        {
            get { return _matchEvent; }
        }

        protected Visibility _headerVisibility;
        public override Visibility HeaderVisibility { get { return _headerVisibility; } set { _headerVisibility = value; } }

        public override int NodeId
        {
            get { return _nodeId; }
        }

        public override IEnumerable<DataTreeNode> Children
        {
            get { return _children; }
            set { }
        }
         
        public override DataTable DataTreeNodeSource
        {
            get { return _dataTreeNodeSource; }
            set { _dataTreeNodeSource = value; }
        }

        #endregion

        #region Data
        private void ProcessClaimsList()
        {
            if (_returnedMatches.Matches == null) return;
            var matchItemClaims = _returnedMatches.Matches.Where(rm => rm.EventId == NodeId.ToString()).Select(claim => _returnedMatches.Claims.FirstOrDefault(s => s.GetClaimId() == claim.ClaimId)).ToList();
            if (matchItemClaims.Count == 0) return;
            _children = new List<ClaimReturnedMatchesViewModel>();
            _claimEventMatches = new List<ClaimEventMatch>();
            matchItemClaims.ToList().ForEach(x => _claimEventMatches.Add(new ClaimEventMatch { ClaimId = x.GetClaimId(), EventId = NodeId.ToString() }));
            _children.Add(new ClaimReturnedMatchesViewModel(_eventsConsumer,
                                                                   this,
                                                                   GetDetailedTableFromClaim(matchItemClaims),
                                                                   null,
                                                                   _returnedMatches,
                                                                   IsItemSaved,
                                                                   _isSingleSelectMode));
        }

        public void AddChildren(IList<ClaimEventMatch> claimEventMatches)
        {
            _claimEventMatches.AddRange(claimEventMatches);
        }

        public static DataTable GetDetailedTableFromEvent(MatchItemEvent matchItemEvent)
        {
            DataTable temptable = GetHeaderRow(matchItemEvent);
            DataRow valueRow = GetValueRow(matchItemEvent, temptable.NewRow());
            valueRow[DataTreeNode.IsSelected] = false;
            valueRow[DataTreeNode.IsParent] = true;
            valueRow[DataTreeNode.IsDeleteVisible] = true;
            valueRow[DataTreeNode.DataTreeNodeId] = matchItemEvent.GetEventId();
            valueRow[DataTreeNode.DataTreeParentNodeId] = 0;
            temptable.Rows.Add(valueRow);
            return temptable;
        }

        public DataTable GetDetailedTableFromClaim(IList<MatchItemClaim> matchItemClaims)
        {
            var temptable = GetHeaderRow(matchItemClaims[0]);
            foreach (var matchItemClaim in matchItemClaims)
            {
                DataRow valueRow = GetValueRow(matchItemClaim, temptable.NewRow());
                valueRow[DataTreeNode.IsSelected] = false;
                valueRow[DataTreeNode.IsParent] = false;
                valueRow[DataTreeNode.IsDeleteVisible] = true;
                valueRow[DataTreeNode.DataTreeNodeId] = matchItemClaim.GetClaimId();
                valueRow[DataTreeNode.DataTreeParentNodeId] = NodeId;
                temptable.Rows.Add(valueRow);
            }
            return temptable;
        }


        private static DataTable GetHeaderRow(object matchItem)
        {
            var temptable = new DataTable();
            temptable.Columns.Add(new DataColumn { ColumnName = DataTreeNode.IsDeleteVisible, DataType = System.Type.GetType("System.Boolean") });
            temptable.Columns.Add(new DataColumn { ColumnName = DataTreeNode.IsSelected, DataType = System.Type.GetType("System.Boolean") });
            temptable.Columns.Add(new DataColumn { ColumnName = DataTreeNode.IsParent, DataType = System.Type.GetType("System.Boolean") });
            temptable.Columns.Add(new DataColumn { ColumnName = DataTreeNode.DataTreeNodeId, DataType = System.Type.GetType("System.Int32") });
            temptable.Columns.Add(new DataColumn { ColumnName = DataTreeNode.DataTreeParentNodeId, DataType = System.Type.GetType("System.Int32") });
            foreach (var prop in matchItem.GetType().GetProperties())
            {
                if (!(prop.GetValue(matchItem, null) is IList && prop.GetValue(matchItem, null).GetType().IsGenericType))
                {
                   // bool displayColumn = false;
                    string headerName = prop.Name;
                    var attributes = prop.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    if (attributes != null)
                    {
                        var attribute = attributes.SingleOrDefault() as DisplayNameAttribute;
                        headerName = attribute.DisplayName;
                      //  displayColumn = true;
                    }
                    var column=new DataColumn { ColumnName = headerName };
                   
                    temptable.Columns.Add(column);
                }
            }
            return temptable;
        }

        private static DataRow GetValueRow(object matchItem, DataRow valueRow)
        {
            foreach (var valueItem in matchItem.GetType().GetProperties())
            {
                if (!(valueItem.GetValue(matchItem, null) is IList && valueItem.GetValue(matchItem, null).GetType().IsGenericType))
                {
                    string headerName = valueItem.Name;
                    var attributes = valueItem.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    if (attributes != null)
                    {
                        var attribute = attributes.SingleOrDefault() as DisplayNameAttribute;
                        headerName = attribute.DisplayName;
                    }
                    valueRow[headerName] = (valueItem.GetValue(matchItem, null) ?? string.Empty).ToString();
                }
            }
            return valueRow;
        }

        # endregion
    }
}
