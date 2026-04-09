// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core
{
    public class ControlActivationService
    {
        private readonly List<ReceivableControl> _controls = new();

        public void Register(ReceivableControl control)
        {
            if (control.CanRegister)
                _controls.Add(control);
        }

        public void ActivateAll()
        {
            foreach (var control in _controls)
            {
                if (!control.IsReceiving)
                    control.Activate();
            }
            _controls.Clear();
        }

        public void Clear() => _controls.Clear();
    }
}
