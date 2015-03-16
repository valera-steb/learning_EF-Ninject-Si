using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IgniterPart.SDK.ReactiveMock;

namespace Igniter.Core
{
    abstract class Watcher<T> : IWatcher where T : class
    {
        public event EventHandler Changed = delegate { };

        private readonly SerialDisposable _disposable = new SerialDisposable();

        private readonly Func<T> _accessor;
        private T _current;

        public Watcher(Expression accessor)
        {
            if (accessor.NodeType == ExpressionType.Constant)
            {
                // do this outside the closure so we do it only once.
                var value = (T)((ConstantExpression)accessor).Value;
                _accessor = () => value;
            }
            else
                _accessor = GetAccessor(accessor);
        }

        public void SubscribeToCurrentNotifier()
        {
            _current = _accessor();

            _disposable.Disposable = _current != null ? Subscribe(_current) : null;
        }

        protected abstract IDisposable Subscribe(T notifier);

        protected virtual void OnChanged()
        {
            Changed(this, EventArgs.Empty);
        }

        public virtual void Dispose()
        {
            _disposable.Dispose();
        }

        private static Func<T> GetAccessor(Expression expression)
        {
            ConstantExpression root;
            var members = GetMemberChain(expression, out root);

            if (root == null) return null;

            if (root.Value == null) return () => null;

            var nullChecks = members
                .Select((m, i) => members.Take(i).Aggregate((Expression)root, Expression.MakeMemberAccess))
                .Where(a => a.Type.IsClass)
                .Select(a => Expression.NotEqual(a, Expression.Constant(null, a.Type)))
                .Aggregate(Expression.AndAlso);

            var returnLabel = Expression.Label(typeof(T));
            var lambda = Expression.Lambda<Func<T>>(
                Expression.Block(
                    Expression.IfThen(nullChecks, Expression.Return(returnLabel, Expression.Convert(expression, typeof(T)))),
                    Expression.Label(returnLabel, Expression.Constant(null, typeof(T)))));

            return lambda.Compile();
        }

        private static IEnumerable<MemberInfo> GetMemberChain(Expression expression, out ConstantExpression root)
        {
            var members = new Stack<MemberInfo>(16);

            var node = expression;

            root = null;

            while (node != null)
                switch (node.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        var memberExpr = (MemberExpression)node;
                        members.Push(memberExpr.Member);
                        node = memberExpr.Expression;
                        break;

                    case ExpressionType.Constant:
                        root = (ConstantExpression)node;
                        node = null;
                        break;

                    default:
                        throw new NotSupportedException(node.NodeType.ToString());
                }

            return members;
        }
    }
}