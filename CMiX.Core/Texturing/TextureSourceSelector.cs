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
            ID = Guid.NewGuid();
            ProceduralName = new GenericValue<TextureSourceName>();
            Gradient = new Gradient();
            BubbleNoise = new BubbleNoise();
            Image = new Image();
        }

        public Guid ID { get; set; }
        public GenericValue<TextureSourceName> ProceduralName { get; set; }
        public Gradient Gradient { get; set; }
        public BubbleNoise BubbleNoise { get; set; }
        public Image Image { get; set; }
    }
}
