using System;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentScheduler;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class ToRunNowAndEvery : ObservableObject, IToRun//, IScheduleInterface<Schedule>
    {
        public ToRunNowAndEvery()
        {
            Name = "ToRunNowAndEvery";
            SetScheduler = new Action<Schedule>((s) => { SetSchedule(s); });
            UnitType = new UnitType();
            UnitInterval = new UnitInterval(60);
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

        //private void SetToRunNowAndEvery(Schedule schedule)
        //{
        //    var unittype = UnitType.SelectedUnitType;
        //    unittype.SetScheduler.Invoke(schedule.ToRunNow().AndEvery(UnitType.UnitInterval.Interval));
        //}

        private void SetSchedule(Schedule schedule)
        {
            schedule.ToRunNow().AndEvery(UnitType.UnitInterval.Interval);
        }
    }
}
