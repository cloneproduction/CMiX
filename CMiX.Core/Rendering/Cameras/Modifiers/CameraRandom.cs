// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Rendering.Cameras.Modifiers
{
    // [ModifierPanel] removed - superseded by Modulation.CameraRandomModifier. No longer addable
    // via the picker; kept so already-saved Project data referencing CameraRandom still loads.
    public partial class CameraRandom : BeatModifiableModifierBase, ICameraModifier, IModifier
    {
        public CameraRandom(PrefabManager beatModifierManager,
                            PrefabService prefabService,
                            GenericValue<bool> pingPong,
                            GenericValue<CameraAxis> axis,
                            GenericValue<float> width)
            : base(prefabService, beatModifierManager)
        {
            PingPong = pingPong;
            Axis = axis;
            Width = width;
        }

        public GenericValue<bool> PingPong { get; set; }
        public GenericValue<CameraAxis> Axis { get; set; }
        public GenericValue<float> Width { get; set; }

        public override IControlModel ToModel()
        {
            var model = new CameraRandomModel
            {
                PingPong = (GenericValueModel<bool>)PingPong.ToModel(),
                Width = (GenericValueModel<float>)Width.ToModel(),
                Axis = (GenericValueModel<CameraAxis>)Axis.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (CameraRandomModel)model;
            LoadBaseModel(m);
            PingPong.FromModel(m.PingPong);
            Width.FromModel(m.Width);
            Axis.FromModel(m.Axis);
        }
    }
}
