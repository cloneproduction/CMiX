// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class SamplerState : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {
        public SamplerState(SamplerStateModel samplerStateModel, CompositionService compositionService)
        {
            ID = samplerStateModel.ID;
            CompositionService = compositionService;

            BorderColor = new ColorSelector(samplerStateModel.BorderColor);
            AddressU = samplerStateModel.AddressU;
            AddressV = samplerStateModel.AddressV;

            IsActive = true;
        }

        public Guid ID { get; set; }
        public CompositionService CompositionService { get; set; }


        public ColorSelector BorderColor { get; set; }

        private string _AddressU;
        public string AddressU
        {
            get => _AddressU;
            set
            {
                SetProperty(ref _AddressU, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this.GetModel()), MessageType.Out);
            }
        }

        private string _AddressV;
        public string AddressV
        {
            get => _AddressV;
            set
            {
                SetProperty(ref _AddressV, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this.GetModel()), MessageType.Out);
            }
        }


        public IModel GetModel()
        {
            SamplerStateModel samplerStateModel = new SamplerStateModel();

            samplerStateModel.ID = ID;
            samplerStateModel.AddressU = AddressU;
            samplerStateModel.AddressV = AddressV;
            samplerStateModel.BorderColor = (ColorSelectorModel)BorderColor.GetModel();

            return samplerStateModel;
        }

        public void SetViewModel(IModel model)
        {
            SamplerStateModel samplerStateModel = model as SamplerStateModel;
            ID = samplerStateModel.ID;
            AddressU = samplerStateModel.AddressU;
            AddressV = samplerStateModel.AddressV;
            BorderColor.SetViewModel(samplerStateModel.BorderColor);
        }

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID && !message.HasReceivedResponse)
                message.Reply(this);
        }
    }
}
