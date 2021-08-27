// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Models.Scheduling;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Components;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class SchedulerManager : ObservableRecipient, IRecipient<ISchedulerMessage>
    {
        public SchedulerManager(IProject project)
        {
            Project = project;
            PlaylistEditor = new PlaylistEditor(project);
            Messenger.Register(this, MessageType.In);

            IsActive = true;
            CreateSchedulerCommand = new RelayCommand(CreateScheduler);
            DeleteSchedulerCommand = new RelayCommand(DeleteScheduler);
        }

        public ICommand CreateSchedulerCommand { get; set; }
        public ICommand DeleteSchedulerCommand { get; set; }


        public IProject Project { get; set; }
        public PlaylistEditor PlaylistEditor { get; set; }


        private CompositionScheduler _selectedScheduler;
        public CompositionScheduler SelectedScheduler
        {
            get => _selectedScheduler;
            set => SetProperty(ref _selectedScheduler, value);
        }


        private int _selectedSchedulerIndex;
        public int SelectedSchedulerIndex
        {
            get => _selectedSchedulerIndex;
            set
            {
                SetProperty(ref _selectedSchedulerIndex, value);
                Console.WriteLine("SelectedSchedulerIndex = " + SelectedSchedulerIndex);

                Messenger.Send<IMessage, string>(new MessageSelectedSchedulerIndex(value), "OUT");
            }
        }


        public void CreateScheduler()
        {
            CompositionSchedulerModel compositionSchedulerModel = new CompositionSchedulerModel();
            CompositionScheduler compositionScheduler = new CompositionScheduler(compositionSchedulerModel, PlaylistEditor.Playlists);
            Project.CompositionSchedulers.Add(compositionScheduler);

            Messenger.Send<IMessage, string>(new MessageAddScheduler(compositionSchedulerModel), "OUT");

        }

        public void CreateScheduler(CompositionSchedulerModel compositionSchedulerModel)
        {
            CompositionScheduler compositionScheduler = new CompositionScheduler(compositionSchedulerModel, PlaylistEditor.Playlists);
            Project.CompositionSchedulers.Add(compositionScheduler);
            Console.WriteLine("Scheduler Created, Count is " + Project.CompositionSchedulers.Count);
        }

        public void DeleteScheduler()
        {
            Project.CompositionSchedulers.Remove(SelectedScheduler);
            SelectedScheduler = null;
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
