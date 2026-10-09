// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Reflection;

namespace CMiX_ENGINEUtils
{
    /// <summary>
    /// Registers an object as a service on the given registry, under the given interface type.
    /// </summary>
    [ProcessNode]
    public class RegisterServiceAs
    {
        private static readonly MethodInfo RegisterExisting = typeof(ServiceRegistry)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .First(mi => mi.Name == "RegisterService"
                && mi.IsGenericMethodDefinition
                && mi.GetGenericArguments().Length == 1
                && mi.GetParameters().Length == 1
                && mi.GetParameters()[0].ParameterType == mi.GetGenericArguments()[0]);

        private bool _done;

        public bool Update(ServiceRegistry registry, Type interfaceType, object obj, bool register, out string error)
        {
            error = string.Empty;
            if (!register || _done)
                return false;

            if (registry == null) { error = "registry is null"; return false; }
            if (interfaceType == null) { error = "interfaceType is null"; return false; }

            RegisterExisting.MakeGenericMethod(interfaceType).Invoke(registry, new object[] { obj });

            _done = true;
            return true;
        }
    }
}
