// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Texturing.Sources;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing
{
    public class TextureSourceSelector : ObservableRecipient, IControl
    {
        public TextureSourceSelector()
        {
            ProceduralName = new GenericValue<TextureSourceName>(TextureSourceName.Gradient);
            Gradient = new Gradient();
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<TextureSourceName> ProceduralName { get; set; }
        public Gradient Gradient { get; set; }

    }
}
