using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starshine.Admin.EntityFrameworkCore
{
    internal static class ConnectionStringParser
    {
        public static string ParseConnectionString(string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("缺少数据库连接字符串配置");
            // 动态替换解决方案根路径  
            if (connectionString.Contains("%SOLUTIONROOT%"))
            {
                var slnPath = GetSolutionDirectoryPath();
                connectionString = connectionString.Replace("%SOLUTIONROOT%", slnPath);
            }
            return connectionString;
        }

        private static string? GetSolutionDirectoryPath()
        {
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

            while (Directory.GetParent(currentDirectory.FullName) != null)
            {
                currentDirectory = Directory.GetParent(currentDirectory.FullName);
                if (currentDirectory == null) return null;
                if (Directory.GetFiles(currentDirectory.FullName).FirstOrDefault(f => f.EndsWith(".sln")) != null)
                {
                    return currentDirectory.FullName;
                }
            }

            return null;
        }
    }
}
