// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing
{
    public class LayerMaskSettings : IControl
    {
        public LayerMaskSettings(GenericValue<bool> isMask, 
                                GenericValue<MaskChannel> maskChannel, 
                                GenericValue<MaskMode> maskMode, 
                                GenericValue<bool> invert) 
        {
            IsMask = isMask;
            MaskChannel = maskChannel;
            MaskMode = maskMode;
            Invert = invert;
        }

        public GenericValue<bool> Invert { get; set; }
        public GenericValue<bool> IsMask { get; set; }
        public GenericValue<MaskMode> MaskMode { get; set; }
        public GenericValue<MaskChannel> MaskChannel { get; set; }
        public Guid ID { get; set; }

        public void FromModel(IControlModel model)
        {
            throw new NotImplementedException();
        }

        public IControlModel ToModel()
        {
            throw new NotImplementedException();
        }
    }
}
