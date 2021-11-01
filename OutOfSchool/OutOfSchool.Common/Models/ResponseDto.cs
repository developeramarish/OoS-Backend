﻿using System.Collections.Generic;

namespace OutOfSchool.Common
{
    public class ResponseDto
    {
        public object Result { get; set; }

        public IEnumerable<string> ErrorMessages { get; set; }

        public string Message { get; set; }

        public bool IsSuccess { get; set; }
    }
}
