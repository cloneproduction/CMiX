// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Modulation.Modulators
{
    public partial class TrackingModulator : ObservableObject, IModulator
    {
        public TrackingModulator(PrefabService prefabService, GenericValue<int> count, GenericValue<float> x)
        {
            PrefabService = prefabService;
            Count = count;
            X = x;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }

        [ObservableProperty]
        private bool isHovered;

        [ObservableProperty]
        private bool isExpanded = true;

        public IReadOnlyList<ModulatorOutput> Outputs { get; } = new[]
        {
            new ModulatorOutput("Count", ModulatorKind.Set, typeof(int)),
            new ModulatorOutput("X", ModulatorKind.Set, typeof(float))
        };

        public GenericValue<int> Count { get; set; }
        public GenericValue<float> X { get; set; }

        public IControlModel ToModel() => new TrackingModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Count = (GenericValueModel<int>)Count.ToModel(),
            X = (GenericValueModel<float>)X.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (TrackingModulatorModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Count.FromModel(m.Count);
            X.FromModel(m.X);
        }
    }
}
