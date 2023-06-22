using System;

namespace Shared.Helpers;

public static class DateTimeHelper
{
    public static DateTime SetUtcKind(this DateTime dt)
    {        
        return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
    }
}