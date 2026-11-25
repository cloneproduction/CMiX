// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Modulation.Modulators
{
    // Drives a value that grows by Offset each frame while Step is true. Reset sets it to ResetValue.
    // The engine holds the accumulator. Core holds the parameters.
    public class IntegratorModulator : ModulatorBase
    {
        public IntegratorModulator(PrefabService prefabService,
                                   GenericValue<float> offset,
                                   GenericValue<bool> step,
                                   GenericValue<float> resetValue,
                                   CMiXButton reset,
                                   ControlRegistry registry)
            : base(prefabService)
        {
            Offset = offset;
            Step = step;
            ResetValue = resetValue;
            Reset = reset;

            IsActive = false;
            registry.Register(this);
        }

        public override IReadOnlyList<IModulatorOutput> Outputs { get; } = new IModulatorOutput[] {
            new ModulatorOutput<float>("Value"),
        };

        public GenericValue<float> Offset { get; set; }
        public GenericValue<bool> Step { get; set; }
        public GenericValue<float> ResetValue { get; set; }
        public CMiXButton Reset { get; set; }

        public override IControlModel ToModel() => new IntegratorModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Offset = (GenericValueModel<float>)Offset.ToModel(),
            Step = (GenericValueModel<bool>)Step.ToModel(),
            ResetValue = (GenericValueModel<float>)ResetValue.ToModel(),
            Reset = (ButtonModel)Reset.ToModel(),
        };

        public override void FromModel(IControlModel model)
        {
            var m = (IntegratorModulatorModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Offset.FromModel(m.Offset);
            Step.FromModel(m.Step);
            ResetValue.FromModel(m.ResetValue);
            Reset.FromModel(m.Reset);
        }
    }
}
