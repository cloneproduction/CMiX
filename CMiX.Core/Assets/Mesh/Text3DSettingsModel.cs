// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
