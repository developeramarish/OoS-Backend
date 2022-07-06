﻿using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using OutOfSchool.Services;
using OutOfSchool.Services.Enums;
using OutOfSchool.Services.Models;
using OutOfSchool.Tests.Common;
using OutOfSchool.Tests.Common.TestDataGenerators;

namespace OutOfSchool.Tests;

public static class UnitTestHelper
{
    public static DbContextOptions<OutOfSchoolDbContext> GetUnitTestDbOptions()
    {
        var options = new DbContextOptionsBuilder<OutOfSchoolDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).EnableSensitiveDataLogging()
            .UseLazyLoadingProxies()
            .Options;

        using (var context = new OutOfSchoolDbContext(options))
        {
            SeedData(context);
        }

        return options;
    }

    public static void SeedData(OutOfSchoolDbContext context)
    {
        var socialGroups = new List<SocialGroup>
        {
            new SocialGroup { Id = 1, Name = "sg1" },
            new SocialGroup { Id = 2, Name = "sg2" },
            new SocialGroup { Id = 3, Name = "sg3" },
        };

        context.SocialGroups.AddRange(socialGroups);

        var parents = new List<Parent>
        {
            new Parent { Id = Guid.NewGuid(), UserId = Guid.NewGuid().ToString() },
            new Parent { Id = Guid.NewGuid(), UserId = Guid.NewGuid().ToString() },
            new Parent { Id = Guid.NewGuid(), UserId = Guid.NewGuid().ToString() },
        };

        context.Parents.AddRange(parents);

        var children = ChildGenerator.Generate(3);
        children.ForEach(c => c.WithSocial(socialGroups.RandomItem())
            .WithParent(parents.RandomItem()));

        context.Children.AddRange(children);

        context.Directions.Add(new Direction { Id = 1, Title = "c1" });
        context.Directions.Add(new Direction { Id = 2, Title = "c2" });
        context.Directions.Add(new Direction { Id = 3, Title = "c3" });

        context.Departments.Add(new Department { Id = 1, Title = "sc1", DirectionId = 1 });
        context.Departments.Add(new Department { Id = 2, Title = "sc2", DirectionId = 3 });

        context.Classes.Add(new Class { Id = 1, Title = "ssc1", DepartmentId = 1 });
        context.Classes.Add(new Class { Id = 2, Title = "ssc2", DepartmentId = 1 });
        context.Classes.Add(new Class { Id = 3, Title = "ssc3", DepartmentId = 2 });

        var workshops = new List<Workshop>
        {
            new Workshop { Id = Guid.NewGuid(), Title = "w1", DirectionId = 1 },
            new Workshop { Id = Guid.NewGuid(), Title = "w2", DirectionId = 1 },
            new Workshop { Id = Guid.NewGuid(), Title = "w3", DirectionId = 3 },
        };

        context.Workshops.AddRange(workshops);

        context.Applications.Add(new Application() { Id = Guid.NewGuid(), ChildId = children[0].Id, Status = ApplicationStatus.Pending, WorkshopId = workshops[0].Id, ParentId = parents[0].Id, CreationTime = new DateTime(2021, 7, 9) });
        context.Applications.Add(new Application() { Id = Guid.NewGuid(), ChildId = children[1].Id, Status = ApplicationStatus.Pending, WorkshopId = workshops[0].Id, ParentId = parents[0].Id, CreationTime = new DateTime(2021, 7, 9) });
        context.Applications.Add(new Application() { Id = Guid.NewGuid(), ChildId = children[2].Id, Status = ApplicationStatus.Pending, WorkshopId = workshops[2].Id, ParentId = parents[0].Id, CreationTime = new DateTime(2021, 7, 9) });

        context.SaveChanges();
    }
}