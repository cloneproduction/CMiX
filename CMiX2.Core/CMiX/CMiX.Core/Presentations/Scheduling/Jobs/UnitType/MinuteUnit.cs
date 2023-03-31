using System;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentScheduler;

namespace CMiX.Core.Presentations.ViewModels.Scheduling
{
    public class MinuteUnit : ObservableObject, IUnit
    {
        public MinuteUnit()
        {
            Name = "Minutes";
            SetScheduler = new Action<TimeUnit>((s) => { SetUnit(s); });
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public Action<TimeUnit> SetScheduler { get; set; }

        public void SetUnit(TimeUnit timeUnit)
        {
            timeUnit.Minutes();
        }
    }
}
