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
using Igniter;
using IgniterPart.SDK.Core;
using IgniterPart.TelerikProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace SilverlightUnitTestsApp.IgniterPart.TelerikProvider
{
    [TestClass]
    public class BindableBaseTest
    {
        private IKernel kernel;
        private TestBindableType _sut;


        [TestInitialize]
        public void FixureSetup()
        {
            kernel = new StandardKernel(new TelerikProviderModule());
            _sut = kernel.Get<TestBindableType>();
        }

        [TestMethod]
        public void SupportsExplicitInterfaces()
        {
            ((ITestInterface)_sut).MyInterfaceValue = 5;
        }

        [TestMethod]
        public void CanSubscribeToPropertyChanged()
        {
            bool propertyChanged = false;
            var subscription = _sut.SubscribeToPropertyChanged(m => m.MyValue, (s, e) => propertyChanged = true);
            _sut.MyValue = 4;
            Assert.IsTrue(propertyChanged);

            propertyChanged = false;

            subscription.Dispose();

            _sut.MyValue = 5;

            Assert.IsFalse(propertyChanged);
        }

        public class TestBindableType : BindableBase, ITestInterface
        {
            private int _myValue;
            public int MyValue
            {
                get { return _myValue; }
                set { SetProperty(ref _myValue, value); }
            }

            private int _myInterfaceValue;
            int ITestInterface.MyInterfaceValue
            {
                get { return _myInterfaceValue; }
                set { SetProperty(ref _myInterfaceValue, value); }
            }
        }

        private interface ITestInterface
        {
            int MyInterfaceValue { get; set; }
        }
    }
}
