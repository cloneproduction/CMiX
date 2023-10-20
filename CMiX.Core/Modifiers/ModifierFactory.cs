// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking;

namespace CMiX.Core.Modifiers
{
    public abstract class ModifierFactory : IModifierFactory
    {

        public Dictionary<Type, Type> TypePairs = new Dictionary<Type, Type>();

        public IModifier Create(Type modifierType)
        {
            return (IModifier)Activator.CreateInstance(modifierType);
        }
        public IModifier Create(IModifierModel modifierModel)
        {
            Type viewModelType = TypePairs.FirstOrDefault(x => x.Value == modifierModel.GetType()).Key;
            return (IModifier)ControlMessenger.Mapper.Map(modifierModel, modifierModel.GetType(), viewModelType);
        }

        public IModifierModel CreateModel(IModifier modifier)
        {
            Type modelType = TypePairs.GetValueOrDefault(modifier.GetType());
            return (IModifierModel)ControlMessenger.Mapper.Map(modifier, modifier.GetType(), modelType);
        }
    }
}
