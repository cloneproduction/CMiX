// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
