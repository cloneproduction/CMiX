using System;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentScheduler;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class HourUnit : ObservableObject, IUnit
    {
        public HourUnit()
        {
            Name = "Hours";
            SetScheduler = new Action<TimeUnit>((s) => { SetUnit(s); });
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public Action<TimeUnit> SetScheduler { get; set; }

        public void SetUnit(TimeUnit timeunit)
        {
            timeunit.Hours();
        }
    }
}
