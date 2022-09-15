// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class DirectionXYZ : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {
        public DirectionXYZ(DirectionXYZModel directionXYZModel)
        {
            ID = directionXYZModel.ID;
            DirectionX = directionXYZModel.DirectionX;
            DirectionY = directionXYZModel.DirectionY;
            DirectionZ = directionXYZModel.DirectionZ;

            IsActive = true;
        }

        public Guid ID { get; set; }

        private bool _directionX;
        public bool DirectionX
        {
            get => _directionX;
            set
            {
                SetProperty(ref _directionX, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
            }
        }

        private bool _directionY;
        public bool DirectionY
        {
            get => _directionY;
            set
            {
                SetProperty(ref _directionY, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
            }
        }

        private bool _directionZ;
        public bool DirectionZ
        {
            get => _directionZ;
            set
            {
                SetProperty(ref _directionZ, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
            }
        }

        public IModel GetModel()
        {
            DirectionXYZModel directionXYZModel = new DirectionXYZModel();

            directionXYZModel.ID = ID;
            directionXYZModel.DirectionX = DirectionX;
            directionXYZModel.DirectionY = DirectionY;
            directionXYZModel.DirectionZ = DirectionZ;

            return directionXYZModel;
        }

        public void SetViewModel(IModel model)
        {
            DirectionXYZModel directionXYZModel = model as DirectionXYZModel;
            this.ID = directionXYZModel.ID;
            DirectionX = directionXYZModel.DirectionX;
            DirectionY = directionXYZModel.DirectionY;
            DirectionZ = directionXYZModel.DirectionZ;
        }

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID && !message.HasReceivedResponse)
                message.Reply(this);
        }
    }
}
