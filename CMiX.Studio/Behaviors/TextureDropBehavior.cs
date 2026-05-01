using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Interactivity;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Studio.Behaviors
{
    public class TextureDropBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty PrefabManagerProperty =
            DependencyProperty.Register("PrefabManager", typeof(PrefabManager), typeof(TextureDropBehavior));

        public PrefabManager PrefabManager
        {
            get => (PrefabManager)GetValue(PrefabManagerProperty);
            set => SetValue(PrefabManagerProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AllowDrop = true;
            AssociatedObject.Drop += OnDrop;
            AssociatedObject.DragOver += OnDragOver;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Drop -= OnDrop;
            AssociatedObject.DragOver -= OnDragOver;
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            Debug.WriteLine($"OnDragOver IsFileDrop={e.Data.GetDataPresent(DataFormats.FileDrop)}");

            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
            e.Handled = true;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null || PrefabManager == null) return;

            foreach (var file in files)
            {
                var ext = Path.GetExtension(file).ToLower();
                Type textureType = ext switch
                {
                    ".png" or ".jpg" or ".jpeg" or ".bmp" => typeof(CMiX.Core.Texturing.Sources.Image),
                    ".mp4" or ".avi" or ".mov" => typeof(VideoPlayer),
                    _ => null
                };

                if (textureType == null) continue;

                PrefabManager.AddItem(textureType);

                if (PrefabManager.SelectedItem is IAssetTextureSource source)
                {
                    var existing = source.AssetSelector.AssetRepository.FindByPath(file);
                    if (existing != null)
                    {
                        source.AssetSelector.Asset = existing;
                    }
                    else
                    {
                        var asset = source.AssetSelector.CreateAssetFromPath(file);
                        if (asset != null)
                        {
                            source.AssetSelector.AssetRepository.Add(asset);
                            source.AssetSelector.Asset = asset;
                        }
                    }
                }
            }
        }
    }
}
