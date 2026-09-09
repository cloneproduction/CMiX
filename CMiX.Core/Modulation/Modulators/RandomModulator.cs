// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Undo;

namespace CMiX.Core.Modulation.Modulators
{
    public class RandomModulator : ReceivableControl, IModulator
    {
        public RandomModulator(PrefabService prefabService,
                               GenericValue<float> center,
                               GenericValue<float> width,
                               GenericValue<int> seed,
                               UndoManager undoManager,
                               ControlActivationService activationService)
        {
            PrefabService = prefabService;
            Center = center;
            Width = width;
            Seed = seed;

            UndoManager = undoManager;
            IsActive = false;
            activationService.Register(this);
        }

        private bool _isHovered;
        public bool IsHovered
        {
            get => _isHovered;
            set { _isHovered = value; OnPropertyChanged(); }
        }

        private bool _isExpanded = true;
        public bool IsExpanded
        {
            get => _isExpanded;
            set { _isExpanded = value; OnPropertyChanged(); }
        }

        public IReadOnlyList<IModulatorOutput> Outputs { get; } = new IModulatorOutput[] {
            new ModulatorOutput<float>("Value"),
        };

        public PrefabService PrefabService { get; set; }
        public Guid ID { get; set; }

        public GenericValue<float> Center { get; set; }
        public GenericValue<float> Width { get; set; }
        public GenericValue<int> Seed { get; set; }

        public IControlModel ToModel() => new RandomModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Center = (GenericValueModel<float>)Center.ToModel(),
            Width = (GenericValueModel<float>)Width.ToModel(),
            Seed = (GenericValueModel<int>)Seed.ToModel(),
        };

        public void FromModel(IControlModel model)
        {
            var m = (RandomModulatorModel)model;
            ID = m.ID;
            Center.FromModel(m.Center);
            Width.FromModel(m.Width);
            Seed.FromModel(m.Seed);
        }
    }
}
