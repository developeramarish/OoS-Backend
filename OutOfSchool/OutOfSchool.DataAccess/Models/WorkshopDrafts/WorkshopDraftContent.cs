﻿using System;
using System.Collections.Generic;
using OutOfSchool.Common.Enums;

namespace OutOfSchool.Services.Models.WorkshopDrafts;

/// <summary>
///     Will be stored as nested objects in the JSON format of the workshop draft.
///     This entity is specific to the draft and can be hard-deleted if the draft is removed.
/// </summary>
public class WorkshopDraftContent
{
    public int MinAge { get; set; }

    public int MaxAge { get; set; }

    public uint EducationalShiftId { get; set; }

    public bool ShortStay { get; set; }

    public uint LanguageOfEducationId { get; set; }

    public List<DateTimeRangeDraft> DateTimeRanges { get; set; } = new ();

    public List<WorkshopDescriptionItemDraft> WorkshopDescriptionItems { get; set; } = new();

    public bool IsSelfFinanced { get; set; }

    public uint TypeOfAgeCompositionId { get; set; }

    public bool CompetitiveSelection { get; set; }

    public DateOnly ActiveFrom { get; set; }

    public DateOnly ActiveTo { get; set; }

    public List<Guid> EducationalDisciplinesId { get; set; } = new();

    public List<long> TagIds { get; set; } = new();

    public string Title { get; set; }

    public string ProviderTitle { get; set; }

    public string ProviderTitleEn { get; set; }

    public List<long> DirectionIds { get; set; } = new();

    public uint CategoryId { get; set; }

    public uint GroupeTypeId { get; set; }

    public uint CoverageId { get; set; }

    public Guid? MemberOfWorkshopId { get; set; }

    public bool IsPaid { get; set; }

    public bool IsSpecial { get; set; }

    public bool IsInclusive { get; set; }

    public IEnumerable<string> Keywords { get; set; }

    public AddressDraft Address { get; set; }

    public bool WithDisabilityOptions { get; set; }

    public string  DisabilityOptionsDesc { get; set; }

    public OwnershipType OwnershipType { get; set; }

    public uint AvailableSeats { get; set; }

    public List<Guid> IncludedStudyGroupsIds { get; set; } = new();

    public uint SpecialNeedsId { get; set; }

    public string AdditionalDescription { get; set; }

    public string ShortTitle { get; set; }

    public string CompetitiveSelectionDescription { get; set; }

    public FormOfLearning FormOfLearning { get; set; }

    public WorkshopStatus WorkshopStatus { get; set; }

    public ProviderLicenseStatus ProviderLicenseStatus { get; set; }

    public decimal Price { get; set; }

    public PayRateType PayRate { get; set; }

    public bool AreThereBenefits { get; set; }

    public string PreferentialTermsOfParticipation { get; set; }

    public Guid? InstitutionHierarchyId { get; set; }

    public Guid? InstitutionId { get; set; }

    public string Phone { get; set; } 

    public string Email { get; set; } 

    public string Website { get; set; } 

    public string Facebook { get; set; } 

    public string Instagram { get; set; } 
}