﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OutOfSchool.Services.Models;

namespace OutOfSchool.Services.Repository
{
    public class ProviderAdminRepository : EntityRepository<ProviderAdmin>, IProviderAdminRepository
    {
        private readonly OutOfSchoolDbContext db;

        public ProviderAdminRepository(OutOfSchoolDbContext dbContext)
         : base(dbContext)
        {
            db = dbContext;
        }

        public async Task<bool> IsExistProviderAdminWithUserIdAsync(Guid providerId, string userId)
        {
            var providerAdmin = await db.ProviderAdmins //single query to db
                .Where(pa => pa.ProviderId == providerId)
                .SingleOrDefaultAsync(pa => pa.UserId == userId);

            return providerAdmin != null;
        }

        public async Task<bool> IsExistProviderWithUserIdAsync(Guid providerId, string userId)
        {
            var provider = await db.Providers
                .Where(p => p.Id == providerId)
                .SingleOrDefaultAsync(p => p.UserId == userId);

            return provider != null;
        }

        public async Task<int> GetNumberProviderAdminsAsync(Guid providerId)
        {
            var number = await db.ProviderAdmins
                .CountAsync(pa => pa.ProviderId == providerId);

            return number;
        }
    }
}
