// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;

namespace CMiX.Core.Modifiers
{
    public class ModifierCreatedEventArgs : EventArgs
    {
        public ModifierCreatedEventArgs(IModifierModel modifierModel)
        {
            _modifierModel = modifierModel;
        }

        private IModifierModel _modifierModel;

        public IModifierModel ModifierModel
        {
            get { return _modifierModel; }
        }
    }
}
