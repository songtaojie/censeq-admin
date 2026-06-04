using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// OpenIddict Server 构建器扩展方法。
/// </summary>
public static class OpenIddictServerBuilderExtensions
{
    /// <summary>
    /// 添加生产环境加密和签名证书。
    /// </summary>
    /// <param name="builder">构建器。</param>
    /// <param name="fileName">文件Name。</param>
    /// <param name="passPhrase">密码短语。</param>
    /// <returns>操作结果。</returns>
    public static OpenIddictServerBuilder AddProductionEncryptionAndSigningCertificate(this OpenIddictServerBuilder builder, string fileName, string passPhrase)
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException($"Signing Certificate couldn't found: {fileName}");
        }

        var certificate = new X509Certificate2(fileName, passPhrase);
        builder.AddSigningCertificate(certificate);
        builder.AddEncryptionCertificate(certificate);
        return builder;
    }
}
