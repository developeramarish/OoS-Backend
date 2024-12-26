﻿using System.Linq.Expressions;
using AutoMapper;
using OutOfSchool.BusinessLogic.Models;
using OutOfSchool.BusinessLogic.Models.Exported;
using OutOfSchool.BusinessLogic.Services.AverageRatings;
using OutOfSchool.Services.Repository.Api;

namespace OutOfSchool.BusinessLogic.Services;

public class ExternalExportService : IExternalExportService
{
    private const string ProviderIncludes =
        "ProviderSectionItems,Images,Institution,ActualAddress,ActualAddress.CATOTTG.Parent.Parent.Parent.Parent,LegalAddress,LegalAddress.CATOTTG.Parent.Parent.Parent.Parent,Type";

    private const string WorkshopIncludes =
        "WorkshopDescriptionItems,Tags,Address,Address.CATOTTG.Parent.Parent.Parent.Parent,Images,DateTimeRanges,Teachers,InstitutionHierarchy,InstitutionHierarchy.Institution,InstitutionHierarchy.Directions,DefaultTeacher";

    private readonly IProviderRepository providerRepository;
    private readonly IWorkshopRepository workshopRepository;
    private readonly IApplicationRepository applicationRepository;
    private readonly IAverageRatingService averageRatingService;
    private readonly IMapper mapper;
    private readonly ILogger<ExternalExportService> logger;

    public ExternalExportService(
        IProviderRepository providerRepository,
        IWorkshopRepository workshopRepository,
        IApplicationRepository applicationRepository,
        IAverageRatingService averageRatingService,
        IMapper mapper,
        ILogger<ExternalExportService> logger)
    {
        this.providerRepository = providerRepository ?? throw new ArgumentNullException(nameof(providerRepository));
        this.workshopRepository = workshopRepository ?? throw new ArgumentNullException(nameof(workshopRepository));
        this.applicationRepository =
            applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
        this.averageRatingService =
            averageRatingService ?? throw new ArgumentNullException(nameof(averageRatingService));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<SearchResult<ProviderInfoBaseDto>> GetProviders(DateTime updatedAfter,
        OffsetFilter offsetFilter)
    {
        try
        {
            logger.LogDebug("Getting all updated providers started");
            offsetFilter ??= new OffsetFilter();

            Expression<Func<Provider, bool>> filterExpression = updatedAfter == default
                ? provider => !provider.IsDeleted
                : provider => provider.UpdatedAt > updatedAfter ||
                              provider.Workshops.Any(w => w.UpdatedAt > updatedAfter);

            var providers = await providerRepository.Get(
                    offsetFilter.From,
                    offsetFilter.Size,
                    ProviderIncludes,
                    filterExpression)
                .ToListAsync()
                .ConfigureAwait(false);

            var providersDto = providers
                .Select(MapToInfoProviderDto)
                .ToList();

            await FillRatingsForType(providersDto).ConfigureAwait(false);

            var count = await providerRepository.Count(filterExpression);

            var searchResult = new SearchResult<ProviderInfoBaseDto>
            {
                TotalAmount = count,
                Entities = providersDto,
            };

            return searchResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while processing providers");
            throw;
        }
    }

    public async Task<SearchResult<WorkshopInfoBaseDto>> GetWorkshops(DateTime updatedAfter, OffsetFilter offsetFilter)
    {
        try
        {
            logger.LogDebug("Getting all updated providers started");
            offsetFilter ??= new OffsetFilter();

            Expression<Func<Workshop, bool>> filterExpression = updatedAfter == default
                ? workshop => !workshop.IsDeleted
                : workshop => workshop.UpdatedAt > updatedAfter || workshop.DeleteDate > updatedAfter;

            var workshops = await workshopRepository.Get(
                    offsetFilter.From,
                    offsetFilter.Size,
                    WorkshopIncludes,
                    filterExpression)
                .ToListAsync()
                .ConfigureAwait(false);

            var workshopsDto = workshops.Select(MapToInfoWorkshopDto).ToList();

            await FillRatingsForType(workshopsDto).ConfigureAwait(false);
            await FillTakenSeats(workshopsDto).ConfigureAwait(false);

            var count = await workshopRepository.Count(filterExpression).ConfigureAwait(false);

            return new SearchResult<WorkshopInfoBaseDto>
            {
                TotalAmount = count,
                Entities = workshopsDto,
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while processing workshops");
            throw;
        }
    }

    private ProviderInfoBaseDto MapToInfoProviderDto(Provider provider)
    {
        return provider.IsDeleted
            ? mapper.Map<ProviderInfoBaseDto>(provider)
            : mapper.Map<ProviderInfoDto>(provider);
    }

    private WorkshopInfoBaseDto MapToInfoWorkshopDto(Workshop workshop)
    {
        return workshop.IsDeleted
            ? mapper.Map<WorkshopInfoBaseDto>(workshop)
            : mapper.Map<WorkshopInfoDto>(workshop);
    }

    private async Task FillRatingsForType<T>(List<T> dtos)
        where T : class, IExternalInfo<Guid>
    {
        var ids = dtos.Select(w => w.Id).ToList();

        var averageRatings =
            (await averageRatingService.GetByEntityIdsAsync(ids).ConfigureAwait(false)).ToList();

        foreach (var dto in dtos.OfType<IExternalRatingInfo>())
        {
            var averageRatingsForProvider = averageRatings?.SingleOrDefault(r => r.EntityId == dto.Id);
            dto.Rating = averageRatingsForProvider?.Rate ?? 0;
            dto.NumberOfRatings = averageRatingsForProvider?.RateQuantity ?? 0;
        }
    }

    private async Task FillTakenSeats(List<WorkshopInfoBaseDto> dtos)
    {
        var fullDtos = dtos.OfType<WorkshopInfoDto>().ToList();
        var ids = fullDtos.Select(w => w.Id).ToList();
        var takenSeats = await applicationRepository.CountTakenSeatsForWorkshops(ids).ConfigureAwait(false);
        foreach (var dto in fullDtos)
        {
            var takenSeat = takenSeats?.SingleOrDefault(w => w.WorkshopId == dto.Id)?.TakenSeats;
            dto.TakenSeats = takenSeat ?? 0;
        }
    }
}