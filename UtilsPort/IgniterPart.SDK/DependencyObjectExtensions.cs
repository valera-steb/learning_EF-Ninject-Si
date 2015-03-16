﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using Igniter.Reflections;
using IgniterPart.SDK.ReactiveMock;

namespace Igniter
{
    /// <summary>
    /// Extensions for working with <see cref="DependencyObject"/>s.
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Subscribes to changes of a dependency property.
        /// </summary>
        /// <param name="source">The source dependency object.</param>
        /// <param name="property">The property to monitor for changes.</param>
        /// <param name="handler">The handler to invoke when a change occurs.</param>
        /// <returns>A token that, when disposed, unsubscribes the handler.</returns>
        /// <exception cref="System.ArgumentNullException">source, property</exception>
        public static IDisposable SubscribeToDependencyPropertyChanges(this DependencyObject source, DependencyProperty property, DependencyPropertyChangedEventHandler handler)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (property == null) throw new ArgumentNullException("property");

            var proxies = GetEventProxies(source);

            if (proxies == null)
                SetEventProxies(source, proxies = new Dictionary<DependencyProperty, EventProxy>());

            EventProxy proxy;
            if (!proxies.TryGetValue(property, out proxy))
            {
                proxies.Add(property, proxy = new EventProxy(source));
                BindingOperations.SetBinding(proxy, EventProxy.ValueProperty, new Binding { Path = new PropertyPath("(0)", property), Source = source });
            }

            proxy.ValueChanged += handler;

            return AnonymousDisposable.Create(() => proxy.ValueChanged -= handler);
        }
        

        //TODO: test method
        public static bool IsPropertySet(this DependencyObject source, DependencyProperty property)
        {
//            var valSource = DependencyPropertyHelper.GetValueSource(source, property);
            var x = Helper.GetBaseValueSource(source, property).ToString();

  //          return valSource.BaseValueSource != BaseValueSource.Unknown && valSource.BaseValueSource != BaseValueSource.Default;
            return x != "Unknown" && x != "Default";

        }

        #region [Attached] private static Dictionary<DependencyProperty, EventProxy> EventProxies { get; set; }

        private static Dictionary<DependencyProperty, EventProxy> GetEventProxies(DependencyObject obj)
        {
            return (Dictionary<DependencyProperty, EventProxy>)obj.GetValue(EventProxiesProperty);
        }

        private static void SetEventProxies(DependencyObject obj, Dictionary<DependencyProperty, EventProxy> value)
        {
            obj.SetValue(EventProxiesProperty, value);
        }

        private static readonly DependencyProperty EventProxiesProperty =
            DependencyProperty.RegisterAttached("EventProxies", typeof(Dictionary<DependencyProperty, EventProxy>), typeof(DependencyObjectExtensions), null);

        #endregion

        private class EventProxy : DependencyObject
        {
            private readonly DependencyObject _target;

            public event DependencyPropertyChangedEventHandler ValueChanged = delegate { };

            public EventProxy(DependencyObject target)
            {
                _target = target;
            }

            #region public object Value { get; set; }

            public object Value
            {
                get { return GetValue(ValueProperty); }
                set { SetValue(ValueProperty, value); }
            }

            public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register("Value", typeof(object), typeof(EventProxy), new PropertyMetadata(OnValueChanged));

            private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                ((EventProxy)sender).OnValueChanged(e);
            }

            private void OnValueChanged(DependencyPropertyChangedEventArgs e)
            {
                ValueChanged(_target, e);
            }

            #endregion
        }
    }
}
