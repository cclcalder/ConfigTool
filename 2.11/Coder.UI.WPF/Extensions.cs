using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Coder.WPF.UI;

namespace Coder.UI.WPF
{
    public static class Extentions
    {
        public static IEnumerable<SearchableNode> WithAllChildren(this SearchableNode parent)
        {
            return parent.GetAllChildren().Concat(parent);
        }

        public static IEnumerable<SearchableNode> GetAllChildren(this SearchableNode parent)
        {
            if (parent.Children == null) return Enumerable.Empty<SearchableNode>();
            return parent.Children.SelectMany(c => c.WithAllChildren());
        }

        public static void ClearSelectedFlag(this IEnumerable<SearchableNode> viewModels)
        {
            foreach (SearchableNode viewModel in viewModels)
            {
                viewModel.IsSelected = false;
                if (viewModel.Children != null && viewModel.Children.Any())
                {
                    ClearSelectedFlag(viewModel.Children);
                }
            }
        }

        //internal static void Raise(this PropertyChangedEventHandler handler, object sender, string propertyName)
        //{
        //    if (handler != null)
        //    {
        //        handler(sender, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

        public static IEnumerable<DataTreeNode> WithAllChildren(this DataTreeNode parent)
        {
            return parent.GetAllChildren().Concat(parent);
        }

        public static IEnumerable<DataTreeNode> GetAllChildren(this DataTreeNode parent)
        {
            if (parent.Children == null) return Enumerable.Empty<DataTreeNode>();
            return parent.Children.SelectMany(c => c.WithAllChildren());
        }

        public static void ClearSelectedFlag(this IEnumerable<DataTreeNode> viewModels)
        {
            foreach (DataTreeNode viewModel in viewModels)
            {
                if (viewModel.Children != null && viewModel.Children.Any())
                {
                    ClearSelectedFlag(viewModel.Children);
                }
            }
        }
         
        //public static string GetName(this LambdaExpression expression)
        //{
        //    MemberExpression memberExpression;
        //    if (expression.Body is UnaryExpression)
        //    {
        //        var unaryExpression = (UnaryExpression)expression.Body;
        //        memberExpression = (MemberExpression)unaryExpression.Operand;
        //    }
        //    else if (expression.Body is MemberExpression)
        //    {
        //        memberExpression = (MemberExpression)expression.Body;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //    return memberExpression.Member.Name;
        //} 
    }
}
