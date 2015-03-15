using IgniterPart.SDK.SystemMock;
using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace IgniterPart.SDK.Core
{
    /// <summary>
    /// Must be static
    /// </summary>
    public class PropertyChangedEventManagerProxy
    {
        private readonly NotifyPropertyChangedProxy _notifyPropertyChangedProxy;

        // We need to hold on to this ref to keep it from getting GC'd
        private readonly IWeakEventListener _weakEventListener;

        public PropertyChangedEventManagerProxy(IPropertyChangedEventManager propertyChangedEventManager)
        {
            _notifyPropertyChangedProxy = new NotifyPropertyChangedProxy();
            _weakEventListener = new WeakListenerStub();

            propertyChangedEventManager.AddListener(_notifyPropertyChangedProxy, _weakEventListener, string.Empty);
        }

        public void RaisePropertyChanged(object sender, string propertyName)
        {
            _notifyPropertyChangedProxy.Raise(sender, new PropertyChangedEventArgs(propertyName));
        }


        private class NotifyPropertyChangedProxy : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged = delegate { };

            public void Raise(object sender, PropertyChangedEventArgs e)
            {
                PropertyChanged(sender, e);
            }
        }

        private class WeakListenerStub : IWeakEventListener
        {
            public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) { return false; }
        }
    }
}
