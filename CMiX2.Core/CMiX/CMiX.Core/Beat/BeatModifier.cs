// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Beat
{
    public class BeatModifier : ObservableRecipient, 
        IRecipient<MessageRequestControl>,
        IControl,
        IDisposable
    {
        public BeatModifier(BeatModifierModel beatModifierModel, CompositionService compositionService)
        {
            this.ID = beatModifierModel.ID;
            CompositionService = compositionService;

            ChanceToHit = new FloatValue(beatModifierModel.ChanceToHit);

            ResetCommand = new RelayCommand(Reset);
            MultiplyCommand = new RelayCommand(Multiply);
            DivideCommand = new RelayCommand(Divide);

            IsActive = true;
        }


        public ICommand ResetCommand { get; set; }
        public ICommand MultiplyCommand { get; set; }
        public ICommand DivideCommand { get; set; }


        public Guid ID { get; set; }
        public FloatValue ChanceToHit { get; set; }


        private int maxIndex = 4;
        private int minIndex = -4;


        private CompositionService _compositionService;
        public CompositionService CompositionService
        {
            get => _compositionService;
            set => SetProperty(ref _compositionService, value);
        }


        private int _beatIndex;
        public int BeatIndex
        {
            get => _beatIndex;
            set => SetProperty(ref _beatIndex, value);
        }


        public void Reset()
        {
            BeatIndex = 0;
            SendMessage();
        }

        public void Multiply()
        {
            if (BeatIndex <= minIndex)
                return;
            BeatIndex--;
            SendMessage();
        }

        public void Divide()
        {
            if (BeatIndex >= maxIndex)
                return;
            BeatIndex++;
            SendMessage();
        }

        public void SendMessage()
        {
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this.GetModel()), MessageType.Out);
        }

        public IModel GetModel()
        {
            BeatModifierModel model = new BeatModifierModel();
            model.ID = this.ID;
            model.BeatIndex = this.BeatIndex;
            model.ChanceToHit = (FloatValueModel)ChanceToHit.GetModel();
            return model;
        }

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID)
            {
                if (!message.HasReceivedResponse)
                    message.Reply(this);
            }
        }

        public void Dispose()
        {

        }
    }
}
