﻿using System;
using System.ComponentModel;
using System.Linq.Expressions;
using IgniterPart.SDK.ReactiveMock;

//using System.Reactive.Disposables;
//using System.Reactive.Linq;

namespace Igniter
{
    /// <summary>
    /// Provides extension methods for subscribing to <see cref="INotifyPropertyChanged"/> and
    /// <see cref="INotifyPropertyChanging"/> in a strongly typed manner.
    /// </summary>
    public static class NotifyPropertyChangedExtensions
    {
        /// <summary>
        /// Subscribes the given handler to the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        /// <typeparam name="TSource">The type of the object providing the event.</typeparam>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="source">The object providing the event.</param>
        /// <param name="propertySelector">A selector taking the given object and selecting the property to subscribe.</param>
        /// <param name="onChanged">The handler to call when the property changes.</param>
        /// <returns>A subscription token that, when disposed, will unsubscribe the handler.</returns>
        public static IDisposable SubscribeToPropertyChanged<TSource, TProp>(this TSource source, Expression<Func<TSource, TProp>> propertySelector, PropertyChangedEventHandler onChanged)
            where TSource : INotifyPropertyChanged
        {
            if (source == null) throw new ArgumentNullException("source");
            if (propertySelector == null) throw new ArgumentNullException("propertySelector");
            if (onChanged == null) throw new ArgumentNullException("onChanged");

            var subscribedPropertyName = ExpressionUtil.GetPropertyName(propertySelector);

            PropertyChangedEventHandler handler = (s, e) =>
            {
                if (string.Equals(e.PropertyName, subscribedPropertyName, StringComparison.InvariantCulture))
                    onChanged(s, e);
            };

            source.PropertyChanged += handler;

            return AnonymousDisposable.Create(() => source.PropertyChanged -= handler);
        }

        /// <summary>
        /// Subscribes the given handler to the <see cref="INotifyPropertyChanging.PropertyChanging"/> event.
        /// </summary>
        /// <typeparam name="TSource">The type of the object providing the event.</typeparam>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="source">The object providing the event.</param>
        /// <param name="propertySelector">A selector taking the given object and selecting the property to subscribe.</param>
        /// <param name="onChanging">The handler to call when the property is changing.</param>
        /// <returns>A subscription token that, when disposed, will unsubscribe the handler.</returns>
        public static IDisposable SubscribeToPropertyChanging<TSource, TProp>(this TSource source, Expression<Func<TSource, TProp>> propertySelector, PropertyChangingEventHandler onChanging)
            where TSource : INotifyPropertyChanging
        {
            if (source == null) throw new ArgumentNullException("source");
            if (propertySelector == null) throw new ArgumentNullException("propertySelector");
            if (onChanging == null) throw new ArgumentNullException("onChanged");

            var subscribedPropertyName = ExpressionUtil.GetPropertyName(propertySelector);

            PropertyChangingEventHandler handler = (s, e) =>
            {
                if (string.Equals(e.PropertyName, subscribedPropertyName, StringComparison.InvariantCulture))
                    onChanging(s, e);
            };

            source.PropertyChanging += handler;

            return AnonymousDisposable.Create(() => source.PropertyChanging -= handler);
        }

        ///// <summary>
        ///// Gets a stream of changes to a property triggered by  <see cref="INotifyPropertyChanged.PropertyChanged"/>.
        ///// </summary>
        ///// <typeparam name="TSource">The type of the object providing the event.</typeparam>
        ///// <typeparam name="TProp">The type of the property.</typeparam>
        ///// <param name="source">The object providing the event.</param>
        ///// <param name="propertySelector">A selector taking the given object and selecting the property to subscribe.</param>
        ///// <returns>A stream of new values being set to the given property.</returns>
        //public static IObservable<TProp> GetPropertyChanges<TSource, TProp>(this TSource source, Expression<Func<TSource, TProp>> propertySelector)
        //    where TSource : INotifyPropertyChanged
        //{
        //    var propertyName = ExpressionUtil.GetPropertyName(propertySelector);

        //    var selector = new Lazy<Func<TSource, TProp>>(propertySelector.Compile, isThreadSafe: true);

        //    return Observable
        //        .FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
        //            h => new PropertyChangedEventHandler((s, e) => h(e)),
        //            h => source.PropertyChanged += h,
        //            h => source.PropertyChanged -= h)
        //        .Where(e => string.Equals(propertyName, e.PropertyName, StringComparison.Ordinal))
        //        .Select(e =>
        //        {
        //            var typedArgs = e as PropertyChangedEventArgs<TProp>;
        //            return typedArgs != null ? typedArgs.NewValue : selector.Value(source);
        //        });
        //}

    }
}
