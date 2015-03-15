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
using IgniterPart.SDK.Core;
using IgniterPart.SDK.SystemMock;
using Ninject.Modules;

namespace IgniterPart.TelerikProvider
{
    public class TelerikProviderModule : NinjectModule
    {
        public override void Load()
        {
            Bind<Class1>().ToSelf().InSingletonScope();
            Bind<Class2>().ToSelf();


            Bind<PropertyChangedEventManagerProxy>().ToSelf();
            Bind<IPropertyChangedEventManager>().To<PropertyChangedEventManagerProvider>();
        }
    }
}
