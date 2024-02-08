// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Cameras.Modifiers;

namespace CMiX.Core.Mapping
{
    public class CameraPairProfile : ControlPairProfile
    {
        public CameraPairProfile()
        {
            CreatePair<Camera, CameraModel>();
            CreatePair<CameraLFO, CameraLFOModel>();
            CreatePair<CameraRandom, CameraRandomModel>();
        }
    }
}
