// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Undo;

namespace CMiX.Core.Modulation.Modulators
{
    public class LFOModulator : ModulatorBase
    {
        public LFOModulator(PrefabService prefabService,
                            GenericValue<float> period,
                            FloatRange range,
                            GenericValue<WaveTypeEnum> waveType,
                            Quantize quantize,
                            UndoManager undoManager,
                            ControlActivationService activationService)
            : base(prefabService)
        {
            Period = period;
            Range = range;
            WaveType = waveType;
            Quantize = quantize;

            UndoManager = undoManager;
            IsActive = false;
            activationService.Register(this);
        }

        public override IReadOnlyList<IModulatorOutput> Outputs { get; } = new IModulatorOutput[] {
            new ModulatorOutput<float>("Phase"),
            new ModulatorOutput<int>("Cycles"),
            new ModulatorOutput<bool>("Gate"),
        };

        public GenericValue<float> Period { get; set; }
        public FloatRange Range { get; set; }
        public GenericValue<WaveTypeEnum> WaveType { get; set; }
        public Quantize Quantize { get; set; }

        public override IControlModel ToModel() => new LFOModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Period = (GenericValueModel<float>)Period.ToModel(),
            Range = (FloatRangeModel)Range.ToModel(),
            WaveType = (GenericValueModel<WaveTypeEnum>)WaveType.ToModel(),
            Quantize = (QuantizeModel)Quantize.ToModel()
        };
        public override void FromModel(IControlModel model)
        {
            var m = (LFOModulatorModel)model;
            ID = m.ID;
            Period.FromModel(m.Period);
            Range.FromModel(m.Range);
            WaveType.FromModel(m.WaveType);
            Quantize.FromModel(m.Quantize);
        }
    }
}
