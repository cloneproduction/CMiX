// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Models.Scheduling;
using CMiX.Core.Network.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class SchedulerManager : ObservableRecipient, IRecipient<IMessage>, IControl
    {
        public SchedulerManager(IProject project)
        {
            this.ID = new Guid("22223344-5566-7788-99AA-BBCCDDEEFF00");
            Project = project;
            PlaylistEditor = new PlaylistEditor(project);
            Messenger.Register(this, "IN");
            IsActive = true;
            CreateSchedulerCommand = new RelayCommand(CreateScheduler);
            DeleteSchedulerCommand = new RelayCommand(DeleteScheduler);
        }


        public Guid ID { get; set; }
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
            schedulerModel.ID = this.ID;
            return schedulerModel;
        }

        public void SetViewModel(IModel model)
        {
            SchedulerManagerModel schedulerModel = model as SchedulerManagerModel;
            this.ID = schedulerModel.ID;
        }

        public void Receive(IMessage message)
        {
            if (message is ISchedulerMessage)
                message.Process(this);
        }
    }
}
