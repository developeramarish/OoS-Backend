﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OutOfSchool.BusinessLogic.Common;
using OutOfSchool.BusinessLogic.Models;
using OutOfSchool.BusinessLogic.Models.StudySubjects;
using OutOfSchool.BusinessLogic.Services;
using OutOfSchool.BusinessLogic.Services.ProviderServices;
using OutOfSchool.BusinessLogic.Util;
using OutOfSchool.BusinessLogic.Util.Mapping;
using OutOfSchool.Services;
using OutOfSchool.Services.Models;
using OutOfSchool.Services.Repository.Base;
using OutOfSchool.Services.Repository.Base.Api;
using OutOfSchool.Tests.Common;
using OutOfSchool.Tests.Common.DbContextTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OutOfSchool.WebApi.Tests.Services;
[TestFixture]
public class StudySubjectServiceTests
{
    private DbContextOptions<OutOfSchoolDbContext> options;
    private OutOfSchoolDbContext context;
    private StudySubjectService service;
    private IEntityRepositorySoftDeleted<Guid, StudySubject> studySubjectRepository;
    private IEntityRepository<long, Language> languageRepository;
    private Mock<IProviderService> providerService;
    private Mock<ILogger<StudySubjectService>> logger;
    private IMapper mapper;
    private Guid providerId;

    [SetUp]
    public void SetUp()
    {
        var builder = new DbContextOptionsBuilder<OutOfSchoolDbContext>().UseInMemoryDatabase(
            databaseName: "OutOfSchoolTestDB");

        options = builder.Options;
        context = new TestOutOfSchoolDbContext(options);

        studySubjectRepository = new EntityRepositorySoftDeleted<Guid, StudySubject>(context);
        languageRepository = new EntityRepository<long, Language>(context);
        providerId = Guid.NewGuid();

        providerService = new Mock<IProviderService>();
        logger = new Mock<ILogger<StudySubjectService>>();
        mapper = TestHelper.CreateMapperInstanceOfProfileTypes<CommonProfile, MappingProfile>();

        service = new StudySubjectService(studySubjectRepository, languageRepository, providerService.Object, logger.Object, mapper);

        SeedDatabase();
    }
    
