﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OutOfSchool.Services.Enums;
using OutOfSchool.Services.Extensions;
using OutOfSchool.Services.Models;
using OutOfSchool.Services.Repository.Base.Api;

namespace OutOfSchool.Services.Repository.Base;

public class EntityRepositorySoftDeleted<TKey, TEntity> : EntityRepositoryBase<TKey, TEntity>, IEntityRepositorySoftDeleted<TKey, TEntity>
    where TEntity : class, IKeyedEntity<TKey>, ISoftDeleted, new()
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityRepositorySoftDeleted{TKey, TEntity}"/> class.
    /// </summary>
    /// <param name="dbContext">OutOfSchoolDbContext.</param>
    public EntityRepositorySoftDeleted(OutOfSchoolDbContext dbContext)
        : base(dbContext)
    {
    }

    /// <inheritdoc/>
    public override async Task<IEnumerable<TEntity>> GetAll()
    {
        return await dbSet.Where(x => !x.IsDeleted).ToListAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<IEnumerable<TEntity>> GetAllWithDetails(string includeProperties = "")
    {
        IQueryable<TEntity> query = dbSet.Where(x => !x.IsDeleted);
        query = query.IncludeProperties(includeProperties);
        return await query.ToListAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<IEnumerable<TEntity>> GetByFilter(
        Expression<Func<TEntity, bool>> whereExpression,
        string includeProperties = "")
    {
        whereExpression = this.GetWhereExpression(whereExpression);
        return await base.GetByFilter(whereExpression, includeProperties).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override IQueryable<TEntity> GetByFilterNoTracking(
        Expression<Func<TEntity, bool>> whereExpression,
        string includeProperties = "")
    {
        whereExpression = this.GetWhereExpression(whereExpression);
        return base.GetByFilterNoTracking(whereExpression, includeProperties);
    }

    /// <inheritdoc/>
    public override Task<TEntity> GetById(TKey id) => dbSet.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id.Equals(id));

    /// <inheritdoc/>
    public override Task<TEntity> GetByIdWithDetails(TKey id, string includeProperties = "")
        => dbSet.Where(x => !x.IsDeleted && x.Id.Equals(id)).IncludeProperties(includeProperties).FirstOrDefaultAsync();

    /// <inheritdoc/>
    public override Task<int> Count(Expression<Func<TEntity, bool>> whereExpression = null)
    {
        return base.Count(this.GetWhereExpression(whereExpression));
    }

    public override Task<bool> Any(Expression<Func<TEntity, bool>> whereExpression = null)
    {
        return base.Any(this.GetWhereExpression(whereExpression));
    }

    public override IQueryable<TEntity> Get(
        int skip = 0,
        int take = 0,
        string includeProperties = "",
        Expression<Func<TEntity, bool>> whereExpression = null,
        Dictionary<Expression<Func<TEntity, object>>, SortDirection> orderBy = null,
        bool asNoTracking = false)
    {
        return base.Get(skip, take, includeProperties, this.GetWhereExpression(whereExpression), orderBy, asNoTracking);
    }

    private Expression<Func<TEntity, bool>> GetWhereExpression(Expression<Func<TEntity, bool>> whereExpression)
    {
        if (whereExpression != null)
        {
            Expression<Func<TEntity, bool>> right = x => !x.IsDeleted;
            whereExpression = Expression.Lambda<Func<TEntity, bool>>(
                Expression.AndAlso(
                    whereExpression.Body,
                    new ExpressionParameterReplacer(right.Parameters, whereExpression.Parameters).Visit(right.Body)
                    ),
                whereExpression.Parameters);
        }
        else
        {
            whereExpression = x => !x.IsDeleted;
        }

        return whereExpression;
    }

    private sealed class ExpressionParameterReplacer : ExpressionVisitor
    {
        private IDictionary<ParameterExpression, ParameterExpression> ParameterReplacements { get; set; }

        public ExpressionParameterReplacer(
            IList<ParameterExpression> fromParameters,
            IList<ParameterExpression> toParameters)
        {
            ParameterReplacements = fromParameters.Zip(toParameters, (k, v) => new { k, v })
                .ToDictionary(x => x.k, x => x.v);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            ParameterExpression replacement;

            if (ParameterReplacements.TryGetValue(node, out replacement))
            { node = replacement; }

            return base.VisitParameter(node);
        }
    }
}
