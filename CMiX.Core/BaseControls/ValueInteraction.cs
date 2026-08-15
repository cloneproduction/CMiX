// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.BaseControls
{
    // Ambient scope the Avalonia drag controls open for the duration of a pointer drag so a
    // gesture that writes a control value on every pointer move produces one undo step and a
    // throttled message stream instead of one of each per move. Hosts that never open a scope,
    // the WPF studio and the console, keep the unthrottled one step per write path untouched.
    // Single threaded by contract: only the UI thread opens a scope, writes inside it and closes it.
    public static class ValueInteraction
    {
        private static readonly List<IInteractiveValue> _participants = new();

        public static bool IsActive { get; private set; }

        public static void Begin()
        {
            // A gesture that never reached its release, a lost pointer capture for instance, must
            // not leave the previous scope open across the next one.
            End();
            IsActive = true;
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

            for (int i = 0; i < _participants.Count; i++)
                _participants[i].EndInteraction();

            _participants.Clear();
        }
    }
}
