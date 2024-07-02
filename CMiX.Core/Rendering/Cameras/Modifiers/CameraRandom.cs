// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering.Cameras.Modifiers
{
    public partial class CameraRandom : ObservableObject, IControl, IBeatModifiable, IPrefab
    {
        public CameraRandom(PrefabManager beatModifierManager,
                            PrefabService prefabService, 
                            BeatModifier beatModifier, 
                            Easing easing, 
                            GenericValue<bool> pingPong, 
                            GenericValue<CameraAxis> axis, 
                            GenericValue<float> width)
        {
            BeatModifierManager = beatModifierManager;
            PrefabService = prefabService;
            BeatModifier = beatModifier;
            Easing = easing;
            PingPong = pingPong;
            Axis = axis;
            Width = width;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public GenericValue<bool> PingPong { get; set; }
        public GenericValue<CameraAxis> Axis { get; set; }
        public Easing Easing { get; set; }
        public GenericValue<float> Width { get; set; }
        public PrefabManager BeatModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
