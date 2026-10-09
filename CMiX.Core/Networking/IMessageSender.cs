// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Networking
{
    public interface IMessageSender
    {
        void SendMessage(IMessage message);
    }
}
