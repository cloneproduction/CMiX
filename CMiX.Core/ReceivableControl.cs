// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core
{
    public abstract class ReceivableControl : ObservableRecipient
    {
        internal bool IsReceiving { get; private set; }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate() => IsActive = false;

        internal bool CanRegister => !IsReceiving;

        protected void ReceiveWithoutEcho(Action action)
        {
            IsReceiving = true;
            IsActive = false;
            action();
            IsActive = true;
            IsReceiving = false;
        }
    }
}
