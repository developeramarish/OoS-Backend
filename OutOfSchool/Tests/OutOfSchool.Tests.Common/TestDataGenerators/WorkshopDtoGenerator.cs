﻿using Bogus;
using OutOfSchool.Common.Enums;
using OutOfSchool.BusinessLogic.Models.Workshops;
using System.Collections.Generic;
using OutOfSchool.BusinessLogic.Models.Tag;

namespace OutOfSchool.Tests.Common.TestDataGenerators;

public static class WorkshopDtoGenerator
{
    private static readonly Faker<WorkshopDto> Faker = new Faker<WorkshopDto>()
    .RuleFor(x => x.TakenSeats, f => f.Random.UInt(0, 5))
    .RuleFor(x => x.Rating, f => f.Random.Float(0, 5))
    .RuleFor(x => x.NumberOfRatings, f => f.Random.Int(0))
    .RuleFor(x => x.Status, f => f.PickRandom<WorkshopStatus>())
    .RuleFor(x => x.IsBlocked, f => f.Random.Bool())
    .RuleFor(x => x.ProviderOwnership, f => f.PickRandom<OwnershipType>())
    .RuleFor(x => x.ProviderStatus, f => f.PickRandom<ProviderStatus>())
    .RuleFor(x => x.Tags, f => f.Make(5, () => new TagDto
    {
        Id = f.Random.Long(1, 100),
        Name = f.Commerce.ProductName()
    }))
    .CustomInstantiator(f =>
    {
        var dto = new WorkshopDto();
        WorkshopBaseDtoGenerator.Populate(dto);
        return dto;
    });

    public static WorkshopDto Generate() => Faker.Generate();

    public static List<WorkshopDto> Generate(int count) => Faker.Generate(count);

    public static void Populate(WorkshopDto dto) => Faker.Populate(dto);
}