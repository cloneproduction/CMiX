// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentation.ViewModels.Service;
using CMiX.Core.Texturing.Sources.BubbleNoise;
using CMiX.Core.Texturing.Sources.Gradient;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class ProceduralSelector : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {
        public ProceduralSelector(ProceduralSelectorModel proceduralTextureSelectorModel, CompositionService compositionService)
        {
            ID = proceduralTextureSelectorModel.ID;
            ProceduralName = new GenericValue<TextureSourceName>(proceduralTextureSelectorModel.ProceduralName);
            CompositionService = compositionService;
            Gradient = new Gradient(proceduralTextureSelectorModel.Gradient, compositionService);
            BubbleNoise = new BubbleNoise(proceduralTextureSelectorModel.BubbleNoise, compositionService);
        }

        public Guid ID { get; set; }

        public Gradient Gradient { get; set; }
        public BubbleNoise BubbleNoise { get; set; }

        public GenericValue<TextureSourceName> ProceduralName { get; set; }
        public CompositionService CompositionService { get; set; }


        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID && !message.HasReceivedResponse)
                message.Reply(this);
        }
    }
}
