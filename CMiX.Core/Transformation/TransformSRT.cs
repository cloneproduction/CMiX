// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation
{
    [ModifierPanel(typeof(Entity))]
    public partial class TransformSRT : ObservableObject, IModifier
    {
        public TransformSRT(PrefabService prefabService, 
                            GenericValue<float> uniform, 
                            Translate translate, 
                            Scale scale, 
                            Rotation rotation,
                            DirectionXYZ directionXYZ, 
                            GenericValue<ModifierMode> mode)
        {
            PrefabService = prefabService;
            Uniform = uniform;
            Translate = translate;
            Scale = scale;
            Rotation = rotation;
            DirectionXYZ = directionXYZ;
            Mode = mode;
            isExpanded = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<float> Uniform { get; set; }
        public Translate Translate { get; set; }
        public Scale Scale { get; set; }
        public Rotation Rotation { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }
        public PrefabService PrefabService { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        public IControlModel ToModel() => new TransformSRTModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Uniform = (GenericValueModel<float>)Uniform.ToModel(),
            DirectionXYZ = (DirectionXYZModel)DirectionXYZ.ToModel(),
            Translate = (TranslateModel)Translate.ToModel(),
            Scale = (ScaleModel)Scale.ToModel(),
            Rotation = (RotationModel)Rotation.ToModel(),
            Mode = (GenericValueModel<ModifierMode>)Mode.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (TransformSRTModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Uniform.FromModel(m.Uniform);
            DirectionXYZ.FromModel(m.DirectionXYZ);
            Translate.FromModel(m.Translate);
            Scale.FromModel(m.Scale);
            Rotation.FromModel(m.Rotation);
            Mode.FromModel(m.Mode);
        }
    }
}
