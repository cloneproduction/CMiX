// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Sources
{
    public partial class Gradient : ObservableObject, ITextureSource
    {
        public Gradient(PrefabService prefabService,
                        PrefabManager filterManager,
                        Integer2 resolution, 
                        GenericValue<string> from, 
                        GenericValue<string> to, 
                        GenericValue<float> gamma, 
                        GenericValue<bool> horizontal)
        {
            PrefabService = prefabService; 
            Resolution = resolution;
            From = from;
            To = to;
            Gamma = gamma;
            Horizontal = horizontal;
            FilterManager = filterManager;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2 Resolution { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<string> From { get; set; }
        public GenericValue<string> To { get; set; }
        public GenericValue<float> Gamma { get; set; }
        public GenericValue<bool> Horizontal { get; set; }
        public PrefabManager FilterManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new GradientModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            FilterManager = (PrefabManagerModel)FilterManager.ToModel(),
            Resolution = (Integer2Model)Resolution.ToModel(),
            Gamma = (GenericValueModel<float>)Gamma.ToModel(),
            From = (GenericValueModel<string>)From.ToModel(),
            To = (GenericValueModel<string>)To.ToModel(),
            Horizontal = (GenericValueModel<bool>)Horizontal.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (GradientModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Resolution.FromModel(m.Resolution);
            Gamma.FromModel(m.Gamma);
            From.FromModel(m.From);
            To.FromModel(m.To);
            Horizontal.FromModel(m.Horizontal);

            LoadManager(FilterManager, m.FilterManager);
        }
    }
}
