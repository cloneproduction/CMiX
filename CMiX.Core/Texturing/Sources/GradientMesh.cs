// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    public partial class GradientMesh : TextureSourceBase, ITextureSource
    {
        public GradientMesh(PrefabService prefabService,
                            PrefabManager textureModifierManager,
                            ModulatorHost modulators,
                            Integer2 resolution,
                            GenericValue<bool> useCompositionResolution,
                            ModulatableValue<float> time,
                            ModulatableValue<float> rotationSpeed,
                            GenericValue<string> color1,
                            GenericValue<string> color2,
                            GenericValue<string> color3,
                            GenericValue<string> color4,
                            GenericValue<GradientMeshType> gradientType)
            : base(prefabService, textureModifierManager, modulators, useCompositionResolution)
        {
            Resolution = resolution;
            Time = time;
            RotationSpeed = rotationSpeed;
            Color1 = color1;
            Color2 = color2;
            Color3 = color3;
            Color4 = color4;
            GradientType = gradientType;

            Modulators.Add(Time, RotationSpeed);
        }

        public Integer2 Resolution { get; set; }
        public ModulatableValue<float> Time { get; set; }
        public ModulatableValue<float> RotationSpeed { get; set; }
        public GenericValue<string> Color1 { get; set; }
        public GenericValue<string> Color2 { get; set; }
        public GenericValue<string> Color3 { get; set; }
        public GenericValue<string> Color4 { get; set; }
        public GenericValue<GradientMeshType> GradientType { get; set; }

        public override IControlModel ToModel()
        {
            var model = new GradientMeshModel
            {
                Resolution = (Integer2Model)Resolution.ToModel(),
                Time = (ModulatableValueModel<float>)Time.ToModel(),
                RotationSpeed = (ModulatableValueModel<float>)RotationSpeed.ToModel(),
                Color1 = (GenericValueModel<string>)Color1.ToModel(),
                Color2 = (GenericValueModel<string>)Color2.ToModel(),
                Color3 = (GenericValueModel<string>)Color3.ToModel(),
                Color4 = (GenericValueModel<string>)Color4.ToModel(),
                GradientType = (GenericValueModel<GradientMeshType>)GradientType.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (GradientMeshModel)model;
            LoadBaseModel(m);
            Resolution.FromModel(m.Resolution);
            Time.FromModel(m.Time);
            RotationSpeed.FromModel(m.RotationSpeed);
            Color1.FromModel(m.Color1);
            Color2.FromModel(m.Color2);
            Color3.FromModel(m.Color3);
            Color4.FromModel(m.Color4);
            GradientType.FromModel(m.GradientType);
        }
    }
}
