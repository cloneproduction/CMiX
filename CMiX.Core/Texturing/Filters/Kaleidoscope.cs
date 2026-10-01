// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Kaleidoscope : TextureFilterBase
    {
        public Kaleidoscope(PrefabService prefabService,
                            GenericValue<int> divisions,
                            GenericValue<int> iterations,
                            ModulatableValue<float> iterationZoom,
                            ModulatableValue<float> rotation,
                            ModulatableValue<float> zoom,
                            ModulatableValue<float> cellRotation,
                            ModulatableVector2 center,
                            ModulatableVector2 cellOffset,
                            ModulatableVector2 cellScale,
                            Blend blend,
                            PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            Divisions = divisions;
            Iterations = iterations;
            Center = center;
            CellOffset = cellOffset;
            CellScale = cellScale;

            Bindables = new List<ModulatableValue<float>>
            {
                iterationZoom, rotation, zoom, cellRotation,
                center.X, center.Y,
                cellOffset.X, cellOffset.Y,
                cellScale.X, cellScale.Y
            };

            iterationZoom.Label = "Iteration Zoom";
            rotation.Label = "Rotation";
            zoom.Label = "Zoom";
            cellRotation.Label = "Cell Rotation";

            iterationZoom.SetDefault(0.0f);
            rotation.SetDefault(0.0f);
            zoom.SetDefault(0.5f);
            cellRotation.SetDefault(0.0f);
            center.X.SetDefault(0.0f);
            center.Y.SetDefault(0.0f);
            cellOffset.X.SetDefault(0.0f);
            cellOffset.Y.SetDefault(0.0f);
            cellScale.X.SetDefault(1.0f);
            cellScale.Y.SetDefault(1.0f);
        }

        public GenericValue<int> Divisions { get; set; }
        public GenericValue<int> Iterations { get; set; }
        public ModulatableValue<float> IterationZoom => Bindables[0];
        public ModulatableValue<float> Rotation => Bindables[1];
        public ModulatableValue<float> Zoom => Bindables[2];
        public ModulatableValue<float> CellRotation => Bindables[3];
        public ModulatableVector2 Center { get; }
        public ModulatableVector2 CellOffset { get; }
        public ModulatableVector2 CellScale { get; }

        public override IControlModel ToModel()
        {
            var model = new KaleidoscopeModel
            {
                Divisions = (GenericValueModel<int>)Divisions.ToModel(),
                Iterations = (GenericValueModel<int>)Iterations.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (KaleidoscopeModel)model;
            LoadBaseModel(m);
            Divisions.FromModel(m.Divisions);
            Iterations.FromModel(m.Iterations);
        }
    }
}
