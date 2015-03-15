using System;
using System.Collections.Generic;
using System.Linq.Expressions;

//using System.Reactive.Disposables;
//using System.Reactive.Linq;

namespace Igniter.Core
{
    public sealed class ExpressionWatcher : IDisposable
    {
        private readonly List<IWatcher> _watchers = new List<IWatcher>();

        public ExpressionWatcher(Expression expression)
        {
            SubscribeToChanges(expression);
        }

        //public static IObservable<TResult> WatchResult<TResult>(Expression<Func<TResult>> expression)
        //{
        //    return WatchResult(expression, f => f());
        //}

        //public static IObservable<TResult> WatchResult<TDelegate, TResult>(Expression<TDelegate> expression, Func<TDelegate, TResult> invoker)
        //{
        //    var lazyFunc = new Lazy<TDelegate>(expression.Compile, LazyThreadSafetyMode.ExecutionAndPublication);

        //    return Observable
        //        .Using(() => new ExpressionWatcher(expression), watcher => Observable
        //            .FromEventPattern(
        //                h => watcher.ExpressionChanged += h, 
        //                h => watcher.ExpressionChanged -= h))
        //        .Select(e => invoker(lazyFunc.Value));
        //}

        public event EventHandler ExpressionChanged = delegate { };


        /// <summary>
        /// Analyzes an expression for its notifying componetns and subscribes to their changes.
        /// </summary>
        /// <param name="expression">The expression to analyze.</param>
        private void SubscribeToChanges(Expression expression)
        {
            var visitor = new NotifierFindingExpressionVisitor();
            visitor.Visit(expression);

            foreach (var notifier in visitor.NotifyingMembers)
                _watchers.Add(new NotifyingMemberWatcher(notifier));

            foreach (var notifier in visitor.NotifyingCollections)
                _watchers.Add(new NotifyingCollectionWatcher(notifier));

            foreach (var notifier in visitor.BindingLists)
                _watchers.Add(new BindingListWatcher(notifier));

            foreach (var pair in visitor.DependencyProperties)
                _watchers.Add(new DependencyPropertyWatcher(pair.Key.Expression, pair.Value));

            foreach (var watcher in _watchers)
            {
                watcher.Changed += OnWatcherChanged;
                watcher.SubscribeToCurrentNotifier();
            }
        }


        private void OnWatcherChanged(object sender, EventArgs e)
        {
            ExpressionChanged(this, EventArgs.Empty);

            foreach (var watcher in _watchers)
                watcher.SubscribeToCurrentNotifier();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            foreach (var watcher in _watchers)
                watcher.Dispose();
        }

    }
}
