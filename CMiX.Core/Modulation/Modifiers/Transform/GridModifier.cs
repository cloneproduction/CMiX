// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Modulation.Modifiers
{
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class GridModifier : Modifier, ISpreadableModifier3
    {
        public GridModifier(PrefabService prefabService,
                            PrefabManager modulatorManager,
                            ControlRepository controlRepository,
                            ModulatableValue<int> countX, ModulatableValue<int> countY, ModulatableValue<int> countZ,
                            ModulatableValue<float> widthX, ModulatableValue<float> widthY, ModulatableValue<float> widthZ,
                            ModulatableValue<float> phaseX, ModulatableValue<float> phaseY, ModulatableValue<float> phaseZ)
            : base(prefabService, modulatorManager, controlRepository, new IModulatorBindable[] { countX, countY, countZ })
        {
            Count = new ModulatableInteger3(countX, countY, countZ);

            Width = new ModulatableVector3(widthX, widthY, widthZ);
            Phase = new ModulatableVector3(phaseX, phaseY, phaseZ);
            Bindables = new List<ModulatableValue<float>> { widthX, widthY, widthZ, phaseX, phaseY, phaseZ };
        }

        public ModulatableVector3 Width { get; }
        public ModulatableVector3 Phase { get; }
        public ModulatableInteger3 Count { get; }

        public override IControlModel ToModel()
        {
            var model = new GridModifierModel
            {
                CountX = (ModulatableValueModel<int>)Count.X.ToModel(),
                CountY = (ModulatableValueModel<int>)Count.Y.ToModel(),
                CountZ = (ModulatableValueModel<int>)Count.Z.ToModel(),
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (GridModifierModel)model;
            LoadBaseModel(m);
            Count.X.FromModel(m.CountX);
            Count.Y.FromModel(m.CountY);
            Count.Z.FromModel(m.CountZ);
            ResolveNestedBindables();
        }
    }
}
