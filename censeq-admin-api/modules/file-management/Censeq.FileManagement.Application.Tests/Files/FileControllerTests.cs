using Censeq.FileManagement.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using Xunit;

namespace Censeq.FileManagement.Application.Tests.Files;

public class FileControllerTests
{
    [Fact]
    public void UploadAvatarAsync_ShouldBindFormDto_InsteadOfDirectFormFileParameter()
    {
        var method = typeof(FileController).GetMethod(nameof(FileController.UploadAvatarAsync));
        method.ShouldNotBeNull();

        var parameter = method.GetParameters().ShouldHaveSingleItem();
        parameter.ParameterType.ShouldNotBe(typeof(IFormFile));
        parameter.GetCustomAttributes(typeof(FromFormAttribute), inherit: true).ShouldHaveSingleItem();

        var fileProperty = parameter.ParameterType.GetProperty("File");
        fileProperty.ShouldNotBeNull();
        fileProperty.PropertyType.ShouldBe(typeof(IFormFile));
    }
}
