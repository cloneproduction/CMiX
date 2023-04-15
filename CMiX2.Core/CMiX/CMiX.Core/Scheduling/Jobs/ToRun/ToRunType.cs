using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentScheduler;

namespace CMiX.Core.ViewModels.Scheduling
{
    public class ToRunType : ObservableObject
    {
        public ToRunType()
        {
            ToRunTypes = new ObservableCollection<IToRun>();
            ToRunTypes.Add(new ToRunNow());
            ToRunTypes.Add(new ToRunEvery());
            ToRunTypes.Add(new ToRunNowAndEvery());

            SelectedToRunType = new ToRunNow();
        }

        public ObservableCollection<IToRun> ToRunTypes { get; set; }

        private IToRun _selectedToRunType;
        public IToRun SelectedToRunType
        {
            get => _selectedToRunType;
            set
            {
                if (SelectedToRunType != null)
                    SetScheduler = SelectedToRunType.SetScheduler;
                SetProperty(ref _selectedToRunType, value);
            }
        }

        public Action<Schedule> SetScheduler { get; set; }

        public void SetRunType(Schedule schedule)
        {
            //var selected = SelectedToRunType as IScheduleInterface<Schedule>;
            SelectedToRunType.SetScheduler.Invoke(schedule);
        }
    }
}
