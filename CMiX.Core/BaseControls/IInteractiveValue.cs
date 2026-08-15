// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.BaseControls
{
    // Implemented by controls that coalesce their undo step and throttle their outgoing messages
    // while a ValueInteraction scope is open. EndInteraction must always flush the final value.
    public interface IInteractiveValue
    {
        void EndInteraction();
    }
}
