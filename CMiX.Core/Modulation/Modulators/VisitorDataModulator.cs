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
                                 GenericValue<float> blueiotposY,
                                 FloatRange heartRateRange,
                                 FloatRange temperatureRange,
                                 FloatRange conductivityRange,
                                 FloatRange etempRange,
                                 FloatRange blueiotposXRange,
                                 FloatRange blueiotposYRange)
            : base(prefabService)
        {
            Count = count;
            HeartRate = heartrate;
            Temperature = temperature;
            Conductivity = conductivity;
            ETemp = etemp;
            BlueIOTPosX = blueiotposX;
            BlueIOTPosY = blueiotposY;
            HeartRateRange = heartRateRange;
            TemperatureRange = temperatureRange;
            ConductivityRange = conductivityRange;
            ETempRange = etempRange;
            BlueIOTPosXRange = blueiotposXRange;
            BlueIOTPosYRange = blueiotposYRange;
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
        public FloatRange HeartRateRange { get; set; }
        public FloatRange TemperatureRange { get; set; }
        public FloatRange ConductivityRange { get; set; }
        public FloatRange ETempRange { get; set; }
        public FloatRange BlueIOTPosXRange { get; set; }
        public FloatRange BlueIOTPosYRange { get; set; }

        public override IControlModel ToModel() => new VisitorDataModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Count = (GenericValueModel<int>)Count.ToModel(),
            HeartRate = (GenericValueModel<float>)HeartRate.ToModel(),
            Temperature = (GenericValueModel<float>)Temperature.ToModel(),
            Conductivity = (GenericValueModel<float>)Conductivity.ToModel(),
            ETemp = (GenericValueModel<float>)ETemp.ToModel(),
            BlueIOTPosX = (GenericValueModel<float>)BlueIOTPosX.ToModel(),
            BlueIOTPosY = (GenericValueModel<float>)BlueIOTPosY.ToModel(),
            HeartRateRange = (FloatRangeModel)HeartRateRange.ToModel(),
            TemperatureRange = (FloatRangeModel)TemperatureRange.ToModel(),
            ConductivityRange = (FloatRangeModel)ConductivityRange.ToModel(),
            ETempRange = (FloatRangeModel)ETempRange.ToModel(),
            BlueIOTPosXRange = (FloatRangeModel)BlueIOTPosXRange.ToModel(),
            BlueIOTPosYRange = (FloatRangeModel)BlueIOTPosYRange.ToModel()
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
            ETemp.FromModel(m.ETemp);
            BlueIOTPosX.FromModel(m.BlueIOTPosX);
            BlueIOTPosY.FromModel(m.BlueIOTPosY);
            HeartRateRange.FromModel(m.HeartRateRange);
            TemperatureRange.FromModel(m.TemperatureRange);
            ConductivityRange.FromModel(m.ConductivityRange);
            ETempRange.FromModel(m.ETempRange);
            BlueIOTPosXRange.FromModel(m.BlueIOTPosXRange);
            BlueIOTPosYRange.FromModel(m.BlueIOTPosYRange);
        }
    }
}
