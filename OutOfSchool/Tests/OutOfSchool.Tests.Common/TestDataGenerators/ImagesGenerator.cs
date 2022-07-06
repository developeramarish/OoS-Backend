using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Microsoft.AspNetCore.Http;
using OutOfSchool.Services.Models.Images;

namespace OutOfSchool.Tests.Common.TestDataGenerators;

public static class ImagesGenerator
{
    public static List<string> CreateRandomImageIds(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentException($"{nameof(count)} cannot be less or equal 0.");
        }

        return Enumerable.Range(0, count)
            .Select(r => Guid.NewGuid().ToString())
            .ToList();
    }

    public static List<DateTime> CreateRandomImageDateTimes(int count, DateTime minDateTime, DateTime maxDateTime)
    {
        if (count <= 0)
        {
            throw new ArgumentException($"{nameof(count)} cannot be less or equal 0.");
        }

        if (maxDateTime < minDateTime)
        {
            throw new InvalidOperationException(
                $"{nameof(maxDateTime)} must be greater than {nameof(minDateTime)} at least for 1 millisecond.");
        }

        var random = new Random();
        var offset = maxDateTime - minDateTime;

        return Enumerable.Range(0, count)
            .Select(r => minDateTime + TimeSpan.FromMinutes(random.Next(0, (int)offset.TotalMinutes)))
            .ToList();
    }

    public static List<Google.Apis.Storage.v1.Data.Object> CreateGcpEmptyObjects(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentException($"{nameof(count)} cannot be less or equal 0.");
        }

        var objects = new List<Google.Apis.Storage.v1.Data.Object>();
        for (var i = 0; i < count; i++)
        {
            objects.Add(new Google.Apis.Storage.v1.Data.Object());
        }

        return objects;
    }
}