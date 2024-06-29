// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Transformation
{
    public class TransformSRTModel : IControlModel, IPrefabModel
    {
        public TransformSRTModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            Translate = new TranslateModel();
            Scale = new ScaleModel();
            Rotation = new RotationModel();
            Visible = new GenericValueModel<bool>(true);
            Uniform = new GenericValueModel<float>(1.0f);
            Mode = new GenericValueModel<ModifierMode>(ModifierMode.ToSpread);
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<float> Uniform { get; set; }
        public TranslateModel Translate { get; set; }
        public ScaleModel Scale { get; set; }
        public RotationModel Rotation { get; set; }
        public GenericValueModel<bool> Visible { get; set; }
        public GenericValueModel<ModifierMode> Mode { get; set; }
    }
}
