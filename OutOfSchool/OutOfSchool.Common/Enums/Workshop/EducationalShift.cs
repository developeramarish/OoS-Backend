using System.Text.Json.Serialization;

namespace OutOfSchool.Common.Enums.Workshop;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EducationalShift
{
    First,
    Second,
}