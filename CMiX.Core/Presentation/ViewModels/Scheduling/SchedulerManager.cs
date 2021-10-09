// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Models.Scheduling;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.Views.Scheduling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class SchedulerManager : ObservableRecipient, IRecipient<ISchedulerMessage>
    {
        public SchedulerManager(IProject project)
        {
            Project = project;
            CompositionSchedulers = new ObservableCollection<CompositionScheduler>();
            PlaylistEditor = new PlaylistEditor(project);
            JobEditor = new JobEditor(project);

            Messenger.Register(this, MessageType.In);

            IsActive = true;
            AddJobCommand = new RelayCommand(AddJob);
            AddItemCommand = new RelayCommand(CreateScheduler);
            DeleteItemCommand = new RelayCommand(DeleteScheduler);
        }

        public IProject Project { get; set; }

        public SchedulerManager(IProject project, IDialogService dialogService) : this(project)
        {
            DialogService = dialogService;
        }

        private void AddJob()
        {
            JobEditor jobEditor = new JobEditor(Project);
            bool? success = DialogService.ShowDialog<TaskEditor>(this, jobEditor);
            if (success == true)
            {
                Job job = new JobNextComposition(jobEditor.JobName, jobEditor.SelectedPlaylist, (s) => jobEditor.ToRunType.SetRunType(s.WithName(jobEditor.JobName)));
                SelectedScheduler.AddJob(job);
            }
        }

        public ICommand AddJobCommand { get; set; }
        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }


        public IDialogService DialogService { get; set; }
        public PlaylistEditor PlaylistEditor { get; set; }
        public JobEditor JobEditor { get; set; }


        private CompositionScheduler _selectedScheduler;
        public CompositionScheduler SelectedScheduler
        {
            get => _selectedScheduler;
            set => SetProperty(ref _selectedScheduler, value);
        }


        public ObservableCollection<Playlist> Playlists
        {
            get => PlaylistEditor.Project.Playlists;
        }

        public ObservableCollection<CompositionScheduler> CompositionSchedulers { get; set; }



        private int _selectedSchedulerIndex;
        public int SelectedSchedulerIndex
        {
            get => _selectedSchedulerIndex;
            set
            {
                SetProperty(ref _selectedSchedulerIndex, value);
                Messenger.Send<IMessage, int>(new MessageSelectedSchedulerIndex(value), MessageType.Out);
            }
        }



        public void CreateScheduler()
        {
            CompositionSchedulerModel compositionSchedulerModel = new CompositionSchedulerModel();
            CompositionScheduler compositionScheduler = new CompositionScheduler(compositionSchedulerModel);
            CompositionSchedulers.Add(compositionScheduler);

            Messenger.Send<IMessage, int>(new MessageAddScheduler(compositionSchedulerModel), MessageType.Out);
        }

        public void CreateScheduler(CompositionSchedulerModel compositionSchedulerModel)
        {
            CompositionScheduler compositionScheduler = new CompositionScheduler(compositionSchedulerModel);
            CompositionSchedulers.Add(compositionScheduler);
        }

        public void DeleteScheduler()
        {
            int index = CompositionSchedulers.IndexOf(SelectedScheduler);

            if (SelectedScheduler != null)
                CompositionSchedulers.Remove(SelectedScheduler);

            if (index > 0)
            {
                SelectedScheduler = CompositionSchedulers[index - 1];
                return;
            }

            if (index == 0 && CompositionSchedulers.Count > 0)
            {
                SelectedScheduler = CompositionSchedulers[0];
                return;
            }
        }

        public void Receive(ISchedulerMessage message)
        {

        }
    }
}
