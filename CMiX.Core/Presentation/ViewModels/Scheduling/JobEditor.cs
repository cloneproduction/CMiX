using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Models.Scheduling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class JobEditor : ObservableObject, IControl
    {
        public JobEditor(JobEditorModel jobEditorModel, ObservableCollection<Playlist> playlists, JobScheduler jobScheduler)
        {
            this.ID = jobEditorModel.ID;

            JobScheduler = jobScheduler;
            Playlists = playlists;
            ToRunType = new ToRunType();

            CreateJobCommand = new RelayCommand(CreateJob);
        }


        public Guid ID { get; set; }
        public ICommand CreateJobCommand { get; set; }
        public JobScheduler JobScheduler { get; set; }
        public ObservableCollection<Playlist> Playlists { get; set; }


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


        private void CreateJob()
        {
            if (SelectedPlaylist != null)
            {
                var job = new JobNextComposition(JobName, SelectedPlaylist, (s) => ToRunType.SetRunType(s.WithName(JobName)));
                JobScheduler.AddJob(job);
            }
        }


        public void SetViewModel(IModel model)
        {
            JobEditorModel jobEditorModel = model as JobEditorModel;
            this.ID = jobEditorModel.ID;
        }

        public IModel GetModel()
        {
            JobEditorModel jobEditorModel = new JobEditorModel();
            return jobEditorModel;
        }
    }
}
