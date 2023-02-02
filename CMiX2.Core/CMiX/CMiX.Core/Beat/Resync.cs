// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Models.Beat;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Beat
{
    public class Resync : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {
        public Resync(BeatAnimations beatAnimations, ResyncModel resyncModel)
        {
            this.ID = resyncModel.ID;
            BeatAnimations = beatAnimations;
            ResyncCommand = new RelayCommand(DoResync);
            IsActive = true;
        }


        public Guid ID { get; set; }
        public BeatAnimations BeatAnimations { get; set; }
        public ICommand ResyncCommand { get; }


        private bool _resynced;
        public bool Resynced
        {
            get { return _resynced; }
            set { _resynced = value; }
        }


        public void DoResync()
        {
            BeatAnimations.ResetAnimation();
            OnBeatResync();
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this.GetModel()), MessageType.Out);
        }


        public event EventHandler BeatResync;
        public void OnBeatResync()
        {
            EventHandler handler = BeatResync;
            if (null != handler) handler(this, EventArgs.Empty);
        }

        public IModel GetModel()
        {
            ResyncModel model = new ResyncModel();
            model.ID = this.ID;
            return model;
        }

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID && !message.HasReceivedResponse)
            {
                message.Reply(this);
                OnBeatResync();
            }
        }
    }
}
