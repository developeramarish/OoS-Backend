﻿using OutOfSchool.WebApi.Models.Images;

namespace OutOfSchool.WebApi.Models.Teachers
{
    public class TeacherCreationResponse
    {
        public TeacherDTO Teacher { get; set; }

        public SingleImageUploadingResponse UploadingAvatarImageResult { get; set; }
    }
}
