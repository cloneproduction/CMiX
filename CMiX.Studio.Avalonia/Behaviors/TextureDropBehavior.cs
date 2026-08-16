// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Xaml.Interactivity;
using CMiX.Core.Assets;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Studio.Avalonia.Behaviors
{
    public class TextureDropBehavior : Behavior<Control>
    {
        public static readonly StyledProperty<PrefabManager> PrefabManagerProperty =
            AvaloniaProperty.Register<TextureDropBehavior, PrefabManager>(nameof(PrefabManager));

        public PrefabManager PrefabManager
        {
            get => GetValue(PrefabManagerProperty);
            set => SetValue(PrefabManagerProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject == null)
                return;

            DragDrop.SetAllowDrop(AssociatedObject, true);
            AssociatedObject.AddHandler(DragDrop.DropEvent, OnDrop);
            AssociatedObject.AddHandler(DragDrop.DragOverEvent, OnDragOver);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject == null)
                return;

            AssociatedObject.RemoveHandler(DragDrop.DropEvent, OnDrop);
            AssociatedObject.RemoveHandler(DragDrop.DragOverEvent, OnDragOver);
        }

        private void OnDragOver(object? sender, DragEventArgs e)
        {
            e.DragEffects = e.Data.Contains(DataFormats.Files)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
            e.Handled = true;
        }

        private void OnDrop(object? sender, DragEventArgs e)
        {
            if (!e.Data.Contains(DataFormats.Files)) return;
            var files = e.Data.GetFiles()?.Select(f => f.TryGetLocalPath()).Where(p => p != null).ToArray();
            if (files == null || PrefabManager == null) return;

            foreach (var file in files)
            {
                if (!AssetTypes.TryResolve(file, out var kind)) continue;
                Type? textureType = kind switch
                {
                    AssetKind.Image => typeof(CMiX.Core.Texturing.Sources.Image),
                    AssetKind.Video => typeof(VideoPlayer),
                    _ => null
                };

                if (textureType == null) continue;
                PrefabManager.AddItemFromFilePath(textureType, file);
            }
        }
    }
}
