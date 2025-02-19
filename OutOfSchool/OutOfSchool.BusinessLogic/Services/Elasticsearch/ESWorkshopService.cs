﻿using AutoMapper;
using Elastic.Clients.Elasticsearch;
using OutOfSchool.BusinessLogic.Models;
using OutOfSchool.BusinessLogic.Services.AverageRatings;

namespace OutOfSchool.BusinessLogic.Services;

/// <inheritdoc/>
public class ESWorkshopService : IElasticsearchService<WorkshopES, WorkshopFilterES>
{
    private readonly IWorkshopService workshopService;
    private readonly IAverageRatingService averageRatingService;
    private readonly IElasticsearchProvider<WorkshopES, WorkshopFilterES> esProvider;
    private readonly IElasticsearchHealthService elasticHealthService;
    private readonly ILogger<ESWorkshopService> logger;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ESWorkshopService"/> class.
    /// </summary>
    /// <param name="workshopService">Service that provides access to Workshops in the database.</param>
    /// <param name="esProvider">Provider to the Elasticsearch workshops index.</param>
    /// <param name="elasticHealthService">Background worker pings the Elasticsearch.</param>
    /// <param name="logger">Logger.</param>
    /// <param name="averageRatingService">Service that provides access to average ratings in the database.</param>
    /// <param name="mapper">AutoMapper interface.</param>
    public ESWorkshopService(
        IWorkshopService workshopService,
        IElasticsearchProvider<WorkshopES, WorkshopFilterES> esProvider,
        IElasticsearchHealthService elasticHealthService,
        ILogger<ESWorkshopService> logger,
        IAverageRatingService averageRatingService,
        IMapper mapper)
    {
        this.workshopService = workshopService;
        this.esProvider = esProvider;
        this.elasticHealthService = elasticHealthService;
        this.logger = logger;
        this.averageRatingService = averageRatingService;
        this.mapper = mapper;
    }

    /// <inheritdoc/>
    public bool IsElasticAlive => elasticHealthService.IsHealthy;

    /// <inheritdoc/>
    public async Task<bool> Index(WorkshopES entity)
    {
        NullCheck(entity);

        var resp = await esProvider.IndexEntityAsync(entity).ConfigureAwait(false);

        if (resp == Result.Updated || resp == Result.Created)
        {
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public async Task<bool> Update(WorkshopES entity)
    {
        NullCheck(entity);

        var rating = await averageRatingService.GetByEntityIdAsync(entity.Id).ConfigureAwait(false);

        entity.Rating = rating?.Rate ?? default;

        var resp = await esProvider.UpdateEntityAsync(entity).ConfigureAwait(false);

        if (resp == Result.Updated || resp == Result.Created)
        {
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public async Task<bool> Delete(Guid id)
    {
        var resp = await esProvider.DeleteEntityAsync(new WorkshopES() { Id = id }).ConfigureAwait(false);

        if (resp == Result.Deleted)
        {
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public async Task<bool> ReIndex()
    {
        try
        {
            var filter = new OffsetFilter() { From = 0, Size = 500 };
            var source = new List<WorkshopES>();

            var data = await workshopService.GetAll(filter).ConfigureAwait(false);
            while (data.Entities.Count > 0)
            {
                foreach (var entity in data.Entities)
                {
                    var rating = await averageRatingService.GetByEntityIdAsync(entity.Id).ConfigureAwait(false);
                    entity.Rating = rating?.Rate ?? default;
                    source.Add(mapper.Map<WorkshopES>(entity));
                }

                filter.From += filter.Size;
                data = await workshopService.GetAll(filter).ConfigureAwait(false);
            }

            var resp = await esProvider.ReIndexAll(source).ConfigureAwait(false);

            if (resp == Result.Updated)
            {
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }

        return false;
    }

    /// <inheritdoc/>
    public async Task<SearchResultES<WorkshopES>> Search(WorkshopFilterES filter)
    {
        try
        {
            var res = await esProvider.Search(filter).ConfigureAwait(false);

            return res;
        }
        catch
        {
            return new SearchResultES<WorkshopES>();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> PartialUpdate(Guid id, IPartial<WorkshopES> partialWorkshop)
    {
        var responce = await esProvider.PartialUpdateEntityAsync(id, partialWorkshop).ConfigureAwait(false);

        if (responce == Result.Updated)
        {
            return true;
        }

        return false;
    }

    private void NullCheck(WorkshopES entity)
    {
        if (entity is null)
        {
            logger.LogError("WorkshopES entity {Entity} is null or uninitialized.", entity);
            throw new ArgumentNullException($"{entity} is not set to an instance.");
        }
    }
}