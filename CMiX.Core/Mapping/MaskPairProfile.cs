// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Texturing;

namespace CMiX.Core.Mapping
{
    public class MaskPairProfile : ControlPairProfile
    {
        public MaskPairProfile()
        {
            CreatePair<MaskTexture, MaskModel>();
        }
    }
}
