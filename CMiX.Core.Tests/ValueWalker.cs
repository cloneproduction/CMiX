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

        public Type ValueType => _instance.GetType().GetGenericArguments()[0];

        public Guid ID => ((IControl)_instance).ID;

        public void Reset() => _instance.GetType().GetMethod("Reset", Type.EmptyTypes)!.Invoke(_instance, null);

        // Sets a value that differs from the current one. The number n keeps two values of one control apart.
        // A value that cannot change throws, so a theory never passes on a value it did not test.
        public void Change(int n)
        {
            var before = Value;
            Value = before switch
            {
                float f => f + 0.37f + n * 0.001f,
                int i => i + 3,
                bool b => !b,
                string s => s + "x",
                Guid => new Guid(n + 1, 0, 0, new byte[8]),
                Enum e => NextValue(e),
                null when ValueType == typeof(string) => "x",
                null when ValueType == typeof(Guid?) => new Guid(n + 1, 0, 0, new byte[8]),
                _ => throw new InvalidOperationException($"ValueWalker cannot change a value of type {ValueType.Name}.")
            };

            // A float at its maximum is clamped back, so it goes the other way.
            if (before is float && Equals(Value, before))
                Value = (float)before - 0.37f - n * 0.001f;

            if (Equals(Value, before))
                throw new InvalidOperationException($"ValueWalker could not change a {ValueType.Name} value of {before}.");
        }

        private static object NextValue(Enum value)
        {
            var values = Enum.GetValues(value.GetType());
            return values.GetValue((Array.IndexOf(values, value) + 1) % values.Length);
        }

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
                Walk(t.GetProperty("ModulatorIDSource")!.GetValue(o), path + ".ModulatorID", seen, depth + 1, found);
                Walk(t.GetProperty("BoundOutputNameSource")!.GetValue(o), path + ".BoundOutputName", seen, depth + 1, found);
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
                // A control points to these objects of the project, but does not own their values.
                if (p.Name is "Project" or "MasterBeat" or "SelectedOutputMapping") continue;
                object value;
                try { value = p.GetValue(o); } catch { continue; }
                if (value == null || value is string || value.GetType().IsPrimitive || value.GetType().IsEnum || value is Guid) continue;
                Walk(value, $"{path}.{p.Name}", seen, depth + 1, found);
            }
        }
    }
}
