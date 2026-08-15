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
    public partial class CameraRandom : ObservableObject, IBeatModifiable, IPrefab, ICameraModifier, IDisposable
    {
        public CameraRandom(PrefabManager beatModifierManager,
                            PrefabService prefabService, 
                            GenericValue<bool> pingPong, 
                            GenericValue<CameraAxis> axis, 
                            GenericValue<float> width)
        {
            BeatModifierManager = beatModifierManager;
            PrefabService = prefabService;
            PingPong = pingPong;
            Axis = axis;
            Width = width;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<bool> PingPong { get; set; }
        public GenericValue<CameraAxis> Axis { get; set; }
        public GenericValue<float> Width { get; set; }
        public PrefabManager BeatModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new CameraRandomModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            PingPong = (GenericValueModel<bool>)PingPong.ToModel(),
            Width = (GenericValueModel<float>)Width.ToModel(),
            Axis = (GenericValueModel<CameraAxis>)Axis.ToModel(),
            BeatModifierManager = (PrefabManagerModel)BeatModifierManager.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (CameraRandomModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            PingPong.FromModel(m.PingPong);
            Width.FromModel(m.Width);
            Axis.FromModel(m.Axis);

            LoadManager(BeatModifierManager, m.BeatModifierManager);
        }
        public void Dispose()
        {
            BeatModifierManager.Dispose();
        }
    }
}
