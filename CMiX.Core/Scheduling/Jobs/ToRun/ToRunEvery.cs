using System;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentScheduler;

namespace CMiX.Core.ViewModels.Scheduling
{
    public class ToRunEvery : ObservableObject, IToRun//, IScheduleInterface<Schedule>
    {
        public ToRunEvery()
        {
            Name = "ToRunEvery";
            SetScheduler = new Action<Schedule>((s) => { SetSchedule(s); });
            UnitType = new UnitType();
            UnitInterval = new UnitInterval(5);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private UnitInterval _unitinterval;
        public UnitInterval UnitInterval
        {
            get => _unitinterval;
            set => SetProperty(ref _unitinterval, value);
        }

        private UnitType _unittype;
        public UnitType UnitType
        {
            get => _unittype;
            set => SetProperty(ref _unittype, value);
        }


        public Action<Schedule> SetScheduler { get; set; }

        //public void SetToRunEvery(Schedule schedule)
        //{
        //    var unittype = (IScheduleInterface<TimeUnit>)UnitType.SelectedUnitType;
        //    unittype.SetScheduler.Invoke(schedule.ToRunEvery(UnitType.UnitInterval.Interval));
        //}

        private void SetSchedule(Schedule schedule)
        {
            UnitType.SetScheduler.Invoke(schedule.ToRunEvery(UnitType.UnitInterval.Interval));
            //schedule.ToRunEvery(UnitType.UnitInterval.Interval);
        }
    }
}
