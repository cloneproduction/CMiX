// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Modulation.Modulators
{
    // Stand-in for a future real tracking-data source (e.g. a live headcount from a camera) - lets
    // Kind = Set (lock-and-override, vs. BeatModulator's Modulate/blend) be exercised live in the
    // running app before any real tracking integration exists. Count is manually typed here, in
    // place of what a live tracker would report on its own; the engine side of actually computing
    // it is out of scope.
    public partial class TrackingModulator : ObservableObject, IModulator
    {
        public TrackingModulator(PrefabService prefabService, GenericValue<int> count)
        {
            PrefabService = prefabService;
            Count = count;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }

        [ObservableProperty]
        private bool isHovered;

        [ObservableProperty]
        private bool isExpanded = true;

        public ModulatorKind Kind => ModulatorKind.Set;
        public IReadOnlyList<string> OutputNames { get; } = new[] { "Count" };

        public GenericValue<int> Count { get; set; }

        public IControlModel ToModel() => new TrackingModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Count = (GenericValueModel<int>)Count.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (TrackingModulatorModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Count.FromModel(m.Count);
        }
    }
}
