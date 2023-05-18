// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Materials
{
    public partial class Material : ObservableRecipient, IPrefab
    {
        public Material(MaterialModel materialModel)
        {
            this.ID = materialModel.ID;
            Name = new StringValue(materialModel.Name);
            IsSelected = new BooleanValue(materialModel.IsSelected);
            IsRenaming = new BooleanValue(materialModel.IsRenaming);
            Texture = new Texture(materialModel.Texture);
            BaseColor = new ColorSelector(materialModel.BaseColor);
            Mask = new Mask(materialModel.Mask);
            MaskChannelSelector = new GenericValue<MaskChannel>(materialModel.MaskChannelSelector);
            Pipeline = new GenericValue<PipelineType>(materialModel.Pipeline);
            CullMode = new GenericValue<CullModeType>(materialModel.CullMode);
            Transparency = new GenericValue<TransparencyType>(materialModel.Transparency);
            Metalness = new FloatValue(materialModel.Metalness);
            Specularity = new FloatValue(materialModel.Specularity);
            Glossiness = new FloatValue(materialModel.Glossiness);
            Alpha = new FloatValue(materialModel.Alpha);
            IsShadowCaster = new BooleanValue(materialModel.IsShadowCaster);
            isExpanded = false;
            IsActive = true;
        }

        public Guid ID { get; set; }
        public Texture Texture { get; set; }
        public ColorSelector BaseColor { get; set; }
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
        public StringValue Name { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public BooleanValue IsSelected { get; set; }

        [ObservableProperty]
        private bool isExpanded;
    }
}