    [Test]
    public async Task GetByFilter_ReturnsAListOfStudySubjects_WhenSearchStringIsEmpty()
    {
        // Arrange
        var expected = StudySubjects();

        // Act
        var result = await service.GetByFilter(providerId, null);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Entities.First().Id, Is.EqualTo(expected.First().Id));
        Assert.That(result.TotalAmount, Is.EqualTo(expected.Count));
        Assert.IsInstanceOf<SearchResult<StudySubjectDto>>(result);
    }
    
    [Test]
    public async Task GetByFilter_ReturnsAListOfFilteredStudySubjects_WhenSearchStringIsSpecified()
    {
        // Arrange
        var expected = StudySubjects().FirstOrDefault(x => x.NameInInstructionLanguage == "test");
        var filter = new SearchStringFilter() { SearchString = "test" };

        // Act
        var result = await service.GetByFilter(providerId, filter);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Entities.First().Id, Is.EqualTo(expected.Id));
        Assert.IsInstanceOf<SearchResult<StudySubjectDto>>(result);
    }
    
    [Test]
    public async Task GetByFilter_ReturnsEmptyList_WhenSearchStringDoesNotMatch()
    {
        // Arrange
        var filter = new SearchStringFilter() { SearchString = "nonexistent" };

        // Act
        var result = await service.GetByFilter(providerId, filter);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Entities, Is.Empty);
    }
    
    [Test]
    public async Task GetById_ReturnsNull_WhenStudySubjectDoesNotExist()
    {
        // Arrange
        var id = Guid.Empty;

        // Act
        var result = await service.GetById(id, providerId);

        // Assert
        Assert.That(result, Is.Null);
    }
    
    [Test]
    public async Task GetById_ReturnsStudySubject_WhenItExists()
    {
        // Arrange
        var id = new Guid("eb49a87c-7042-45e9-a76b-79ebd98b6b16");
        var expected = StudySubjects().FirstOrDefault(x => x.Id == id);

        // Act
        var result = await service.GetById(id, providerId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(expected.Id));
        Assert.IsInstanceOf<StudySubjectDto>(result);
    }
    
    [Test]
    public async Task Create_ReturnsNull_WhenDtoIsNull()
    {
        // Arrange
        StudySubjectCreateUpdateDto dto = null;

        // Act
        var result = await service.Create(dto, providerId);

        // Assert
        Assert.That(result, Is.Null);
    }
    
    [Test]
    public async Task Create_CreatesStudySubject_WhenDtoIsValid()
    {
        // Arrange
        var dto = new StudySubjectCreateUpdateDto()
        {
            Id = Guid.NewGuid(),
            IsPrimaryLanguageUkrainian = true,
            LanguagesSelection = new List<LanguagesSelection>()
            {
                new LanguagesSelection()
                {
                    Id = 2,
                    IsPrimary = true
                }
            },
            NameInInstructionLanguage = "ім'я",
            NameInUkrainian = "ім'я",
        };

        // Act
        var result = await service.Create(dto, providerId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(dto.Id));
        Assert.IsInstanceOf<StudySubjectDto>(result);
    }

    [Test]
    public async Task Create_AddsUkrainianLanguageToInvalidEntity_WhenUkrainianMarkedAsPrimaryLanguage()
    {
        // Arrange
        var dto = new StudySubjectCreateUpdateDto()
        {
            Id = Guid.NewGuid(),
            IsPrimaryLanguageUkrainian = true,
            LanguagesSelection = new List<LanguagesSelection>()
            {
                new LanguagesSelection()
                {
                    Id = 1,
                    IsPrimary = true
                }
            },
            NameInInstructionLanguage = "ім'я",
            NameInUkrainian = "ім'я",
        };

        // Act
        var result = await service.Create(dto, providerId).ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(dto.Id));
        Assert.That(result.Languages.Any(l => l.Id == 2));
        Assert.IsInstanceOf<StudySubjectDto>(result);
    }

    [Test]
    public async Task Update_ReturnsResultFailed_WhenDtoIsNull()
    {
        // Arrange
        StudySubjectCreateUpdateDto dto = null;

        // Act
        var result = await service.Update(dto, providerId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.OperationResult.Errors.FirstOrDefault().Code, Is.EqualTo("400"));
        Assert.IsInstanceOf<Result<StudySubjectDto>>(result);
    }
    
    [Test]
    public async Task Update_ReturnsResultFailed_WhenStudySubjectWithIdDoesNotExist()
    {
        // Arrange
        var dto = new StudySubjectCreateUpdateDto()
        {
            Id = Guid.Empty,
            IsPrimaryLanguageUkrainian = true,
            LanguagesSelection = new List<LanguagesSelection>()
            {
                new LanguagesSelection()
                {
                    Id = 2,
                    IsPrimary = true
                }
            },
            NameInInstructionLanguage = "ім'я",
            NameInUkrainian = "ім'я"
        };

        // Act
        var result = await service.Update(dto, providerId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.OperationResult.Errors.FirstOrDefault().Code, Is.EqualTo("404"));
        Assert.IsInstanceOf<Result<StudySubjectDto>>(result);
    }
    
    [Test]
    public async Task Update_ReturnsResultSuccess_WhenEntityWasUpdated()
    {
        // Arrange
        var dto = new StudySubjectCreateUpdateDto()
        {
            Id = new Guid("eb49a87c-7042-45e9-a76b-79ebd98b6b16"),
            IsPrimaryLanguageUkrainian = true,
            LanguagesSelection = new List<LanguagesSelection>()
            {
                new LanguagesSelection()
                {
                    Id = 2,
                    IsPrimary = true
                }
            },
            NameInInstructionLanguage = "ім'я",
            NameInUkrainian = "ім'я"
        };

        // Act
        var result = await service.Update(dto, providerId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value.Id, Is.EqualTo(dto.Id));
        Assert.IsInstanceOf<Result<StudySubjectDto>>(result);
    }
    
    [Test]
    public async Task Delete_ReturnsResutlFailed_WhenStudySubjectWithIdDoesNotExist()
    {
        // Arrange
        var id = Guid.Empty;

        // Act
        var result = await service.Delete(id, providerId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.OperationResult.Errors.FirstOrDefault().Code, Is.EqualTo("404"));
        Assert.IsInstanceOf<Result<StudySubjectDto>>(result);
    }
    
    [Test]
    public async Task Delete_DeletesStudySubject_WhenEntityExists()
    {
        // Arrange
        var id = new Guid("eb49a87c-7042-45e9-a76b-79ebd98b6b16");
        StudySubject result;

        // Act
        await service.Delete(id, providerId);
        using var ctx = new TestOutOfSchoolDbContext(options);
        {       
            result = await ctx.StudySubjects.FirstOrDefaultAsync(x => x.Id == id);
        }

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsDeleted, Is.True);
    }
    
    private void SeedDatabase()
    {
        using var ctx = new TestOutOfSchoolDbContext(options);
        {
            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();

            ctx.StudySubjects.AddRange(StudySubjects());

            ctx.SaveChanges();
        }
    }

    private List<StudySubject> StudySubjects()
    {
        return new List<StudySubject>()
        {
            new StudySubject()
            {
                Id = new Guid("eb49a87c-7042-45e9-a76b-79ebd98b6b16"),
                NameInInstructionLanguage = "тест",
                NameInUkrainian = "тест",
                PrimaryLanguageId = 2,
                IsPrimaryLanguageUkrainian = true
            },
            new StudySubject()
            {
                Id = new Guid("4ca6f3af-5d02-4c16-b4b2-e202c71470f4"),
                NameInInstructionLanguage = "test",
                NameInUkrainian = "тест",
                PrimaryLanguageId = 1,
                IsPrimaryLanguageUkrainian = false
            }
        };
    }
    
}
