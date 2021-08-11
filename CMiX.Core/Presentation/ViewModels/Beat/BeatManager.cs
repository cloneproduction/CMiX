// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Network.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Beat
{
    public class BeatManager : ObservableRecipient, IRecipient<IMessage>
    {
        public BeatManager(IProject project)
        {
            MasterBeats = new ObservableCollection<MasterBeat>();
        }

        public ObservableCollection<MasterBeat> MasterBeats { get; set; }


        public void CreateBeat()
        {

        }

        public void DeleteBeat()
        {

        }



        public void Receive(IMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}
