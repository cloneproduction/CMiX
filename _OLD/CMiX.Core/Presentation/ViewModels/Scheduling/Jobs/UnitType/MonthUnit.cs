using System;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentScheduler;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class MonthUnit : ObservableObject, IUnit
    {
        public MonthUnit()
        {
            Name = "Months";
            SetScheduler = new Action<TimeUnit>((s) => { SetUnit(s); });
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private int _hours;
        public int Hours
        {
            get => _hours;
            set => SetProperty(ref _hours, value);
        }

        private int _minutes;
        public int Minutes
        {
            get => _minutes;
            set => SetProperty(ref _minutes, value);
        }

        public At At { get; set; }

        public Action<TimeUnit> SetScheduler { get; set; }

        public void SetUnit(TimeUnit timeunit)
        {
            timeunit.Months();
        }
    }
}
