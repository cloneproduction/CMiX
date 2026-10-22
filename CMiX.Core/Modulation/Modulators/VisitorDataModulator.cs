// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Modulation.Modulators
{
    public class VisitorDataModulator : ModulatorBase
    {
        public VisitorDataModulator(PrefabService prefabService, 
                                 GenericValue<int> count, 
                                 GenericValue<float> heartrate,
                                 GenericValue<float> temperature,
                                 GenericValue<float> conductivity,
                                 GenericValue<float> etemp,
                                 GenericValue<float> blueiotposX,
                                 GenericValue<float> blueiotposY)
            : base(prefabService)
        {
            Count = count;
            HeartRate = heartrate;
            Temperature = temperature;
            Conductivity = conductivity;
            ETemp = etemp;
            BlueIOTPosX = blueiotposX;
            BlueIOTPosY = blueiotposY;
        }

        public override IReadOnlyList<IModulatorOutput> Outputs { get; } = new IModulatorOutput[]
        {
            new ModulatorOutput<int>("Count"),
            new ModulatorOutput<float>("HeartRate"),
            new ModulatorOutput<float>("Temperature"),
            new ModulatorOutput<float>("Conductivity"),
        };

        public GenericValue<int> Count { get; set; }
        public GenericValue<float> HeartRate { get; set; }
        public GenericValue<float> Temperature { get; set; }
        public GenericValue<float> Conductivity { get; set; }
        public GenericValue<float> ETemp { get; set; }
        public GenericValue<float> BlueIOTPosX { get; set; }
        public GenericValue<float> BlueIOTPosY { get; set; }

        public override IControlModel ToModel() => new VisitorDataModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Count = (GenericValueModel<int>)Count.ToModel(),
            HeartRate = (GenericValueModel<float>)HeartRate.ToModel(),
            Temperature = (GenericValueModel<float>)Temperature.ToModel(),
            Conductivity = (GenericValueModel<float>)Conductivity.ToModel(),
            ETemp = (GenericValueModel<float>)Conductivity.ToModel(),
            BlueIOTPosX = (GenericValueModel<float>)Conductivity.ToModel(),
            BlueIOTPosY = (GenericValueModel<float>)Conductivity.ToModel()
        };

        public override void FromModel(IControlModel model)
        {
            var m = (VisitorDataModulatorModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Count.FromModel(m.Count);
            HeartRate.FromModel(m.HeartRate);
            Temperature.FromModel(m.Temperature);
            Conductivity.FromModel(m.Conductivity);
            ETemp.FromModel(m.Conductivity);
            BlueIOTPosX.FromModel(m.Conductivity);
            BlueIOTPosY.FromModel(m.Conductivity);
        }
    }
}
