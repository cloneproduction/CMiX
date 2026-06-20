// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Kaleidoscope : ObservableObject, IPrefab, ITextureFilter
    {
        public Kaleidoscope(PrefabService prefabService,
                            GenericValue<float> control,
                            GenericValue<int> divisions,
                            GenericValue<int> iterations,
                            GenericValue<float> iterationZoom,
                            GenericValue<float> rotation,
                            GenericValue<float> zoom,
                            GenericValue<float> cellRotation,
                            Vector2 center,
                            Vector2 cellOffset,
                            Vector2 cellScale,
                            Blend blend)
        {
            PrefabService = prefabService;
            Control = control;
            Divisions = divisions;
            Iterations = iterations;
            IterationZoom = iterationZoom;
            Rotation = rotation;
            Zoom = zoom;
            CellRotation = cellRotation;
            Center = center;
            CellOffset = cellOffset;
            CellScale = cellScale;
            Blend = blend;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Control { get; set; }
        public GenericValue<int> Divisions { get; set; }
        public GenericValue<int> Iterations { get; set; }
        public GenericValue<float> IterationZoom { get; set; }
        public GenericValue<float> Rotation { get; set; }
        public GenericValue<float> Zoom { get; set; }
        public GenericValue<float> CellRotation { get; set; }
        public Vector2 Center { get; set; }
        public Vector2 CellOffset { get; set; }
        public Vector2 CellScale { get; set; }
        public Blend Blend { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new KaleidoscopeModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Divisions = (GenericValueModel<int>)Divisions.ToModel(),
            Iterations = (GenericValueModel<int>)Iterations.ToModel(),
            IterationZoom = (GenericValueModel<float>)IterationZoom.ToModel(),
            Rotation = (GenericValueModel<float>)Rotation.ToModel(),
            Zoom = (GenericValueModel<float>)Zoom.ToModel(),
            CellRotation = (GenericValueModel<float>)CellRotation.ToModel(),
            Center = (Vector2Model)Center.ToModel(),
            CellOffset = (Vector2Model)CellOffset.ToModel(),
            CellScale = (Vector2Model)CellScale.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel(),
            Blend = (BlendModel)Blend.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (KaleidoscopeModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Divisions.FromModel(m.Divisions);
            Iterations.FromModel(m.Iterations);
            IterationZoom.FromModel(m.IterationZoom);
            Rotation.FromModel(m.Rotation);
            Zoom.FromModel(m.Zoom);
            CellRotation.FromModel(m.CellRotation);
            Center.FromModel(m.Center);
            CellOffset.FromModel(m.CellOffset);
            CellScale.FromModel(m.CellScale);
            Control.FromModel(m.Control);
            Blend.FromModel(m.Blend);
        }
    }
}
