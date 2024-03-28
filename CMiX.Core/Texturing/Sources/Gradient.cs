// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class Gradient : ObservableObject, ITextureSource, IPrefab
    {
        public Gradient(PrefabService prefabService,
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
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2 Resolution { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<string> From { get; set; }
        public GenericValue<string> To { get; set; }
        public GenericValue<float> Gamma { get; set; }
        public GenericValue<bool> Horizontal { get; set; }
    }
}
