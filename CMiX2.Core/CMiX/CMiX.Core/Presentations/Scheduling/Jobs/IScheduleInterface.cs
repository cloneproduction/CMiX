using System;

namespace CMiX.Core.Presentations.ViewModels.Scheduling
{
    public interface IScheduleInterface<T>
    {
        Action<T> SetScheduler { get; set; }
    }
}
