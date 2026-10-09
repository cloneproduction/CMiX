// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.BaseControls
{
    public static class ValueInteraction
    {
        private static readonly List<IInteractiveValue> _participants = new();
        private static long _openScopeToken;
        private static long _lastToken;

        public static bool IsActive { get; private set; }

        public static void Begin()
        {
            BeginScope();
        }

        public static ValueInteractionScope BeginScope()
        {
            End();
            _openScopeToken = ++_lastToken;
            IsActive = true;
            return new ValueInteractionScope(_openScopeToken);
        }

        public static void Enlist(IInteractiveValue participant)
        {
            if (!IsActive || participant == null) return;
            if (!_participants.Contains(participant))
                _participants.Add(participant);
        }

        public static void End()
        {
            if (!IsActive) return;
            IsActive = false;
            _openScopeToken = 0;

            for (int i = 0; i < _participants.Count; i++)
                _participants[i].EndInteraction();

            _participants.Clear();
        }

        internal static void End(long token)
        {
            if (token == 0 || token != _openScopeToken) return;
            End();
        }
    }
}
