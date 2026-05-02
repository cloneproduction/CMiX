// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Sources
{
    public partial class TouchBlob : ObservableObject, ITextureSource
    {
        public TouchBlob(PrefabService prefabService,
                         PrefabManager filterManager,
                         Integer2 resolution,
                         GenericValue<float> size,
                         GenericValue<string> color,
                         GenericValue<string> background)
        {
            PrefabService = prefabService;
            Resolution = resolution;
            Size = size;
            FilterManager = filterManager;
            Color = color;
            Background = background;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2 Resolution { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Size { get; set; }
        public PrefabManager FilterManager { get; set; }
        public GenericValue<string> Color { get; set; }
        public GenericValue<string> Background { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new TouchBlobModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            FilterManager = (PrefabManagerModel)FilterManager.ToModel(),
            Resolution = (Integer2Model)Resolution.ToModel(),
            Size = (GenericValueModel<float>)Size.ToModel(),
            Color = (GenericValueModel<string>)Color.ToModel(),
            Background = (GenericValueModel<string>)Background.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (TouchBlobModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Resolution.FromModel(m.Resolution);
            Size.FromModel(m.Size);
            Color.FromModel(m.Color);
            Background.FromModel(m.Background);

            LoadManager(FilterManager, m.FilterManager);
        }
    }
}
