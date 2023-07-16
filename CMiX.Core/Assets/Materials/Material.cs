// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefab;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Materials
{
    public partial class Material : ObservableRecipient, IPrefab
    {
        public Material()
        {
            Name = new StringValue();

            IsSelected = new BooleanValue();
            IsRenaming = new BooleanValue();
            Texture = new Texture();
            
            Mask = new Mask();
            MaskChannelSelector = new GenericValue<MaskChannel>();
            Pipeline = new GenericValue<PipelineType>(PipelineType.Constant);
            CullMode = new GenericValue<CullModeType>(CullModeType.Back);
            Transparency = new GenericValue<TransparencyType>(TransparencyType.CutOff);
            Metalness = new FloatValue(0.5f);
            Specularity = new FloatValue(0.5f);
            Glossiness = new FloatValue(0.5f);
            Alpha = new FloatValue(1.0f);
            IsShadowCaster = new BooleanValue(false);

            IsActive = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Texture Texture { get; set; }
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
        private bool isExpanded = false;

        [ObservableProperty]
        private bool surfaceIsExpanded = false;
    }
}
