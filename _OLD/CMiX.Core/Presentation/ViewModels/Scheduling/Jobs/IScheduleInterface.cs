using System;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public interface IScheduleInterface<T>
    {
        Action<T> SetScheduler { get; set; }
    }
}
