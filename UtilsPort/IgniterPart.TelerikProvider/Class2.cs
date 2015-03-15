using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace IgniterPart.TelerikProvider
{
    public class Class2
    {
        private readonly Class1 _class1;

        public string C1Name { get { return _class1.Name; } }

        public Class2(Class1 class1)
        {
            _class1 = class1;
        }
    }
}
