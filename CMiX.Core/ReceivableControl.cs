// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core
{
    public abstract class ReceivableControl : ObservableRecipient
    {
        public UndoManager UndoManager { get; set; }
        public void Activate() => IsActive = true;

        // Called by ControlFactory after a default model loads. Most controls have no default.
        public virtual void SetDefaults() { }

        private int _suppressionDepth;

        public bool IsSendingSuppressed => _suppressionDepth > 0;

        // Stops the network messages and the undo steps of this control until the scope ends.
        // Scopes can nest. Always use it in a using statement.
        public SuppressionScope SuppressSending()
        {
            _suppressionDepth++;
            return new SuppressionScope(this);
        }

        public readonly struct SuppressionScope : IDisposable
        {
            private readonly ReceivableControl _owner;

            internal SuppressionScope(ReceivableControl owner) => _owner = owner;

            public void Dispose() => _owner._suppressionDepth--;
        }
    }
}
