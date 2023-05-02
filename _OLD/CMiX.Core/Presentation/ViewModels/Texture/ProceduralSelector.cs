// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Service;
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


        public IModel GetModel()
        {
            ProceduralSelectorModel proceduralSelectorModel = new ProceduralSelectorModel();

            proceduralSelectorModel.ProceduralName = (GenericValueModel<TextureSourceName>)ProceduralName.GetModel();
            proceduralSelectorModel.Gradient = (GradientModel)Gradient.GetModel();
            proceduralSelectorModel.BubbleNoise = (BubbleNoiseModel)BubbleNoise.GetModel();

            return proceduralSelectorModel;
        }

        public void SetViewModel(IModel model)
        {
            ProceduralSelectorModel proceduralSelectorModel = new ProceduralSelectorModel();

            ProceduralName.SetViewModel(proceduralSelectorModel.ProceduralName);
            Gradient.SetViewModel(proceduralSelectorModel.Gradient);
            BubbleNoise.SetViewModel(proceduralSelectorModel.BubbleNoise);
        }

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID && !message.HasReceivedResponse)
                message.Reply(this);
        }
    }
}
