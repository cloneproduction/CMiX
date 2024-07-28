// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Assets.Mesh
{
    public class Text3DSettingsModel : IControl, IPrefab
    {
        public Text3DSettingsModel()
        {
            Text = new GenericValueModel<string>("CMiX");
            FontSize = new GenericValueModel<int>(10);
            ExtrudeAmount = new GenericValueModel<float>(4.0f);
            FontFamily = new GenericValueModel<string>("Arial");
            HorizontalAlignment = new GenericValueModel<HorizontalAlignment>(Mesh.HorizontalAlignment.Center);
            ParagraphAlignment = new GenericValueModel<ParagraphAlignment>(Mesh.ParagraphAlignment.Center);
        }

        public PrefabService PrefabService { get; set; }
        public Guid ID { get; set; }

        public GenericValueModel<string> Text { get; set; }
        public GenericValueModel<int> FontSize { get; set; }
        public GenericValueModel<float> ExtrudeAmount { get; set; }
        public GenericValueModel<string> FontFamily { get; set; }
        public GenericValueModel<HorizontalAlignment> HorizontalAlignment { get; set; }
        public GenericValueModel<ParagraphAlignment> ParagraphAlignment { get; set; }
    }
}
