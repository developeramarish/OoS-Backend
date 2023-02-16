﻿using System;
using System.Threading.Tasks;

using OutOfSchool.Services.Models;

namespace OutOfSchool.Services.Repository;

public interface IInstitutionAdminRepositoryBase<TId, TEntity> : IEntityRepository<(string, TId), TEntity>
    where TEntity : InstitutionAdminBase, IKeyedEntity<(string, TId)>, new()
{
    Task<TEntity> GetByIdAsync(string userId);
}