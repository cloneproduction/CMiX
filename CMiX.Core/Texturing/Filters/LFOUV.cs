// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class LFOUV : ObservableObject, IPrefab, IBeatModifiable
    {
        public LFOUV(PrefabManager beatModifierManager,
                     PrefabService prefabService, 
                     GenericValue<TransformType> transformType, 
                     ModifierModeSelector modifierModeSelector, 
                     GenericValue<bool> pingPong, 
                     GenericValue<bool> xAxis, 
                     GenericValue<bool> yAxis, 
                     GenericValue<bool> zAxis, 
                     GenericValue<float> from,
                     GenericValue<float> to,
                     SamplerState samplerState)
        {
            BeatModifierManager = beatModifierManager;
            PrefabService = prefabService;
            TransformType = transformType;
            ModifierModeSelector = modifierModeSelector;

            PingPong = pingPong;
            XAxis = xAxis;
            YAxis = yAxis;
            ZAxis = zAxis;

            From = from;
            To = to;
            SamplerState = samplerState;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<TransformType> TransformType { get; set; }
        public GenericValue<bool> PingPong { get; set; }
        public GenericValue<bool> XAxis { get; set; }
        public GenericValue<bool> YAxis { get; set; }
        public GenericValue<bool> ZAxis { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<float> From { get; set; }
        public GenericValue<float> To { get; set; }
        public SamplerState SamplerState { get; set; }
        public PrefabManager BeatModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
