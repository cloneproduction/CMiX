using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit;

namespace CMiX.UI.Tests
{
    // A test method is Method_Scenario_Result. This test finds the names that break the rule.
    public class TestNameTests
    {
        // The tests from before 2026-11-01 keep their names. New tests follow the rule, so this number only goes down.
        private const int PinnedBadNameCount = 152;

        private static readonly Regex NameRule = new Regex("^[A-Z][A-Za-z0-9]*_[A-Z][A-Za-z0-9]*_[A-Z][A-Za-z0-9]*$");

        private static readonly HashSet<string> TestAttributeNames = new HashSet<string>
        {
            "FactAttribute", "TheoryAttribute",
            "AvaloniaFactAttribute", "AvaloniaTheoryAttribute",
            "WindowsFactAttribute", "WindowsTheoryAttribute",
        };

        private static bool IsTestAttribute(Type type)
        {
            for (var t = type; t != null; t = t.BaseType)
            {
                if (TestAttributeNames.Contains(t.Name))
                    return true;
            }
            return false;
        }

        [Fact]
        public void Scan_TheTestAssembly_FindsNoMoreBadNamesThanPinned()
        {
            var badNames = typeof(TestNameTests).Assembly.GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
                .Where(m => m.GetCustomAttributes(false).Any(a => IsTestAttribute(a.GetType())))
                .Where(m => !NameRule.IsMatch(m.Name))
                .Select(m => m.DeclaringType?.Name + "." + m.Name)
                .OrderBy(n => n, StringComparer.Ordinal)
                .ToList();

            Assert.True(badNames.Count <= PinnedBadNameCount,
                $"{badNames.Count} test names break Method_Scenario_Result, the pinned number is {PinnedBadNameCount}:"
                + Environment.NewLine + string.Join(Environment.NewLine, badNames));
        }
    }
}
