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
using IgniterPart.TelerikProvider;
using Microsoft.Silverlight.Testing.UnitTesting.Metadata.VisualStudio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace SilverlightUnitTestsApp
{
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void TestTest()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestNinjectInjection()
        {
            var kernel = new StandardKernel(
                new TelerikProviderModule());

            var c2 =kernel.Get<Class2>();

            Assert.AreEqual(c2.C1Name, "Class1");
        }
    }
}
