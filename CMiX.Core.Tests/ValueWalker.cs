using System.Collections;
using System.Reflection;
using CMiX.Core.BaseControls;

namespace CMiX.Core.Tests
{
    // A value found in a control graph, with the path that leads to it.
    internal sealed class WalkedValue
    {
        private readonly object _instance;

        public WalkedValue(object instance) => _instance = instance;

        public object Value
        {
            get => _instance.GetType().GetProperty("Value")!.GetValue(_instance);
            set => _instance.GetType().GetProperty("Value")!.SetValue(_instance, value);
        }
    }

    // Finds every GenericValue in a control, including those inside ModulatableValue, and the nested controls.
    internal static class ValueWalker
    {
        private static readonly string[] SkippedSuffixes = { "Manager", "Service", "Repository", "Messenger", "Factory", "Handler", "Data" };

        public static SortedDictionary<string, WalkedValue> Collect(object root)
        {
            var found = new SortedDictionary<string, WalkedValue>(StringComparer.Ordinal);
            Walk(root, root.GetType().Name, new HashSet<object>(ReferenceEqualityComparer.Instance), 0, found);
            return found;
        }

        private static void Walk(object o, string path, HashSet<object> seen, int depth, SortedDictionary<string, WalkedValue> found)
        {
            if (o == null || depth > 7 || !seen.Add(o)) return;
            var t = o.GetType();

            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(GenericValue<>))
            {
                found[path] = new WalkedValue(o);
                return;
            }

            if (t.IsGenericType && t.GetGenericTypeDefinition().Name.StartsWith("ModulatableValue"))
            {
                Walk(t.GetProperty("ValueSource")!.GetValue(o), path + ".Value", seen, depth + 1, found);
                return;
            }

            if (o is IEnumerable enumerable && o is not string)
            {
                var i = 0;
                foreach (var item in enumerable)
                    Walk(item, $"{path}[{i++}]", seen, depth + 1, found);
                return;
            }

            if (t.Namespace == null || !t.Namespace.StartsWith("CMiX.Core")) return;
            if (SkippedSuffixes.Any(s => t.Name.EndsWith(s))) return;

            foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (p.GetIndexParameters().Length > 0 || !p.CanRead) continue;
                if (p.Name is "PrefabService" or "ControlRepository" or "UndoManager" or "ControlMessenger" or "MessageFactory") continue;
                object value;
                try { value = p.GetValue(o); } catch { continue; }
                if (value == null || value is string || value.GetType().IsPrimitive || value.GetType().IsEnum || value is Guid) continue;
                Walk(value, $"{path}.{p.Name}", seen, depth + 1, found);
            }
        }
    }
}
