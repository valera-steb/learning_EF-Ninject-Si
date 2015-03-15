using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using Igniter.SystemMock;

namespace Igniter.Core
{
    class NotifierFindingExpressionVisitor : ExpressionVisitor
    {
        public readonly HashSet<MemberExpression> NotifyingMembers = new HashSet<MemberExpression>(PropertyChainEqualityComparer.Instance);

        public readonly Dictionary<MemberExpression, DependencyProperty> DependencyProperties =
            new Dictionary<MemberExpression, DependencyProperty>(PropertyChainEqualityComparer.Instance);

        public readonly HashSet<Expression> NotifyingCollections = new HashSet<Expression>(PropertyChainEqualityComparer.Instance);

        public readonly HashSet<Expression> BindingLists = new HashSet<Expression>(PropertyChainEqualityComparer.Instance);

        protected override Expression VisitMember(MemberExpression node)
        {
            // т.е. есть выражение, по которому мы лазим и из которого выбираем объекты, которые могут нас оповещать
            if (IsPropertyChain(node.Expression))
            {
                var dependencyProperty = GetDependencyProperty(node.Expression.Type, node.Member.Name);

                if (dependencyProperty != null)
                    DependencyProperties.Add(node, dependencyProperty);
                else if (typeof(INotifyPropertyChanged).IsAssignableFrom(node.Expression.Type))
                    NotifyingMembers.Add(node);
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Object != null) // Is this a static method call?
                RegisterNotifyingCollection(node.Object);

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            RegisterNotifyingCollection(node.Object);

            return base.VisitIndex(node);
        }

        private void RegisterNotifyingCollection(Expression node)
        {
            if (IsPropertyChain(node))
                // if node is both of these types, we only want one registration.
                if (typeof(INotifyCollectionChanged).IsAssignableFrom(node.Type))
                    NotifyingCollections.Add(node);
                else if (typeof(IBindingList).IsAssignableFrom(node.Type))
                    BindingLists.Add(node);
        }

        private static bool IsPropertyChain(Expression node)
        {
            while (true)
            {
                switch (node.NodeType)
                {
                    case ExpressionType.Constant:
                        return true;

                    case ExpressionType.MemberAccess:
                        var memberExpr = (MemberExpression)node;
                        node = memberExpr.Expression;
                        break;

                    default:
                        return false;
                }
            }
        }

        private static DependencyProperty GetDependencyProperty(Type type, string name)
        {
            if (!typeof(DependencyObject).IsAssignableFrom(type)) return null;

                

            return TypeDescriptor
                .GetProperties(type, new[] { new PropertyFilterAttribute(PropertyFilterOptions.All) })
                .Cast<PropertyDescriptor>()
                .Where(p => p.Name == name)
                .Select(DependencyPropertyDescriptor.FromProperty)
                .Where(dpd => dpd != null)
                .Select(dpd => dpd.DependencyProperty)
                .OrderBy(dpd => dpd.OwnerType, SubclassCompareer.Instance)
                .FirstOrDefault();
        }

        private class SubclassCompareer : IComparer<Type>
        {
            public static readonly SubclassCompareer Instance = new SubclassCompareer();

            private SubclassCompareer() { }

            public int Compare(Type x, Type y)
            {
                return x == y ? 0 : x.IsSubclassOf(y) ? 1 : -1;
            }
        }

        private class PropertyChainEqualityComparer : IEqualityComparer<Expression>
        {
            public static readonly PropertyChainEqualityComparer Instance = new PropertyChainEqualityComparer();

            private PropertyChainEqualityComparer() { }

            public bool Equals(Expression x, Expression y)
            {
                while (true)
                {
                    if (x.NodeType != y.NodeType) return false;

                    switch (x.NodeType)
                    {
                        case ExpressionType.Constant:
                            var xConstantExpr = (ConstantExpression)x;
                            var yConstantExpr = (ConstantExpression)y;

                            return ReferenceEquals(xConstantExpr.Value, yConstantExpr.Value);

                        case ExpressionType.MemberAccess:
                            var xMemberExpr = (MemberExpression)x;
                            var yMemberExpr = (MemberExpression)y;

                            if (xMemberExpr.Member != yMemberExpr.Member)
                                return false;

                            x = xMemberExpr.Expression;
                            y = yMemberExpr.Expression;
                            break;

                        default:
                            throw new InvalidOperationException(x.NodeType.ToString());
                    }
                }
            }

            public int GetHashCode(Expression node)
            {
                var hash = 17;

                while (true)
                {
                    switch (node.NodeType)
                    {
                        case ExpressionType.Constant:
                            var constantExpr = (ConstantExpression)node;

                            return hash * 31 + (constantExpr.Value != null ? constantExpr.Value.GetHashCode() : 0);

                        case ExpressionType.MemberAccess:
                            var memberExpr = (MemberExpression)node;

                            hash = hash * 31 + memberExpr.Member.GetHashCode();

                            node = memberExpr.Expression;
                            break;

                        default:
                            throw new InvalidOperationException(node.NodeType.ToString());
                    }
                }
            }
        }
    }
}