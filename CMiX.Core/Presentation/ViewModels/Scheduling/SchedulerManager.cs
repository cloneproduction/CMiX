// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Models.Scheduling;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class SchedulerManager : ObservableRecipient, IRecipient<ISchedulerMessage>
    {
        public SchedulerManager(IProject project, IDialogService dialogService)
        {
            CompositionSchedulers = new ObservableCollection<CompositionScheduler>();
            PlaylistEditor = new PlaylistEditor();
            JobEditor = new JobEditor(dialogService);

            Messenger.Register(this, MessageType.In);

            IsActive = true;
            AddItemCommand = new RelayCommand(CreateScheduler);
            DeleteItemCommand = new RelayCommand(DeleteScheduler);
        }


        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }


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
            get => PlaylistEditor.Playlists;
        }

        public ObservableCollection<CompositionScheduler> CompositionSchedulers { get; set; }



        private int _selectedSchedulerIndex;
        public int SelectedSchedulerIndex
        {
            get => _selectedSchedulerIndex;
            set
            {
                SetProperty(ref _selectedSchedulerIndex, value);
                Console.WriteLine("SelectedSchedulerIndex = " + SelectedSchedulerIndex);

                Messenger.Send<IMessage, int>(new MessageSelectedSchedulerIndex(value), MessageType.Out);
            }
        }


        public void CreateScheduler()
        {
            CompositionSchedulerModel compositionSchedulerModel = new CompositionSchedulerModel();
            CompositionScheduler compositionScheduler = new CompositionScheduler(compositionSchedulerModel, PlaylistEditor.Playlists);
            CompositionSchedulers.Add(compositionScheduler);

            Messenger.Send<IMessage, int>(new MessageAddScheduler(compositionSchedulerModel), MessageType.Out);

        }

        public void CreateScheduler(CompositionSchedulerModel compositionSchedulerModel)
        {
            CompositionScheduler compositionScheduler = new CompositionScheduler(compositionSchedulerModel, PlaylistEditor.Playlists);
            CompositionSchedulers.Add(compositionScheduler);
            Console.WriteLine("Scheduler Created, Count is " + CompositionSchedulers.Count);
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


        public IModel GetModel()
        {
            SchedulerManagerModel schedulerModel = new SchedulerManagerModel();
            return schedulerModel;
        }

        public void SetViewModel(IModel model)
        {
            SchedulerManagerModel schedulerModel = model as SchedulerManagerModel;
        }

        public void Receive(ISchedulerMessage message)
        {

        }
    }
}
