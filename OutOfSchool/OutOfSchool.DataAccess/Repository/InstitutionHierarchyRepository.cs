﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OutOfSchool.Services.Models.SubordinationStructure;
using OutOfSchool.Services.Repository.Api;
using OutOfSchool.Services.Repository.Base;

namespace OutOfSchool.Services.Repository;

public class InstitutionHierarchyRepository : EntityRepositorySoftDeleted<Guid, InstitutionHierarchy>, IInstitutionHierarchyRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InstitutionHierarchyRepository"/> class.
    /// </summary>
    /// <param name="dbContext">OutOfSchoolDbContext.</param>
    public InstitutionHierarchyRepository(OutOfSchoolDbContext dbContext)
        : base(dbContext)
    {
    }

    /// <summary>
    /// Add new element.
    /// </summary>
    /// <param name="entity">Entity to create.</param>
    /// <param name="directionsIds">IDs List of directions.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<InstitutionHierarchy> Create(InstitutionHierarchy entity, List<long> directionsIds)
    {
        entity.Directions = dbContext.Directions.Where(w => directionsIds.Contains(w.Id)).ToList();

        await dbSet.AddAsync(entity);
        await dbContext.SaveChangesAsync();

        return await Task.FromResult(entity);
    }

    /// <summary>
    /// Update information about element.
    /// </summary>
    /// <param name="entity">Entity to update.</param>
    /// <param name="directionsIds">Long list of directions.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<InstitutionHierarchy> Update(InstitutionHierarchy entity, List<long> directionsIds)
    {
        var newEntity = await GetById(entity.Id);

        dbContext.Entry(newEntity).CurrentValues.SetValues(entity);

        newEntity.Directions.RemoveAll(x => !directionsIds.Contains(x.Id));
        var exceptDirectionsIds = directionsIds.Where(p => newEntity.Directions.TrueForAll(x => x.Id != p));
        newEntity.Directions.AddRange(dbContext.Directions.Where(w => exceptDirectionsIds.Contains(w.Id)).ToList());

        dbContext.Entry(newEntity).State = EntityState.Modified;

        await this.dbContext.SaveChangesAsync();
        return newEntity;
    }
}
