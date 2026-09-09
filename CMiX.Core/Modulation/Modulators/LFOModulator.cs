// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Undo;

namespace CMiX.Core.Modulation.Modulators
{
    public class LFOModulator : ReceivableControl, IModulator
    {
        public LFOModulator(PrefabService prefabService,
                            GenericValue<float> period,
                            GenericValue<float> minimum,
                            GenericValue<float> maximum,
                            GenericValue<bool> pingPong,
                            GenericValue<float> randomizePhase,
                            Easing easing,
                            UndoManager undoManager,
                            ControlActivationService activationService)
        {
            PrefabService = prefabService;
            Period = period;
            Minimum = minimum;
            Maximum = maximum;
            PingPong = pingPong;
            RandomizePhase = randomizePhase;
            Easing = easing;

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
            new ModulatorOutput<float>("Phase"),
            new ModulatorOutput<int>("Cycles"),
        };

        public PrefabService PrefabService { get; set; }
        public Guid ID { get; set; }

        public GenericValue<float> Period { get; set; }
        public GenericValue<float> Minimum { get; set; }
        public GenericValue<float> Maximum { get; set; }
        public GenericValue<bool> PingPong { get; set; }
        public GenericValue<float> RandomizePhase { get; set; }
        public Easing Easing { get; set; }

        public IControlModel ToModel() => new LFOModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Period = (GenericValueModel<float>)Period.ToModel(),
            Minimum = (GenericValueModel<float>)Minimum.ToModel(),
            Maximum = (GenericValueModel<float>)Maximum.ToModel(),
            PingPong = (GenericValueModel<bool>)PingPong.ToModel(),
            RandomizePhase = (GenericValueModel<float>)RandomizePhase.ToModel(),
            Easing = (EasingModel)Easing.ToModel(),
        };
        public void FromModel(IControlModel model)
        {
            var m = (LFOModulatorModel)model;
            ID = m.ID;
            Period.FromModel(m.Period);
            Minimum.FromModel(m.Minimum);
            Maximum.FromModel(m.Maximum);
            PingPong.FromModel(m.PingPong);
            RandomizePhase.FromModel(m.RandomizePhase);
            Easing.FromModel(m.Easing);
        }
    }
}
