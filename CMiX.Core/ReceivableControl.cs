// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core
{
    public abstract class ReceivableControl : ObservableRecipient
    {
        public UndoManager UndoManager { get; set; }
        public bool IsReceiving { get; set; }
        public void Activate() => IsActive = true;

        // Called by ControlFactory after a default model loads. Most controls have no default.
        public virtual void SetDefaults() { }

        protected void ReceiveWithoutEcho(Action action)
        {
            IsReceiving = true;
            IsActive = false;
            try
            {
                action();
            }
            finally
            {
                IsActive = true;
                IsReceiving = false;
            }
        }
    }
}
