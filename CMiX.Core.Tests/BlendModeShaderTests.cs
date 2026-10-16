using System.Text.RegularExpressions;
using CMiX.Core.Texturing;
using Xunit;

namespace CMiX.Core.Tests
{
    public class BlendModeShaderTests
    {
        private const string ShaderPath = "CMiX.Engine/shaders/CMiXBlendOperations.sdsl";

        [Fact]
        public void BlendMode_HasTheSameNamesAndNumbersAsTheShaderSwitch()
        {
            var shaderCases = ReadShaderCases();
            var mismatches = new List<string>();

            foreach (var mode in Enum.GetValues<BlendMode>())
            {
                if (!shaderCases.TryGetValue(mode.ToString(), out var shaderIndex))
                    mismatches.Add($"{mode} has no case in the shader");
                else if ((int)mode != shaderIndex)
                    mismatches.Add($"{mode} is {(int)mode} in C# and {shaderIndex} in the shader");
            }

            foreach (var name in shaderCases.Keys)
                if (!Enum.IsDefined(typeof(BlendMode), name))
                    mismatches.Add($"{name} is in the shader and not in BlendMode");

            Assert.True(mismatches.Count == 0, string.Join(Environment.NewLine, mismatches));
        }

        private static Dictionary<string, int> ReadShaderCases()
        {
            var text = File.ReadAllText(FindShader());
            var matches = Regex.Matches(text, @"case\s+(\d+)\s*:\s*col\s*=\s*(\w+)\s*\(");
            Assert.NotEmpty(matches);
            return matches.ToDictionary(m => m.Groups[2].Value, m => int.Parse(m.Groups[1].Value));
        }

        private static string FindShader()
        {
            for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
            {
                var candidate = Path.Combine(dir.FullName, ShaderPath);
                if (File.Exists(candidate))
                    return candidate;
            }

            throw new FileNotFoundException($"Could not find {ShaderPath} above {AppContext.BaseDirectory}");
        }
    }
}
