// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Modulation.Modulators
{
    public class WatchesModulator : ModulatorBase
    {
        public WatchesModulator(PrefabService prefabService, 
                                 GenericValue<int> count, 
                                 GenericValue<int> heartrate,
                                 GenericValue<float> temperature,
                                 GenericValue<float> conductivity)
            : base(prefabService)
        {
            Count = count;
            HeartRate = heartrate;
            Temperature = temperature;
            Conductivity = conductivity;
        }

        public override IReadOnlyList<IModulatorOutput> Outputs { get; } = new IModulatorOutput[]
        {
            new ModulatorOutput<int>("Count"),
            new ModulatorOutput<int>("HeartRate"),
            new ModulatorOutput<float>("Temperature"),
            new ModulatorOutput<float>("Conductivity"),
        };

        public GenericValue<int> Count { get; set; }
        public GenericValue<int> HeartRate { get; set; }
        public GenericValue<float> Temperature { get; set; }
        public GenericValue<float> Conductivity { get; set; }

        public override IControlModel ToModel() => new WatchesModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Count = (GenericValueModel<int>)Count.ToModel(),
            HeartRate = (GenericValueModel<int>)HeartRate.ToModel(),
            Temperature = (GenericValueModel<float>)Temperature.ToModel(),
            Conductivity = (GenericValueModel<float>)Conductivity.ToModel()
        };

        public override void FromModel(IControlModel model)
        {
            var m = (WatchesModulatorModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Count.FromModel(m.Count);
            HeartRate.FromModel(m.HeartRate);
            Temperature.FromModel(m.Temperature);
            Conductivity.FromModel(m.Conductivity);
        }
    }
}
