using System;
using System.Collections.Specialized;
using System.Linq.Expressions;

namespace Igniter.Core
{
    class NotifyingCollectionWatcher : Watcher<INotifyCollectionChanged>
    {
        public NotifyingCollectionWatcher(Expression expression) : base(expression) { }

        protected override IDisposable Subscribe(INotifyCollectionChanged notifier)
        {
            notifier.CollectionChanged += OnNotifierCollectionChanged;
            return Disposable.Create(() => notifier.CollectionChanged -= OnNotifierCollectionChanged);
        }

        private void OnNotifierCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnChanged();
        }
    }
}