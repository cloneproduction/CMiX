// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Undo;

namespace CMiX.Core
{
    public class ControlActivationService
    {
        private readonly List<ReceivableControl> _controls = new();

        public void Register(ReceivableControl control)
        {
            if (!control.IsActive)
                _controls.Add(control);
        }

        public void ActivateAll()
        {
            _controls.ForEach(c => c.Activate());
            _controls.Clear();
        }

        public void Clear() => _controls.Clear();
    }
}
