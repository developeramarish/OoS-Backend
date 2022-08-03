using OutOfSchool.Services.Enums;

namespace OutOfSchool.WebApi.Common.StatusPermissions;

public class StatusPermissions<T>
    where T : struct
{
    public List<StatusChangePermission<T>> AllowedStatuses = new List<StatusChangePermission<T>>();

    public void AllowStatusChange(string role, T fromStatus = default, T toStatus = default)
    {
        AllowedStatuses.Add(new StatusChangePermission<T>
        {
            Role = role,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            Allowed = true,
        });
    }

    public void DenyStatusChange(string role, T fromStatus = default, T toStatus = default)
    {
        AllowedStatuses.Add(new StatusChangePermission<T>
        {
            Role = role,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            Allowed = false,
        });
    }

    public bool CanChangeStatus(string role, T from, T to)
    {
        var denyResult = AllowedStatuses.Any(p =>
            (p.Role == role || p.Role == "all")
            && (Convert.ToInt32(p.FromStatus) == Convert.ToInt32(from) || Convert.ToInt32(p.FromStatus) == 0)
            && (Convert.ToInt32(p.ToStatus) == Convert.ToInt32(to) || Convert.ToInt32(p.ToStatus) == 0)
            && p.Allowed == false
        );

        if (denyResult)
            return false;

        var allowResult = AllowedStatuses.Any(p =>
            (p.Role == role || p.Role == "all")
            && (Convert.ToInt32(p.FromStatus) == Convert.ToInt32(from) || Convert.ToInt32(p.FromStatus) == 0)
            && (Convert.ToInt32(p.ToStatus) == Convert.ToInt32(to) || Convert.ToInt32(p.ToStatus) == 0)
            && p.Allowed
        );

        return allowResult;
    }
}