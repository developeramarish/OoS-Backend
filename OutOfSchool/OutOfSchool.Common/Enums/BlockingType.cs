﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OutOfSchool.Common.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum BlockingType
{
    None,
    Manually,
    Automatically,
}
