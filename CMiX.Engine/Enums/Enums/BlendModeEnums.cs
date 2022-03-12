using System;

namespace Enums
{
    public static class BlendModeEnums
    {
        public static string StaticBlendModes(BlendModes e)
        {
            return e.ToString();
        }
    }

    [Flags]
    public enum BlendModes
    {
        Blend,
        Add,
        Subtract,
        Multiply,
        Divide,
    }
}
