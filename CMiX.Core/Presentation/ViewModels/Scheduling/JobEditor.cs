// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class JobEditor : ObservableObject, IModalDialogViewModel
    {
        public JobEditor()
        {
            ToRunType = new ToRunType();
            //AddJobToSchedulerCommand = new RelayCommand<CompositionScheduler>(AddJobToScheduler);
            //AddTaskCommand = new RelayCommand(AddTask);
        }

        public bool? DialogResult { get; set; }

        public void AddJobToScheduler(CompositionScheduler compositionScheduler)
        {
            if (SelectedPlaylist != null)
            {
                Job job = new JobNextComposition(JobName, SelectedPlaylist, (s) => ToRunType.SetRunType(s.WithName(JobName)));
                compositionScheduler.JobScheduler.AddJob(job);
            }
        }


        private Playlist _selectedplaylist;
        public Playlist SelectedPlaylist
        {
            get => _selectedplaylist;
            set => SetProperty(ref _selectedplaylist, value);
        }

        private ToRunType _toruntype;
        public ToRunType ToRunType
        {
            get => _toruntype;
            set => SetProperty(ref _toruntype, value);
        }

        private string _jobName;
        public string JobName
        {
            get => _jobName;
            set => SetProperty(ref _jobName, value);
        }
    }
}
