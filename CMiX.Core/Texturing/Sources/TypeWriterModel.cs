// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    public class TypeWriterModel : IControlModel, IPrefabModel
    {
        public TypeWriterModel()
        {
            ID = Guid.NewGuid();

            StringControl = new GenericValueModel<string>("CMiX");
            FontFamily = new GenericValueModel<string>("Arial");
            FontSize = new GenericValueModel<float>(0.45f);
            FontColor = new GenericValueModel<string>("#ff000000");
            BackgroundColor = new GenericValueModel<string>("#00000000");
            Resolution = new Integer2Model(1024, 1024);
            Position = new Vector2Model();
            Style = new GenericValueModel<FontStyle>(FontStyle.Normal);

            FilterManager = new PrefabManagerModel();
            PrefabService = new PrefabServiceModel();
        }

        public Guid ID { get; set; }
        public GenericValueModel<string> StringControl { get; set; }
        public GenericValueModel<string> FontColor { get; set; }
        public GenericValueModel<string> BackgroundColor { get; set; }
        public Integer2Model Resolution { get; set; }
        public Vector2Model Position { get; set; }
        public GenericValueModel<float> FontSize { get; set; }
        public GenericValueModel<string> FontFamily { get; set; }
        public GenericValueModel<FontStyle> Style { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public PrefabManagerModel FilterManager { get; set; }
    }
}
