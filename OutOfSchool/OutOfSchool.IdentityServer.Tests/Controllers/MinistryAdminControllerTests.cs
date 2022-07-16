using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OutOfSchool.IdentityServer.Controllers;
using OutOfSchool.Common;
using OutOfSchool.Common.Models;
using OutOfSchool.IdentityServer.Services.Interfaces;

namespace OutOfSchool.IdentityServer.Tests.Controllers;

[TestFixture]
public class MinistryAdminControllerTests
{
    private MinistryAdminController ministryAdminController;
    private readonly Mock<ILogger<MinistryAdminController>> fakeLogger;
    private readonly Mock<IMinistryAdminService> fakeMinistryAdminService;
    private Mock<HttpContext> fakehttpContext;

    public MinistryAdminControllerTests()
    {
        fakeLogger = new Mock<ILogger<MinistryAdminController>>();
        fakeMinistryAdminService = new Mock<IMinistryAdminService>();
    
        fakehttpContext = new Mock<HttpContext>();
        
        ministryAdminController = new MinistryAdminController(
            fakeLogger.Object,
            fakeMinistryAdminService.Object
        );
        ministryAdminController.ControllerContext.HttpContext = fakehttpContext.Object;
    }

    [SetUp]
    public void Setup()
    {
        var fakeMinistryAdminDto = new CreateMinistryAdminDto()
        {
            FirstName = "fakeFirstName",
            LastName = "fakeLastName",
            Email = "fake@email.com",
            PhoneNumber = "11-222-33-44",
        };

        var fakeResponseDto = new ResponseDto()
        {
            IsSuccess = true,
            Result = fakeMinistryAdminDto
        };

        fakeMinistryAdminService.Setup(s => s.CreateMinistryAdminAsync(It.IsAny<CreateMinistryAdminDto>(),
            It.IsAny<IUrlHelper>(), It.IsAny<string>(),
            It.IsAny<string>())).ReturnsAsync(fakeResponseDto);

        fakeMinistryAdminService.Setup(s => s.DeleteMinistryAdminAsync(It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(fakeResponseDto);
        
        fakeMinistryAdminService.Setup(s => s.BlockMinistryAdminAsync(It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(fakeResponseDto);
        
        var fakeHttpContext = new Mock<HttpContext>();
        fakeHttpContext.Setup(s => s.Request.Headers[It.IsAny<string>()]).Returns("Ok");
        
        ministryAdminController.ControllerContext.HttpContext = fakeHttpContext.Object;
    }

    [Test]
    public async Task Create_WithInvalidModel_ReturnsNotSuccessResponseDto()
    {
        // Arrange
        ministryAdminController.ModelState.AddModelError("fakeKey", "Model is invalid");

        // Act
        var result = await ministryAdminController.Create(new CreateMinistryAdminDto());

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(false, result.IsSuccess);
    }

    [Test]
    public async Task Create_WithValidModel_ReturnsSuccessResponseDto()
    {
        // Arrange
        ministryAdminController.ModelState.Clear();
        
        // Act
        var result = await ministryAdminController.Create(new CreateMinistryAdminDto());

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(true, result.IsSuccess);
    }
    
    [Test]
    public async Task Delete_WithValidModel_ReturnsSuccessResponseDto()
    {
        // Arrange
        
        // Act
        var result = await ministryAdminController.Delete("fakeAdminId");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(true, result.IsSuccess);
        Assert.AreEqual("fakeFirstName", ((CreateMinistryAdminDto)result.Result).FirstName);
    }
    
    [Test]
    public async Task Block_WithValidModel_ReturnsSuccessResponseDto()
    {
        // Arrange
        
        // Act
        var result = await ministryAdminController.Block("fakeAdminId");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(true, result.IsSuccess);
        Assert.AreEqual("fakeFirstName", ((CreateMinistryAdminDto)result.Result).FirstName);
    }
}