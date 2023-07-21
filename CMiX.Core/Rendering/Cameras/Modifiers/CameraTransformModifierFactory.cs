// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;

namespace CMiX.Core.Rendering.Cameras.Modifiers
{
    public class CameraTransformModifierFactory : ModifierFactory
    {
        public CameraTransformModifierFactory()
        {
            TypePairs.Add(typeof(CameraLFO), typeof(CameraLFOModel));
            TypePairs.Add(typeof(CameraRandom), typeof(CameraRandomModel));
        }
    }
}
