// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing
{
    public class LayerMaskService
    {
        public LayerMaskService() 
        {
            IsMask = new BooleanValue(false);
            MaskChannel = new GenericValue<MaskChannel>();
            MaskMode = new GenericValue<MaskMode>();
            Invert = new BooleanValue(false);
        }

        public BooleanValue Invert { get; set; }
        public BooleanValue IsMask { get; set; }
        public GenericValue<MaskMode> MaskMode { get; set; }
        public GenericValue<MaskChannel> MaskChannel { get; set; }
    }
}
