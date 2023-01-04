﻿namespace OutOfSchool.Services.Enums;

public enum StatisticReportTypes
{
    WorkshopsYear,
    WorkshopsDaily,
}

public static class StatisticReportTypesExtensions
{
    public static string GetReportTitle(this StatisticReportTypes value)
    {
        return value switch
        {
            StatisticReportTypes.WorkshopsYear => "Річний звіт по гуртках: {0:dd-MM-yyyy}",
            StatisticReportTypes.WorkshopsDaily => "Поточний звіт по гуртках: {0:dd-MM-yyyy}",
            _ => string.Empty
        };
    }
}
