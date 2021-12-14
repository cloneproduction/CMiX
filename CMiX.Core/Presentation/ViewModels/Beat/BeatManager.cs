// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CMiX.Core.Models.Beat;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Components;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CMiX.Core.Presentation.ViewModels.Beat
{
    public class BeatManager : ObservableRecipient, IRecipient<IMessage>
    {
        public BeatManager(IProject project)
        {
            Project = project;
            MasterBeats = new ObservableCollection<MasterBeat>();
            MasterBeats.CollectionChanged += MasterBeats_CollectionChanged;
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);

            AddItemCommand = new RelayCommand(CreateBeat);
            DeleteItemCommand = new RelayCommand(DeleteBeat);

            ResetCommand = new RelayCommand(Reset);
            MultiplyCommand = new RelayCommand(Multiply);
            DivideCommand = new RelayCommand(Divide);
            TapCommand = new RelayCommand(Tap);
            ResyncCommand = new RelayCommand(Resync);
        }


        private void MasterBeats_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Broadcast<ObservableCollection<MasterBeat>>(e.OldItems as ObservableCollection<MasterBeat>, e.NewItems as ObservableCollection<MasterBeat>, nameof(MasterBeats));
        }

        public IProject Project { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand MultiplyCommand { get; set; }
        public ICommand DivideCommand { get; set; }
        public ICommand TapCommand { get; }
        public ICommand ResyncCommand { get; }

        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }



        private ObservableCollection<MasterBeat> _masterBeats;
        public ObservableCollection<MasterBeat> MasterBeats
        {
            get => _masterBeats;
            set
            {
                SetProperty(ref _masterBeats, value, true);
                Debug.WriteLine("POUETPOUET");
                System.Console.WriteLine("POUET");
            }
        }


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
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageAddBeat(), MessageType.Out);
        }

        public void DeleteBeat()
        {
            int index = MasterBeats.IndexOf(SelectedMasterBeat);

            if (SelectedMasterBeat != null)
                MasterBeats.Remove(SelectedMasterBeat);

            if (index > 0)
            {
                SelectedMasterBeat = MasterBeats[index - 1];
                return;
            }

            if (index == 0 && MasterBeats.Count > 0)
            {
                SelectedMasterBeat = MasterBeats[0];
                return;
            }
        }


        public void Receive(IMessage message)
        {
            if (message is MessageAddBeat messageAddBeat)
            {
                var masterBeatModel = new MasterBeatModel();
                masterBeatModel.ID = message.ID;
                MasterBeat masterBeat = new MasterBeat(masterBeatModel);
                MasterBeats.Add(masterBeat);
                System.Console.WriteLine("MasterBeats.Count" + MasterBeats.Count);
                return;
            }

            if (message is MessageMasterBeatChange messageMasterBeatChange)
            {
                WeakReferenceMessenger.Default.Send(messageMasterBeatChange, MessageType.Internal);
                System.Console.WriteLine("messageMasterBeatChange");
            }
        }

        private void SendMessage()
        {
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageMasterBeatChange(SelectedMasterBeat), MessageType.Internal);
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageMasterBeatChange(SelectedMasterBeat), MessageType.Out);
        }

        public void Reset()
        {
            SelectedMasterBeat.Reset();
            SendMessage();
        }

        public void Multiply()
        {
            SelectedMasterBeat.Multiply();
            SendMessage();
        }

        public void Divide()
        {
            SelectedMasterBeat.Divide();
            SendMessage();
        }

        public void Tap()
        {
            SelectedMasterBeat?.Tap();
            SendMessage();
        }

        public void Resync()
        {
            SelectedMasterBeat?.Resync.DoResync();
        }
    }
}
