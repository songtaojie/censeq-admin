using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starshine.Admin.EntityFrameworkCore.Modeling
{
    /// <summary>
    /// 通用的值比较器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericValueComparer<T>() : ValueComparer<T>((d1, d2) => IsEquals(d1, d2), c => GetHasCode(c))
    {
        private static int GetHasCode(T c)
        {
            if (c is null)
            {
                return 0;
            }
            if (c is IEnumerable enumerator)
            {
                int hashCode = 0;
                foreach (var item in enumerator)
                {
                    hashCode = HashCode.Combine(hashCode, item.GetHashCode());
                }
                return hashCode;
            }
            else
            {
                return c.GetHashCode();
            }
        }

        private static bool IsEquals(T? d1, T? d2)
        {
            if (d1 is null && d2 is null)
            {
                return true;
            }
            if (d1 is null || d2 is null)
            {
                return false;
            }
            return d1.Equals(d2);
        }
    }
}
