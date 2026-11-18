// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public static class TreeIcons
    {
        public const string CompositionKey = "Composition";
        public const string LayerKey = "LayerScene";
        public const string EntityKey = "Geometry";
        public const string FullscreenQuadKey = "FullscreenQuad";
        public const string TextEntityKey = "TextIcon";
        public const string CameraKey = "Camera";
        public const string LightKey = "MeshLight";
        public const string TextureKey = "Texture";
        public const string FilterKey = "Filter";
        public const string ModifierKey = "Modifier";
        public const string ModulatorKey = "Modulator";

        // A filter, a modifier or a texture has only the icon of its kind, so the Add panel shows no icon for it.
        public static string? OfType(Type type)
        {
            if (type == typeof(Composition)) return CompositionKey;
            if (type == typeof(Layer)) return LayerKey;
            if (type == typeof(Entity)) return EntityKey;
            if (type == typeof(FullscreenQuad)) return FullscreenQuadKey;
            if (type == typeof(TextEntity)) return TextEntityKey;
            if (type == typeof(Camera)) return CameraKey;
            if (type == typeof(LightEntity)) return LightKey;
            return null;
        }

        public static string OfItem(IControl? item) =>
            (item == null ? null : OfType(item.GetType())) ?? item switch
            {
                IModifier => ModifierKey,
                IModulator => ModulatorKey,
                ITextureFilter => FilterKey,
                ITextureSource => TextureKey,
                _ => EntityKey
            };

        public static string OfCategory(TreeCategory category) => category switch
        {
            TreeCategory.Composable => CompositionKey,
            TreeCategory.Light => LightKey,
            _ => category.ToString()
        };
    }
}
