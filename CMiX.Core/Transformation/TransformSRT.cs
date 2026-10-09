// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation
{
    public partial class TransformSRT : ObservableObject, IControl
    {
        public TransformSRT(GenericValue<float> uniform,
                            Vector3 scale,
                            Vector3 translate,
                            Vector3 rotation,
                            PrefabService prefabService)
        {
            Uniform = uniform;
            Scale = scale;
            Translate = translate;
            Rotation = rotation;
            isExpanded = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<float> Uniform { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Translate { get; set; }
        public Vector3 Rotation { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        public IControlModel ToModel() => new TransformSRTModel
        {
            ID = ID,
            Uniform = (GenericValueModel<float>)Uniform.ToModel(),
            Scale = (Vector3Model)Scale.ToModel(),
            Translate = (Vector3Model)Translate.ToModel(),
            Rotation = (Vector3Model)Rotation.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (TransformSRTModel)model;
            ID = m.ID;
            Uniform.FromModel(m.Uniform);
            Scale.FromModel(m.Scale);
            Translate.FromModel(m.Translate);
            Rotation.FromModel(m.Rotation);
        }
    }
}
