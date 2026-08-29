// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation
{
    [ModifierPanel(typeof(Entity))]
    public partial class TransformSRTModifier : ObservableObject, IModifier, IDisposable
    {
        public TransformSRTModifier(PrefabService prefabService,
                            TranslateModifier translate,
                            ScaleModifier scale,
                            RotationModifier rotation,
                            DirectionXYZ directionXYZ,
                            GenericValue<ModifierMode> mode)
        {
            PrefabService = prefabService;
            Translate = translate;
            Scale = scale;
            Rotation = rotation;
            DirectionXYZ = directionXYZ;
            Mode = mode;
            isExpanded = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public TranslateModifier Translate { get; set; }
        public ScaleModifier Scale { get; set; }
        public RotationModifier Rotation { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }
        public PrefabService PrefabService { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {
            Translate.Dispose();
            Scale.Dispose();
            Rotation.Dispose();
        }

        public IControlModel ToModel() => new TransformSRTModifierModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            DirectionXYZ = (DirectionXYZModel)DirectionXYZ.ToModel(),
            Translate = (TranslateModifierModel)Translate.ToModel(),
            Scale = (ScaleModifierModel)Scale.ToModel(),
            Rotation = (RotationModifierModel)Rotation.ToModel(),
            Mode = (GenericValueModel<ModifierMode>)Mode.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (TransformSRTModifierModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            DirectionXYZ.FromModel(m.DirectionXYZ);
            Translate.FromModel(m.Translate);
            Scale.FromModel(m.Scale);
            Rotation.FromModel(m.Rotation);
            Mode.FromModel(m.Mode);
        }
    }
}
