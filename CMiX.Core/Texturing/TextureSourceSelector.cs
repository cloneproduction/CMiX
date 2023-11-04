// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Texturing.Sources;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing
{
    public class TextureSourceSelector : ObservableRecipient, IControl
    {
        public TextureSourceSelector(GenericValue<TextureSourceName> proceduralName, Gradient gradient)
        {
            ProceduralName = proceduralName; // new GenericValue<TextureSourceName>(TextureSourceName.Gradient);
            Gradient = gradient;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<TextureSourceName> ProceduralName { get; set; }
        public Gradient Gradient { get; set; }

    }
}
