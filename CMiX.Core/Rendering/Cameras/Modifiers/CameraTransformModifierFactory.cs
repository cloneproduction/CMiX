// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Networking;

namespace CMiX.Core.Rendering.Cameras.Modifiers
{
    public class CameraTransformModifierFactory : IModifierFactory
    {
        public CameraTransformModifierFactory()
        {

        }

        public IModifier Create(Type modifierType)
        {
            if (modifierType == typeof(CameraLFO))
                return ControlMessenger.Mapper.Map<CameraLFO>(new CameraLFOModel());

            if (modifierType == typeof(CameraRandom))
                return ControlMessenger.Mapper.Map<CameraRandom>(new CameraRandomModel());

            return null;
        }

        public IModifier Create(IModifierModel modifierModel)
        {
            if (modifierModel is CameraLFOModel cameraLFOModel)
                return ControlMessenger.Mapper.Map<CameraLFO>(cameraLFOModel);

            if (modifierModel is CameraRandomModel cameraRandomModel)
                return ControlMessenger.Mapper.Map<CameraRandom>(cameraRandomModel);

            return null;
        }

        public IModifierModel CreateModel(IModifier modifier)
        {
            var type = modifier.GetType();

            if (type == typeof(CameraLFO))
                return ControlMessenger.Mapper.Map<CameraLFOModel>(modifier);

            if (type == typeof(CameraRandom))
                return ControlMessenger.Mapper.Map<CameraRandomModel>(modifier);

            return null;
        }
    }
}
