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
        public Material(CompositionService compositionService)
        {
            Name = new StringValue();

            IsSelected = new BooleanValue();
            IsRenaming = new BooleanValue();

            Pipeline = new GenericValue<PipelineType>(PipelineType.Constant);
            CullMode = new GenericValue<CullModeType>(CullModeType.Back);
            Transparency = new GenericValue<TransparencyType>(TransparencyType.CutOff);

            Metalness = compositionService.GetFloatControl(0.5f);
            Specularity = compositionService.GetFloatControl(0.5f);
            Glossiness = compositionService.GetFloatControl(0.5f);
            Alpha = compositionService.GetFloatControl(1.0f);
            IsShadowCaster = compositionService.GetBooleanControl(false);

            BaseColor = new ColorSelector(Color.FromArgb(255, 255, 255, 255));
            DiffuseTexture = new DiffuseTexture(compositionService);
            MaskTexture = new MaskTexture(compositionService);

            IsActive = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();

        public DiffuseTexture DiffuseTexture { get; set; }
        public MaskTexture MaskTexture { get; set; }
        public ColorSelector BaseColor { get; set; }
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
