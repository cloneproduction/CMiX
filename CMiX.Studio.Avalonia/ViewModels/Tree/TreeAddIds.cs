// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public static class TreeAddIds
    {
        private const string Prefix = "row.add-";

        public const string Composition = "row.add-composition";
        public const string Layer = "row.add-layer";
        public const string Entity = "row.add-entity";
        public const string FullscreenQuad = "row.add-fullscreen-quad";
        public const string TextEntity = "row.add-text-entity";
        public const string Camera = "row.add-camera";
        public const string Light = "row.add-light";

        public static string ForType(Type type) => Prefix + type.Name;

        public static string ForKindMenu(string kindName) => Prefix + kindName.ToLowerInvariant();
    }
}
