// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;
using CMiX.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Sources
{
    public partial class CheckerBoard : TextureSourceBase, ITextureSource
    {
        public CheckerBoard(PrefabService prefabService,
                            PrefabManager textureModifierManager,
                            Integer2 resolution,
                            GenericValue<bool> useCompositionResolution,
                            Vector2 cellCount,
                            GenericValue<string> colorA,
                            GenericValue<string> colorB,
                            Transform2D transform2D)
            : base(prefabService, textureModifierManager, useCompositionResolution)
        {
            Resolution = resolution;
            ColorA = colorA;
            ColorB = colorB;
            CellCount = cellCount;
            Transform2D = transform2D;
        }

        public Integer2 Resolution { get; set; }
        public Transform2D Transform2D { get; set; }
        public Vector2 CellCount { get; set; }
        public GenericValue<string> ColorA { get; set; }
        public GenericValue<string> ColorB { get; set; }

        public override IControlModel ToModel()
        {
            var model = new CheckerBoardModel
            {
                Resolution = (Integer2Model)Resolution.ToModel(),
                Transform2D = (Transform2DModel)Transform2D.ToModel(),
                CellCount = (Vector2Model)CellCount.ToModel(),
                ColorA = (GenericValueModel<string>)ColorA.ToModel(),
                ColorB = (GenericValueModel<string>)ColorB.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (CheckerBoardModel)model;
            LoadBaseModel(m);
            Resolution.FromModel(m.Resolution);
            Transform2D.FromModel(m.Transform2D);
            CellCount.FromModel(m.CellCount);
            ColorA.FromModel(m.ColorA);
            ColorB.FromModel(m.ColorB);
        }
    }
}
