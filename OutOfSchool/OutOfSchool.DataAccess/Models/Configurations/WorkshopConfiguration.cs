﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutOfSchool.Services.Models.Configurations.Base;

namespace OutOfSchool.Services.Models.Configurations;

internal class WorkshopConfiguration : BusinessEntityWithContactsConfiguration<Workshop>
{
    public override void Configure(EntityTypeBuilder<Workshop> builder)
    {
        base.Configure(builder);
        builder.HasMany(x => x.Employees)
            .WithMany(x => x.ManagedWorkshops);

        builder.HasMany(x => x.WorkshopDescriptionItems)
             .WithOne(x => x.Workshop)
             .HasForeignKey(x => x.WorkshopId)
             .OnDelete(DeleteBehavior.Cascade);

        // One-to-One relationship (with shadow property in Workshop)
        builder.HasOne(x => x.DefaultTeacher)
            .WithOne()
            .HasForeignKey<Workshop>(x => x.DefaultTeacherId) // Shadow property for One-to-One relationship
            .IsRequired(false) // Optional relationship
            .OnDelete(DeleteBehavior.Restrict);

        // One-to-Many relationship (with shadow property in Workshop)
        builder.HasMany(x => x.Teachers)
            .WithOne()
            .HasForeignKey(x => x.WorkshopId) // Shadow property for One-to-Many relationship
            .IsRequired(false) // Optional relationship
            .OnDelete(DeleteBehavior.Cascade);

        // Self-referencing one-to-many relationship configuration
        builder.HasOne(x => x.ParentWorkshop)
            .WithMany(x => x.IncludedStudyGroups)
            .HasForeignKey(x => x.ParentWorkshopId)
            .IsRequired(false) // Optional relationship
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Tags)
            .WithMany(x => x.Workshops);
    }
}