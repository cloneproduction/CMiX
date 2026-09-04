// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Networking
{
    // What a peer must do after connect when it does not join automatically.
    public enum StartCheck
    {
        // The store is empty. The local state becomes the store state, without a question.
        PushSilently,

        // The store holds the local state. The peer only follows the stream.
        AlreadyInSync,

        // The states differ. The user must push or pull.
        NotInSync
    }
}
