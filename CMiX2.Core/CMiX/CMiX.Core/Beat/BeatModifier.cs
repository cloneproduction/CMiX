// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Service;
using CMiX.Core.Presentations.Beat;
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
            ControlMessenger.Send<BeatModifierModel>(this);
        }

        public void Multiply()
        {
            if (BeatIndex <= minIndex)
                return;
            BeatIndex--;
            ControlMessenger.Send<BeatModifierModel>(this);
        }

        public void Divide()
        {
            if (BeatIndex >= maxIndex)
                return;
            BeatIndex++;
            ControlMessenger.Send<BeatModifierModel>(this);
        }

        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }

        public void Dispose()
        {

        }
    }
}
