using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Igniter.Core
{
    class NotifyingMemberWatcher : Watcher<INotifyPropertyChanged>
    {
        private readonly string _memberName;

        public NotifyingMemberWatcher(MemberExpression notifyingMember)
            : base(notifyingMember.Expression)
        {
            _memberName = notifyingMember.Member.Name;
        }

        protected override IDisposable Subscribe(INotifyPropertyChanged notifier)
        {
            notifier.PropertyChanged += OnNotifierPropertyChanged;
            return Disposable.Create(() => notifier.PropertyChanged -= OnNotifierPropertyChanged);
        }

        private void OnNotifierPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || string.Equals(e.PropertyName, _memberName, StringComparison.Ordinal))
                OnChanged();
        }
    }
}