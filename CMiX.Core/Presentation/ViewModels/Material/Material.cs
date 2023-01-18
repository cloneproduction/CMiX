// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
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


        public IModel GetModel()
        {
            MaterialModel model = new MaterialModel();
            model.ID = this.ID;

            model.Texture = (TextureModel)Texture.GetModel();
            model.Mask = (MaskModel)Mask.GetModel();
            model.MaskChannelSelector = (GenericValueModel<MaskChannel>)MaskChannelSelector.GetModel();

            model.Pipeline = (GenericValueModel<PipelineType>)Pipeline.GetModel();
            model.CullMode = (GenericValueModel<CullModeType>)CullMode.GetModel();
            model.Transparency = (GenericValueModel<TransparencyType>)Transparency.GetModel();

            model.Metalness = (FloatValueModel)Metalness.GetModel();
            model.Specularity = (FloatValueModel)Specularity.GetModel();
            model.Glossiness = (FloatValueModel)Glossiness.GetModel();
            model.Alpha = (FloatValueModel)Alpha.GetModel();

            model.IsShadowCaster = (BooleanValueModel)IsShadowCaster.GetModel();

            model.ColorModel = (ColorSelectorModel)Color.GetModel();
            
            return model;
        }

        public void SetViewModel(IModel model)
        {
            MaterialModel materialModel = model as MaterialModel;
            this.ID = materialModel.ID;

            this.Texture.SetViewModel(materialModel.Texture);
            this.Mask.SetViewModel(materialModel.Mask);
            this.MaskChannelSelector.SetViewModel(materialModel.MaskChannelSelector);

            this.Pipeline.SetViewModel(materialModel.Pipeline);
            this.CullMode.SetViewModel(materialModel.CullMode);
            this.Transparency.SetViewModel(materialModel.Transparency);

            this.Metalness.SetViewModel(materialModel.Metalness);
            this.Specularity.SetViewModel(materialModel.Specularity);
            this.Glossiness.SetViewModel(materialModel.Glossiness);
            this.Alpha.SetViewModel(materialModel.Alpha);

            this.IsShadowCaster.SetViewModel(materialModel.IsShadowCaster);

            this.Color.SetViewModel(materialModel.ColorModel);
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
