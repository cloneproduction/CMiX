using System;

namespace CMiX.Core.ViewModels.Scheduling
{
    public interface IScheduleInterface<T>
    {
        Action<T> SetScheduler { get; set; }
    }
}
