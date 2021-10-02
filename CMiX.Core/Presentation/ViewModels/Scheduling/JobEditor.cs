// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class JobEditor : ObservableObject, IModalDialogViewModel
    {
        public JobEditor(IProject project)
        {
            Playlists = project.Playlists;
            ToRunType = new ToRunType();
            ApplyCommand = new RelayCommand<CompositionScheduler>(AddJobToScheduler);
            //AddJobToSchedulerCommand = new RelayCommand<CompositionScheduler>(AddJobToScheduler);
            //AddTaskCommand = new RelayCommand(AddTask);
        }

        public ICommand ApplyCommand { get; set; }
        public ObservableCollection<Playlist> Playlists { get; set; }
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
