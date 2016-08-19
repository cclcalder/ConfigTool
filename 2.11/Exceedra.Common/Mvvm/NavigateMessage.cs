 

namespace Exceedra.Common.Mvvm
{
    using System;
    using System.Windows.Controls;
 

    public class NavigateMessage :  IMessage, IEquatable<NavigateMessage>
    {
        public static NavigateMessage Back = new NavigateMessage();
        private readonly Page _page;
        private readonly Uri _uri;
        private readonly object _viewModel;

        private NavigateMessage()
        {
        }

        public NavigateMessage(Page page)
        {
            _page = page;
        }

        public NavigateMessage(Uri uri) : this(uri, null)
        {
        }

        public NavigateMessage(Uri uri, object viewModel)
        {
            _uri = uri;
            _viewModel = viewModel;
        }

        public object ViewModel
        {
            get { return _viewModel; }
        }

        public Uri Uri
        {
            get { return _uri; }
        }

        public Page Page
        {
            get { return _page; }
        }

        public bool Equals(NavigateMessage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_uri, other._uri) && Equals(_page, other._page);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NavigateMessage) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_uri != null ? _uri.GetHashCode() : 0)*397) ^ (_page != null ? _page.GetHashCode() : 0);
            }
        }

        public static bool operator ==(NavigateMessage left, NavigateMessage right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NavigateMessage left, NavigateMessage right)
        {
            return !Equals(left, right);
        }
    }
}