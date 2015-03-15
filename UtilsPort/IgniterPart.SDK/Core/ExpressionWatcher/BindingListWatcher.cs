using System;
using System.Linq.Expressions;

namespace Igniter.Core
{
    class BindingListWatcher : Watcher<IBindingList>
    {
        public BindingListWatcher(Expression accessor) : base(accessor) { }

        protected override IDisposable Subscribe(IBindingList notifier)
        {
            notifier.ListChanged += OnNotifierListChanged;
            return Disposable.Create(() => notifier.ListChanged -= OnNotifierListChanged);
        }

        private void OnNotifierListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                case ListChangedType.ItemDeleted:
                case ListChangedType.ItemMoved:
                case ListChangedType.Reset:
                    OnChanged();
                    break;
            }
        }
    }
}