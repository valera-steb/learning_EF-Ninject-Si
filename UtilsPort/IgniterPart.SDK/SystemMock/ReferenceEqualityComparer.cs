using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;

namespace Igniter.SystemMock
{
  internal class ReferenceEqualityComparer : IEqualityComparer<object>
  {
    public ReferenceEqualityComparer()
    {
    }

    bool IEqualityComparer<object>.Equals(object o1, object o2)
    {
      return object.ReferenceEquals(o1, o2);
    }

    public int GetHashCode(object o)
    {
      return o.GetHashCode();
    }
  }    
}
