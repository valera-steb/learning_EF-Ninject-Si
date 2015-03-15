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
using IgniterPart.SDK.SystemMock;
using IgniterPartIWeakEventListener = IgniterPart.SDK.SystemMock.IWeakEventListener;
using IWeakEventListener = System.Windows.IWeakEventListener;

namespace IgniterPart.TelerikProvider
{
    public class PropertyChangedEventManagerProvider : IPropertyChangedEventManager
    {        
        public void AddListener(INotifyPropertyChanged source, IgniterPartIWeakEventListener listener, string propertyName)
        {            
            PropertyChangedEventManager.AddListener(source, new ListenerProvider{Listener = listener}, propertyName);
        }

        private class ListenerProvider : IWeakEventListener
        {
            public IgniterPartIWeakEventListener Listener;

            public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
            {
                return Listener.ReceiveWeakEvent(managerType, sender, e);
            }
        }
    }
}
