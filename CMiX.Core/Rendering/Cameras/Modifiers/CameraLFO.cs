// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering.Cameras.Modifiers
{
    public partial class CameraLFO : ObservableObject, IPrefab, IBeatModifiable, ICameraModifier
    {
        public CameraLFO(PrefabManager beatModifierManager,
                         PrefabService prefabService,
                         GenericValue<bool> pingPong, 
                         GenericValue<CameraAxis> axis, 
                         GenericValue<float> from, 
                         GenericValue<float> to)
        {
            PrefabService = prefabService;
            BeatModifierManager = beatModifierManager;
            PingPong = pingPong;
            Axis = axis;
            From = from;
            To = to;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public PrefabManager BeatModifierManager { get; set; }
        public GenericValue<bool> PingPong { get; set; }
        public GenericValue<CameraAxis> Axis { get; set; }
        public GenericValue<float> From { get; set; }
        public GenericValue<float> To { get; set; }


        [ObservableProperty]
        private bool isExpanded = true;
    }
}
