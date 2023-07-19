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
            typePairs.Add(typeof(CameraLFO), typeof(CameraLFOModel));
            typePairs.Add(typeof(CameraRandom), typeof(CameraRandomModel));
        }

        private Dictionary<Type, Type> typePairs = new Dictionary<Type, Type>();

        public IModifier Create(Type modifierType)
        {
            return (IModifier)Activator.CreateInstance(modifierType);
        }
        public IModifier Create(IModifierModel modifierModel)
        {
            Type viewModelType = typePairs.FirstOrDefault(x => x.Value == modifierModel.GetType()).Key;
            return (IModifier)ControlMessenger.Mapper.Map(modifierModel, modifierModel.GetType(), viewModelType);
        }

        public IModifierModel CreateModel(IModifier modifier)
        {
            Type modelType = typePairs.GetValueOrDefault(modifier.GetType());
            return (IModifierModel)ControlMessenger.Mapper.Map(modifier, modifier.GetType(), modelType);
        }
    }
}
