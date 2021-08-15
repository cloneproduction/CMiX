// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Models.Beat;
using CMiX.Core.Network.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Beat
{
    public class BeatManager : ObservableRecipient, IRecipient<IMessage>
    {
        public BeatManager(IProject project)
        {
            MasterBeats = new ObservableCollection<MasterBeat>();

            AddBeatCommand = new RelayCommand(CreateBeat);
            DeleteBeatCommand = new RelayCommand<MasterBeat>(DeleteBeat);
        }

        public IProject Project { get; set; }
        public ICommand AddBeatCommand { get; set; }
        public ICommand DeleteBeatCommand { get; set; }
        public ObservableCollection<MasterBeat> MasterBeats { get; set; }


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
    }
}
