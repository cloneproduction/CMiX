// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Modulation.Modulators
{
    public class TrackingModulator : ModulatorBase
    {
        public TrackingModulator(PrefabService prefabService, 
                                 GenericValue<int> count, 
                                 GenericValue<float> x,
                                 GenericValue<float> y)
            : base(prefabService)
        {
            Count = count;
            X = x;
            Y = y;
        }

        public override IReadOnlyList<IModulatorOutput> Outputs { get; } = new IModulatorOutput[]
        {
            new ModulatorOutput<int>("Count"),
            new ModulatorOutput<float>("X"),
            new ModulatorOutput<float>("Y")
        };

        public GenericValue<int> Count { get; set; }
        public GenericValue<float> X { get; set; }
        public GenericValue<float> Y { get; set; }

        public override IControlModel ToModel() => new TrackingModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Count = (GenericValueModel<int>)Count.ToModel(),
            X = (GenericValueModel<float>)X.ToModel(),
            Y = (GenericValueModel<float>)Y.ToModel()
        };

        public override void FromModel(IControlModel model)
        {
            var m = (TrackingModulatorModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Count.FromModel(m.Count);
            X.FromModel(m.X);
            Y.FromModel(m.Y);
        }
    }
}
