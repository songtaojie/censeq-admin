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
        public static string? DbSchema { get; set; } =  null;

        /// <summary>
        ///  数据库链接字符串
        /// </summary>
        public const string ConnectionStringName = "Default";
    }
}
