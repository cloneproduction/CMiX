// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Models.Beat;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Components;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Beat
{
    public class BeatManager : ObservableRecipient, IRecipient<IMessage>
    {
        public BeatManager(IProject project)
        {
            Project = project;
            MasterBeats = new ObservableCollection<MasterBeat>();

            AddBeatCommand = new RelayCommand(CreateBeat);
            DeleteBeatCommand = new RelayCommand<MasterBeat>(DeleteBeat);

            ResetCommand = new RelayCommand(Reset);
            MultiplyCommand = new RelayCommand(Multiply);
            DivideCommand = new RelayCommand(Divide);
            TapCommand = new RelayCommand(Tap);
            ResyncCommand = new RelayCommand(Resync);
        }

        public IProject Project { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand MultiplyCommand { get; set; }
        public ICommand DivideCommand { get; set; }
        public ICommand TapCommand { get; }
        public ICommand ResyncCommand { get; }

        public ICommand AddBeatCommand { get; set; }
        public ICommand DeleteBeatCommand { get; set; }


        public ObservableCollection<MasterBeat> MasterBeats { get; set; }


        public ObservableCollection<IComponent> Components
        {
            get => Project.Components;
        }

        private MasterBeat _selectedMasterBeat;
        public MasterBeat SelectedMasterBeat
        {
            get => _selectedMasterBeat;
            set => SetProperty(ref _selectedMasterBeat, value);
        }

        public void CreateBeat()
        {
            var masterBeatModel = new MasterBeatModel();
            MasterBeat masterBeat = new MasterBeat(masterBeatModel);
            MasterBeats.Add(masterBeat);
        }

        public void DeleteBeat(MasterBeat masterBeat)
        {
            MasterBeats.Remove(masterBeat);
        }


        public void Receive(IMessage message)
        {

        }

        public void Reset()
        {
            SelectedMasterBeat.Reset();
            WeakReferenceMessenger.Default.Send(new MessageMasterBeatChange(SelectedMasterBeat), MessageType.Internal);
        }

        public void Multiply()
        {
            SelectedMasterBeat.Multiply();
            WeakReferenceMessenger.Default.Send(new MessageMasterBeatChange(SelectedMasterBeat), MessageType.Internal);
        }

        public void Divide()
        {
            SelectedMasterBeat.Divide();
            WeakReferenceMessenger.Default.Send(new MessageMasterBeatChange(SelectedMasterBeat), MessageType.Internal);
        }

        public void Tap()
        {
            SelectedMasterBeat?.Tap();
            WeakReferenceMessenger.Default.Send(new MessageMasterBeatChange(SelectedMasterBeat), MessageType.Internal);
        }

        public void Resync()
        {
            SelectedMasterBeat?.Resync.DoResync();
        }
    }
}
