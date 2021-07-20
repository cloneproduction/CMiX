// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Models.Scheduling;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Network.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class SchedulerManager : ObservableObject, IControl
    {
        public SchedulerManager(IProject project)
        {
            this.ID = new Guid("22223344-5566-7788-99AA-BBCCDDEEFF00");
            Project = project;
            PlaylistEditor = new PlaylistEditor(project);

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
                Communicator?.SendMessage(new MessageSelectedSchedulerIndex(value));
                Console.WriteLine("SelectedSchedulerIndex = " + SelectedSchedulerIndex);
            }
        }


        public void CreateScheduler()
        {
            CompositionSchedulerModel compositionSchedulerModel = new CompositionSchedulerModel();
            CompositionScheduler compositionScheduler = new CompositionScheduler(compositionSchedulerModel, PlaylistEditor.Playlists);
            compositionScheduler.SetCommunicator(Communicator);
            Project.CompositionSchedulers.Add(compositionScheduler);

            Communicator?.SendMessage(new MessageAddScheduler(compositionSchedulerModel));
        }

        public void CreateScheduler(CompositionSchedulerModel compositionSchedulerModel)
        {
            CompositionScheduler compositionScheduler = new CompositionScheduler(compositionSchedulerModel, PlaylistEditor.Playlists);
            compositionScheduler.SetCommunicator(Communicator);
            Project.CompositionSchedulers.Add(compositionScheduler);
            Console.WriteLine("Scheduler Created");
        }

        public void DeleteScheduler()
        {
            Project.CompositionSchedulers.Remove(SelectedScheduler);
            SelectedScheduler?.UnsetCommunicator(Communicator);
            SelectedScheduler = null;
        }


        public Communicator Communicator { get; set; }

        public void SetCommunicator(Communicator communicator)
        {
            Communicator = new Communicator(this);
            Communicator.SetCommunicator(communicator);

            PlaylistEditor.SetCommunicator(Communicator);
        }

        public void UnsetCommunicator(Communicator communicator)
        {
            Communicator.UnsetCommunicator(communicator);
            PlaylistEditor.UnsetCommunicator(Communicator);
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
    }
}
