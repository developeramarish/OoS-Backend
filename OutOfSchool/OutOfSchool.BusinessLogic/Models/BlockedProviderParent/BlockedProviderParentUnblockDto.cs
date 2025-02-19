﻿using System.ComponentModel.DataAnnotations;

namespace OutOfSchool.BusinessLogic.Models.BlockedProviderParent;

public class BlockedProviderParentUnblockDto
{
    [Required]
    public Guid ParentId { get; set; }

    [Required]
    public Guid ProviderId { get; set; }
}