﻿using OutOfSchool.BusinessLogic.Common;

namespace OutOfSchool.BusinessLogic.Models.Images;

public class MultipleImageUploadingResponse
{
    public bool AllImagesUploaded { get; set; }

    public string GeneralMessage { get; set; }

    public bool HasResults { get; set; }

    public IDictionary<short, OperationResult> Results { get; set; }
}