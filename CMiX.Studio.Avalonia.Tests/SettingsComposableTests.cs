// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Logging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.Boolean;
using CMiX.UI.Controls.Number;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The settings of a composition or of a layer are one view. The lists and the inspector show it.
    // The settings panels bind a Layer and a Composition by the same property names. A wrong name fails only at run time.
    public class SettingsComposableTests
    {
        private static (Window Window, SettingsComposable View, IControl Item) Show(Type itemType)
        {
            var provider = TestServiceProviderFactory.Create();
            var item = provider.GetRequiredService<ControlFactory>().Create(itemType);
            var view = new SettingsComposable { DataContext = item };
            var window = new Window { Content = view, Width = 400, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (window, view, item);
        }

        [AvaloniaTheory]
        [InlineData(typeof(Layer))]
        [InlineData(typeof(Composition))]
        public void Show_TheView_ShowsTheBlendAndTheMaskSettingsOnlyForAMask(Type itemType)
        {
            var (_, view, item) = Show(itemType);
            var blend = view.GetVisualDescendants().OfType<SettingsComposableCompositing>().Single();
            var mask = view.GetVisualDescendants().OfType<SettingsComposableMask>().Single();
            Assert.True(blend.IsEffectivelyVisible);
            Assert.False(mask.IsVisible);

            ((dynamic)item).Mask.IsMask.Value = true;
            Dispatcher.UIThread.RunJobs();

            Assert.True(mask.IsVisible);
        }

        [AvaloniaFact]
        public void Select_AnItemInTheLists_ShowsThisView()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.SelectMainTab(window, "Composition");
            TestServiceProviderFactory.Pump();

            var settings = window.GetVisualDescendants().OfType<SettingsComposable>().ToList();

            Assert.NotEmpty(settings);
            Assert.All(settings, view => Assert.Same(viewModel.Project.CompositionManager.SelectedItem, view.DataContext));
        }

        [AvaloniaTheory]
        [InlineData(typeof(Layer))]
        [InlineData(typeof(Composition))]
        public void Bind_ALayerAndAComposition_GivesNoErrorAndShowsTheModelValues(Type type)
        {
            var originalSink = Logger.Sink;
            var sink = new RecordingLogSink(originalSink);
            Logger.Sink = sink;

            try
            {
                var item = (IComposable)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(type);
                item.Compositing.Opacity.Value = 0.25f;
                item.Mask.Invert.Value = true;

                var panels = new StackPanel
                {
                    Children =
                    {
                        new Views.SettingsComposableCompositing { DataContext = item },
                        new Views.SettingsComposableMask { DataContext = item }
                    }
                };
                new Window { Content = panels, Width = 400, Height = 600 }.Show();
                for (var i = 0; i < 5; i++)
                    Dispatcher.UIThread.RunJobs();

                var errors = sink.Entries
                    .Where(e => e.Area == LogArea.Binding && e.FlattenedText.Contains("Could not find a matching property accessor"))
                    .Select(e => e.FlattenedText)
                    .Distinct()
                    .ToList();
                Assert.True(errors.Count == 0, string.Join(Environment.NewLine, errors));

                var alpha = panels.GetVisualDescendants().OfType<SliderCMiX>().Single(s => s.Caption == "Alpha");
                Assert.Equal(0.25, alpha.Value, 3);
                var invert = panels.GetVisualDescendants().OfType<ToggleButtonCaptioned>().Single(t => t.Caption == "Invert");
                Assert.True(invert.IsChecked);
            }
            finally
            {
                Logger.Sink = originalSink;
            }
        }
    }
}
