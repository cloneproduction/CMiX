// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Undo;

namespace CMiX.Core.Modulation.Modulators
{
    public class LFOModulator : ModulatorBase
    {
        public LFOModulator(PrefabService prefabService,
                            GenericValue<float> period,
                            GenericValue<float> minimum,
                            GenericValue<float> maximum,
                            GenericValue<WaveTypeEnum> waveType,
                            UndoManager undoManager,
                            ControlActivationService activationService)
            : base(prefabService)
        {
            Period = period;
            Minimum = minimum;
            Maximum = maximum;
            WaveType = waveType;

            UndoManager = undoManager;
            IsActive = false;
            activationService.Register(this);
        }

        public override IReadOnlyList<IModulatorOutput> Outputs { get; } = new IModulatorOutput[] {
            new ModulatorOutput<float>("Phase"),
            new ModulatorOutput<int>("Cycles"),
        };

        public GenericValue<float> Period { get; set; }
        public GenericValue<float> Minimum { get; set; }
        public GenericValue<float> Maximum { get; set; }
        public GenericValue<WaveTypeEnum> WaveType { get; set; }

        public override IControlModel ToModel() => new LFOModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Period = (GenericValueModel<float>)Period.ToModel(),
            Minimum = (GenericValueModel<float>)Minimum.ToModel(),
            Maximum = (GenericValueModel<float>)Maximum.ToModel(),
            WaveType = (GenericValueModel<WaveTypeEnum>)WaveType.ToModel() 
        };
        public override void FromModel(IControlModel model)
        {
            var m = (LFOModulatorModel)model;
            ID = m.ID;
            Period.FromModel(m.Period);
            Minimum.FromModel(m.Minimum);
            Maximum.FromModel(m.Maximum);
            WaveType.FromModel(m.WaveType);
        }
    }
}
