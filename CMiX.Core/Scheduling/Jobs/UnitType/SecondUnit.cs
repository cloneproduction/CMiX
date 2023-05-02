using System;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentScheduler;

namespace CMiX.Core.ViewModels.Scheduling
{
    public class SecondUnit : ObservableObject, IUnit// IScheduleInterface<TimeUnit>
    {
        public SecondUnit()
        {
            Name = "Seconds";
            SetScheduler = new Action<TimeUnit>((s) => { SetUnit(s); });
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public Action<TimeUnit> SetScheduler { get; set; }


        private void SetUnit(TimeUnit timeunit)
        {
            //SetScheduler.Invoke(timeunit);
            timeunit.Seconds();
        }
    }
}
