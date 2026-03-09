using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;

namespace Censeq.Framework.EntityFrameworkCore
{
    public class CenseqCommonDbProperties
    {
        /// <summary>
        /// This table prefix is shared by most of the ABP modules.
        /// You can change it to set table prefix for all modules using this.
        /// 
        /// Default value: "Censeq".
        /// </summary>
        public static string DbTablePrefix { get; set; } = "Censeq";

        /// <summary>
        /// This schema is shared by most of the ABP modules.
        /// You can change it to set schema for all modules using this.
        /// </summary>
        public static string? DbSchema { get; set; } = AbpCommonDbProperties.DbSchema;
    }
}
