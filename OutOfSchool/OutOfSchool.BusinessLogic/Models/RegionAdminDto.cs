﻿using System.ComponentModel.DataAnnotations;
using OutOfSchool.BusinessLogic.Enums;

namespace OutOfSchool.BusinessLogic.Models;

public class RegionAdminDto : BaseUserDto
{
    [EnumDataType(typeof(AccountStatus), ErrorMessage = Constants.EnumErrorMessage)]
    public AccountStatus AccountStatus { get; set; }

    public Guid InstitutionId { get; set; }

    public string InstitutionTitle { get; set; }

    public long CATOTTGId { get; set; }

    public string CATOTTGCategory { get; set; }

    public string CATOTTGName { get; set; }
}