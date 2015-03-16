using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Igniter.Reflections
{
    public class Helper
    {
        public static MethodInfo GetHiddenMethod(object source, string methodName)
        {
            var hiddens = source.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            return hiddens.FirstOrDefault(x => x.Name == methodName);
        }

        public static object GetBaseValueSource(DependencyObject source, DependencyProperty property)
        {
            var methodInfo = GetHiddenMethod(source, "GetValueSource");
            var result = methodInfo.Invoke(source, new object[] { property, null, false });

            return result;
        }
    }
}
