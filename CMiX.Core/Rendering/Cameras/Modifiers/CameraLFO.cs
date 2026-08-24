// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Rendering.Cameras.Modifiers
{
    [ModifierPanel(typeof(Camera))]
    public partial class CameraLFO : BeatModifiableModifierBase, ICameraModifier
    {
        public CameraLFO(PrefabManager beatModifierManager,
                         PrefabService prefabService,
                         GenericValue<bool> pingPong,
                         GenericValue<CameraAxis> axis,
                         GenericValue<float> from,
                         GenericValue<float> to)
            : base(prefabService, beatModifierManager)
        {
            PingPong = pingPong;
            Axis = axis;
            From = from;
            To = to;
        }

        public GenericValue<bool> PingPong { get; set; }
        public GenericValue<CameraAxis> Axis { get; set; }
        public GenericValue<float> From { get; set; }
        public GenericValue<float> To { get; set; }

        public override IControlModel ToModel()
        {
            var model = new CameraLFOModel
            {
                PingPong = (GenericValueModel<bool>)PingPong.ToModel(),
                From = (GenericValueModel<float>)From.ToModel(),
                To = (GenericValueModel<float>)To.ToModel(),
                Axis = (GenericValueModel<CameraAxis>)Axis.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (CameraLFOModel)model;
            LoadBaseModel(m);
            PingPong.FromModel(m.PingPong);
            From.FromModel(m.From);
            To.FromModel(m.To);
            Axis.FromModel(m.Axis);
        }
    }
}
