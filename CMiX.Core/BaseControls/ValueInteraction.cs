// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.BaseControls
{
    // Ambient scope the Avalonia drag controls open for the duration of a pointer drag so a
    // gesture that writes a control value on every pointer move produces one undo step and a
    // throttled message stream instead of one of each per move. Hosts that never open a scope,
    // the WPF studio and the console, keep the unthrottled one step per write path untouched.
    // Single threaded by contract: only the UI thread opens a scope, writes inside it and closes it.
    // This is the one ambient static that neither clause of the rule in App.axaml.cs covers, so
    // it carries the ownership constraint instead: BeginScope hands back a handle and a scope
    // ends through that handle, which keeps one control from ending another control's gesture.
    public static class ValueInteraction
    {
        private static readonly List<IInteractiveValue> _participants = new();

        // Identifies the scope currently open. Zero is never handed out, so the default handle
        // of a control that never opened a scope can never match an open one.
        private static long _openScopeToken;
        private static long _lastToken;

        public static bool IsActive { get; private set; }

        // Opens a scope nobody owns, for a host that drives Begin and End as a pair. The drag
        // controls take BeginScope instead so the scope they opened is the only one they can end.
        public static void Begin()
        {
            BeginScope();
        }

        public static ValueInteractionScope BeginScope()
        {
            // A gesture that never reached its release, a lost pointer capture for instance, must
            // not leave the previous scope open across the next one.
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

        // Ends whatever scope is open, whoever opened it. Kept for hosts that drive the scope
        // without holding a handle and as the teardown path Begin itself uses; owners end their
        // own scope through the handle so a later gesture cannot be flushed by an earlier owner.
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
