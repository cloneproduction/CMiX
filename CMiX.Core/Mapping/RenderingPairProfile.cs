// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Rendering;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Mapping
{
    public class RenderingPairProfile : ControlPairProfile
    {
        public RenderingPairProfile()
        {
            CreatePair<OutputSettings, OutputSettingsModel>();
            CreatePair<AmbientOcclusion, AmbientOcclusionModel>();
        }
    }
}
