// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace CMiX.Core.Networking
{
    // Reads generic credentials of the Windows Credential Manager.
    // The password blob is read as UTF-16, the form that the Control Panel and cmdkey write.
    public sealed class WindowsCredentialStore : ICredentialStore
    {
        private const uint CredTypeGeneric = 1;

        public bool TryRead(string name, out string user, out string password)
        {
            user = string.Empty;
            password = string.Empty;
            if (string.IsNullOrEmpty(name))
                return false;

            try
            {
                if (!CredRead(name, CredTypeGeneric, 0, out var handle))
                    return false;

                try
                {
                    var credential = Marshal.PtrToStructure<Credential>(handle);
                    user = Marshal.PtrToStringUni(credential.UserName) ?? string.Empty;
                    if (credential.CredentialBlobSize > 0 && credential.CredentialBlob != IntPtr.Zero)
                        password = Marshal.PtrToStringUni(credential.CredentialBlob, (int)(credential.CredentialBlobSize / 2));
                    return true;
                }
                finally
                {
                    CredFree(handle);
                }
            }
            catch (Exception)
            {
                user = string.Empty;
                password = string.Empty;
                return false;
            }
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        private static extern bool CredRead(string target, uint type, uint flags, out IntPtr credential);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        private static extern void CredFree(IntPtr buffer);

        // Mirrors the native CREDENTIAL struct.
        [StructLayout(LayoutKind.Sequential)]
        private struct Credential
        {
            public uint Flags;
            public uint Type;
            public IntPtr TargetName;
            public IntPtr Comment;
            public FILETIME LastWritten;
            public uint CredentialBlobSize;
            public IntPtr CredentialBlob;
            public uint Persist;
            public uint AttributeCount;
            public IntPtr Attributes;
            public IntPtr TargetAlias;
            public IntPtr UserName;
        }
    }
}
