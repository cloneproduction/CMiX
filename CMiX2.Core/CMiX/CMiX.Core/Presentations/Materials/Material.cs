// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.Texturing;
using CMiX.Core.Presentations.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentations.Materials
{
    public class Material : ObservableRecipient, IRecipient<MessageRequestControl>, IPrefab, IDisposable
    {
        public Material(MaterialModel materialModel, CompositionService compositionService)
        {
            this.ID = materialModel.ID;
            Name = this.GetType().Name;
            CompositionService = compositionService;

            Texture = new Texture(materialModel.Texture, compositionService);

            Mask = new Mask(materialModel.Mask, compositionService);
            MaskChannelSelector = new GenericValue<MaskChannel>(materialModel.MaskChannelSelector);

            Pipeline = new GenericValue<PipelineType>(materialModel.Pipeline);
            CullMode = new GenericValue<CullModeType>(materialModel.CullMode);
            Transparency = new GenericValue<TransparencyType>(materialModel.Transparency);

            Metalness = new FloatValue(materialModel.Metalness);
            Specularity = new FloatValue(materialModel.Specularity);
            Glossiness = new FloatValue(materialModel.Glossiness);
            Alpha = new FloatValue(materialModel.Alpha);
            IsShadowCaster = new BooleanValue(materialModel.IsShadowCaster);

            IsActive = true;

            Color = new ColorSelector(materialModel.ColorModel);
        }


        public Guid ID { get; set; }
        public CompositionService CompositionService { get; set; }
        public Texture Texture { get; set; }
        public  TransformSRT TransformSRT { get; set; }
        public ColorSelector Color { get; set; }


        public Mask Mask { get; set; }
        public GenericValue<MaskChannel> MaskChannelSelector { get; set; }
        public GenericValue<PipelineType> Pipeline { get; set; }
        public GenericValue<TransparencyType> Transparency { get; set; }
        public GenericValue<CullModeType> CullMode { get; set; }

        public FloatValue Metalness { get; set; }
        public FloatValue Specularity { get; set; }
        public FloatValue Glossiness { get; set; }
        public FloatValue Alpha { get; set; }
        public BooleanValue IsShadowCaster { get; set; }


        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set => SetProperty(ref _isRenaming, value);
        }

        private string _isExpanded;
        public string IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }


        public void Dispose()
        {

        }

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID && !message.HasReceivedResponse)
                message.Reply(this);
        }
    }
}
