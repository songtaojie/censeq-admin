using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starshine.Admin.Consts
{
    /// <summary>
    /// 租户属性
    /// </summary>
    public static class TenantConsts
    {
        /// <summary>
        /// Default value: 64
        /// </summary>
        public static int MaxNameLength { get; set; } = 64;

        /// <summary>
        /// Default value: 128
        /// </summary>
        public static int MaxPasswordLength { get; set; } = 128;

        /// <summary>
        /// Default value: 256
        /// </summary>
        public static int MaxAdminEmailAddressLength { get; set; } = 256;

        /// <summary>
        /// 租户连接字符串常量
        /// </summary>
        public static class TenantConnectionStringConsts
        {
            /// <summary>
            /// Default value: 64
            /// </summary>
            public static int MaxNameLength { get; set; } = 64;

            /// <summary>
            /// Default value: 1024
            /// </summary>
            public static int MaxValueLength { get; set; } = 1024;
        }
    }
}
