// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;
using CMiX.Core.BaseControls;
using CMiX.Core.Services;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Materials
{
    public partial class Material : ObservableRecipient, IControl
    {
        public Material(DiffuseTexture diffuseTexture, MaskTexture maskTexture)
        {
            Pipeline = new GenericValue<PipelineType>(PipelineType.Constant);
            CullMode = new GenericValue<CullModeType>(CullModeType.Back);
            Transparency = new GenericValue<TransparencyType>(TransparencyType.CutOff);

            Metalness = new FloatValue(0.5f);
            Specularity = new FloatValue(0.5f);
            Glossiness = new FloatValue(0.5f);
            Alpha = new FloatValue(1.0f);
            IsShadowCaster = new BooleanValue(false);

            BaseColor = new ColorValue(Color.FromArgb(255, 255, 255, 255));
            DiffuseTexture = diffuseTexture;
            MaskTexture = maskTexture;

            IsActive = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public DiffuseTexture DiffuseTexture { get; set; }
        public MaskTexture MaskTexture { get; set; }
        public ColorValue BaseColor { get; set; }
        public GenericValue<PipelineType> Pipeline { get; set; }
        public GenericValue<TransparencyType> Transparency { get; set; }
        public GenericValue<CullModeType> CullMode { get; set; }
        public FloatValue Metalness { get; set; }
        public FloatValue Specularity { get; set; }
        public FloatValue Glossiness { get; set; }
        public FloatValue Alpha { get; set; }
        public BooleanValue IsShadowCaster { get; set; }


        [ObservableProperty]
        private bool isExpanded = false;

        [ObservableProperty]
        private bool surfaceIsExpanded = false;
    }
}
