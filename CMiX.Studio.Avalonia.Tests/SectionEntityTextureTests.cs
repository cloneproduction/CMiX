// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Panels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class SectionEntityTextureTests
    {
        private static SectionEntityTexture SectionOf(Control view)
        {
            new Window { Content = view, Width = 400, Height = 1200 }.Show();
            Dispatcher.UIThread.RunJobs();
            return view.GetVisualDescendants().OfType<SectionEntityTexture>().Single();
        }

        private static List<ExpanderIndented> TopExpandersOf(SectionEntityTexture section) =>
            section.GetVisualDescendants().OfType<ExpanderIndented>()
                .Where(expander => !expander.GetVisualAncestors().TakeWhile(ancestor => ancestor != section).OfType<ExpanderIndented>().Any())
                .ToList();

        private static bool IsMask(ExpanderIndented expander) =>
            expander.Header is Panel header && header.Children.OfType<TextBlock>().Any(text => text.Text == "Mask");

        [AvaloniaFact]
        public void Template_FullscreenQuadAndModelEntity_ShowTheSameExpanders()
        {
            var factory = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();
            var views = new Control[]
            {
                new Views.FullscreenQuad { DataContext = factory.Create(typeof(CMiX.Core.Compositing.FullscreenQuad)) },
                new ModelEntity { DataContext = factory.Create(typeof(CMiX.Core.Compositing.Entity)) },
            };

            var sections = views.Select(SectionOf).ToList();

            Assert.All(sections, section =>
            {
                var expanders = TopExpandersOf(section);
                expanders[0].IsExpanded = true;
                Dispatcher.UIThread.RunJobs();
                Assert.Equal(new object[] { "Texture", "Transform Texture" }, expanders.Select(expander => expander.Header));
                Assert.Single(expanders[0].GetVisualDescendants().OfType<ExpanderIndented>(), IsMask);
            });
        }
    }
}
