// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Assets.Mesh
{
    public record Text3DSettingsModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<string> Text { get; set; } = new("CMiX");
        public GenericValueModel<int> FontSize { get; set; } = new(10);
        public GenericValueModel<float> ExtrudeAmount { get; set; } = new(4.0f);
        public GenericValueModel<string> FontFamily { get; set; } = new("Arial");
        public GenericValueModel<HorizontalAlignment> HorizontalAlignment { get; set; } = new(Mesh.HorizontalAlignment.Center);
        public GenericValueModel<ParagraphAlignment> ParagraphAlignment { get; set; } = new(Mesh.ParagraphAlignment.Center);
    }
}
