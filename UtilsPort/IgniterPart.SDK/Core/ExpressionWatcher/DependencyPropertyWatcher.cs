using System;
using System.Linq.Expressions;
using System.Windows;

namespace Igniter.Core
{
    class DependencyPropertyWatcher : Watcher<DependencyObject>
    {
        private readonly DependencyProperty _property;

        public DependencyPropertyWatcher(Expression ownerExpression, DependencyProperty property)
            : base(ownerExpression)
        {
            _property = property;
        }

        protected override IDisposable Subscribe(DependencyObject notifier)
        {
            return notifier.SubscribeToDependencyPropertyChanges(_property, OnPropertyChanged);
        }

        private void OnPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnChanged();
        }
    }
}