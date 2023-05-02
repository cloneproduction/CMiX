using System;
using CMiX.Core.Scheduling;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentScheduler;

namespace CMiX.Core.ViewModels.Scheduling
{
    public class ToRunNow : ObservableObject, IToRun//, IScheduleInterface<Schedule>
    {
        public ToRunNow()
        {
            Name = "ToRunNow";
            SetScheduler = new Action<Schedule>((s) => { SetSchedule(s); });
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public Action<Schedule> SetScheduler { get; set; }

        //public void SetToRunNow(Schedule schedule)
        //{
        //    schedule.ToRunNow();
        //}

        public void SetSchedule(Schedule schedule)
        {
            schedule.ToRunNow();
        }

        public IModel GetModel()
        {
            ToRunNowModel toRunNowModel = new ToRunNowModel();
            toRunNowModel.Name = this.Name;
            return toRunNowModel;
        }
    }
}
