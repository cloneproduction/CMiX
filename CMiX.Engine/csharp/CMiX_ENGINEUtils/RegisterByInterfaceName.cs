// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CMiX_ENGINEUtils
{
    /// <summary>
    /// Registers an object as a service under an interface type resolved at runtime from a VL mangled type name.
    /// </summary>
    [ProcessNode]
    public class RegisterServiceByInterfaceName
    {
        private bool _done;

        /// <summary>
        /// Extracts the interface name from <paramref name="fullName"/>, finds the matching Type, and calls RegisterService with it.
        /// </summary>
        public bool Update(AppHost app, string fullName, object obj, bool register, out string error)
        {
            error = string.Empty;
            if (!register || _done)
                return false;

            if (app == null) { error = "app is null"; return false; }

            var m = Regex.Match(fullName ?? string.Empty, @"\.([^.]+)_I$");
            if (!m.Success) { error = "no interface name found in fullName"; return false; }
            string typeName = m.Groups[1].Value;

            Type? interfaceType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name == typeName);
            if (interfaceType == null) { error = $"no Type found named {typeName}"; return false; }

            MethodInfo? generic = app.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(mi => mi.Name == "RegisterService"
                    && mi.IsGenericMethodDefinition
                    && mi.GetParameters().Length == 1
                    && mi.GetParameters()[0].ParameterType.IsGenericType
                    && mi.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>));
            if (generic == null) { error = "RegisterService method not found on app"; return false; }

            var sp = Expression.Parameter(typeof(IServiceProvider));
            var body = Expression.Convert(Expression.Constant(obj), interfaceType);
            var funcType = typeof(Func<,>).MakeGenericType(typeof(IServiceProvider), interfaceType);
            var factory = Expression.Lambda(funcType, body, sp).Compile();

            generic.MakeGenericMethod(interfaceType).Invoke(app, new object[] { factory });

            _done = true;
            return true;
        }
    }
}
