// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Security.Cryptography;
using System.Text;

namespace CMiX.Core
{
    // Gives a model field that no control owns an id that stays the same on every ToModel call.
    // The id comes from the owner id and the field name, so each owner has its own ids.
    public static class DerivedId
    {
        public static Guid Of(Guid owner, string name)
        {
            var bytes = owner.ToByteArray().Concat(Encoding.UTF8.GetBytes(name)).ToArray();
            return new Guid(SHA256.HashData(bytes)[..16]);
        }
    }
}
