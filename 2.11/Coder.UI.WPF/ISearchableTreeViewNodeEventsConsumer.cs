using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coder.WPF.UI
{
    public interface ISearchableTreeViewNodeEventsConsumer
    {
        void NotifySelectedNodeChanged();
    }
}
