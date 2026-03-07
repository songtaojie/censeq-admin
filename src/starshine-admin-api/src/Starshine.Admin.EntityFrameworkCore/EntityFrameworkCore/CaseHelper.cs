using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starshine.Admin.EntityFrameworkCore
{
    /// <summary>
    /// 命名法帮助类
    /// </summary>
    public static class CaseHelper
    {
        /// <summary>
        /// 大写字母
        /// </summary>
        const string _upperLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Snake命名法符号
        /// </summary>
        const char _snakeSymbol = '_';

        /// <summary>
        /// Snake命名法转Pascal命名法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(str))]
        public static string? SnakeCaseToPascalCase(this string? str)
        {
            return SnakeCaseToOtherCase(str, false);
        }

        /// <summary>
        /// Snake命名法转Camel命名法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(str))]
        public static string? SnakeCaseToCamelCase(this string? str)
        {
            return SnakeCaseToOtherCase(str, true);
        }

        /// <summary>
        /// 转换为Snake命名法
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(s))]
        public static string? ToSnakeCase(this string? s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            if (s.Length == 1)
            {
                return s.ToLowerInvariant();
            }

            var sb = new System.Text.StringBuilder(s.Length).Append(char.ToLowerInvariant(s[0]));
            for (int i = 1; i < s.Length; i++)
            {
                if (_upperLetters.Contains(s[i]))
                {
                    sb.Append(_snakeSymbol).Append(char.ToLowerInvariant(s[i]));
                }
                else
                {
                    sb.Append(s[i]);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 转换为Pascal命名法
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(s))]
        public static string? ToPascalCase(string? s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            if (s.Contains(_snakeSymbol))
            {
                return SnakeCaseToPascalCase(s);
            }
            return ToCase(s, true);
        }

        /// <summary>
        /// 转换为Camel命名法
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(s))]
        public static string? ToCamelCase(string? s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            if (s.Contains(_snakeSymbol))
            {
                StringBuilder sb = new(s.Length);
                bool toLower = false;
                foreach (var c in s)
                {
                    if (c == _snakeSymbol)
                    {
                        toLower = true;
                    }
                    else
                    {
                        if (toLower)
                        {
                            sb.Append(char.ToLowerInvariant(c));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        toLower = false;
                    }
                }
                return sb.ToString();
            }
            return ToCase(s, false);
        }

        [return: NotNullIfNotNull(nameof(str))]
        private static string? SnakeCaseToOtherCase(string? str, bool isCamelCase)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            StringBuilder sb = new(str.Length);
            bool isSnakeSymbol = false;
            foreach (var c in str)
            {
                if (c == _snakeSymbol)
                {
                    isSnakeSymbol = true;
                }
                else
                {
                    if (isSnakeSymbol)
                    {
                        sb.Append(isCamelCase ? char.ToLowerInvariant(c) : char.ToUpperInvariant(c));
                    }
                    else
                    {
                        sb.Append(c);
                    }
                    isSnakeSymbol = false;
                }
            }
            return sb.ToString();
        }

        private static string ToCase(string s, bool isUpper = false)
        {
            if (s.Length == 1)
            {
                return isUpper ? s.ToUpperInvariant() : s.ToLowerInvariant();
            }
            return string.Create(s.Length, s, (destination, startIndex) =>
            {
                destination[0] = isUpper ? char.ToUpperInvariant(s[0]) : char.ToLowerInvariant(s[0]);
                s.AsSpan(1).CopyTo(destination[1..]);
                s.AsSpan(1).CopyTo(destination[1..]);
            });
        }

        /// <summary>
        /// 将 ID 转换为 Base62 字符串以进一步压缩
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string ToBase62(long id)
        {
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder sb = new();
            while (id > 0)
            {
                sb.Append(chars[(int)(id % 62)]);
                id /= 62;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将 ID 转换为 Base36 字符串以进一步压缩
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string ToBase36(long id)
        {
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder sb = new();
            while (id > 0)
            {
                sb.Append(chars[(int)(id % 36)]);
                id /= 36;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将 ID 转换为 Base26 字符串以进一步压缩
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string ToBase26(long id)
        {
            StringBuilder sb = new();
            while (id > 0)
            {
                sb.Append(_upperLetters[(int)(id % 26)]);
                id /= 26;
            }
            return sb.ToString();
        }
    }
}
