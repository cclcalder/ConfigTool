using System.Collections.Generic;
using System.ComponentModel;

namespace Exceedra.Common.GroupingMenu
{
    public interface IMenuItem : INotifyPropertyChanged
    {
        string Id { get; set; }
        string Header { get; set; }

        string Url { get; set; }

        bool HasValidUrl { get; set; }

        string ParentId { get; set; }
        bool HasParent { get; }

        IList<IMenuItem> Children { get; set; }
        bool HasChildren { get; }

        bool IsSelected { get; set; }

        int SortOrder { get; set; }
    }
}