// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Transformation;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace CMiX.Studio.Avalonia.Tests
{
    // The add panel puts its buttons in one, two or three equal columns. The add menus do not offer a type with HiddenInMenu,
    // nor the types that do not work yet. A project can still hold them.
    public class PanelAddTypesTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        public class HiddenMenuOwner
        {
        }

        [ModifierPanel(typeof(HiddenMenuOwner))]
        public class VisibleMenuModifier
        {
        }

        [HiddenInMenu]
        [ModifierPanel(typeof(HiddenMenuOwner))]
        public class HiddenMenuModifier
        {
        }

        public interface IMenuItem
        {
        }

        public class VisibleMenuItem : IMenuItem
        {
        }

        [HiddenInMenu]
        public class HiddenMenuItem : IMenuItem
        {
        }

        public interface IThreeAddables
        {
        }

        public interface IFiveAddables
        {
        }

        public interface ITenAddables
        {
        }

        public class ThreeAddableA : IThreeAddables
        {
        }

        public class ThreeAddableB : IThreeAddables
        {
        }

        public class ThreeAddableC : IThreeAddables
        {
        }

        public class FiveAddableA : IFiveAddables
        {
        }

        public class FiveAddableB : IFiveAddables
        {
        }

        public class FiveAddableC : IFiveAddables
        {
        }

        public class FiveAddableD : IFiveAddables
        {
        }

        public class FiveAddableE : IFiveAddables
        {
        }

        public class TenAddableA : ITenAddables
        {
        }

        public class TenAddableB : ITenAddables
        {
        }

        public class TenAddableC : ITenAddables
        {
        }

        public class TenAddableD : ITenAddables
        {
        }

        public class TenAddableE : ITenAddables
        {
        }

        public class TenAddableF : ITenAddables
        {
        }

        public class TenAddableG : ITenAddables
        {
        }

        public class TenAddableH : ITenAddables
        {
        }

        public class TenAddableI : ITenAddables
        {
        }

        public class TenAddableJ : ITenAddables
        {
        }

        private readonly ITestOutputHelper output;

        public PanelAddTypesTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        private PanelAddTypes ShowInWindow(Type interfaceType)
        {
            var panel = new PanelAddTypes { Interface = interfaceType };
            windows.Add(new Window { Content = panel, Width = 400, Height = 400 }).Show();
            Dispatcher.UIThread.RunJobs();
            return panel;
        }

        private PanelAddTypes ShowInARow(Type interfaceType)
        {
            var panel = new PanelAddTypes { Interface = interfaceType };
            var row = new StackPanel { Orientation = Orientation.Horizontal, Children = { panel } };
            windows.Add(new Window { Content = row, Width = 400, Height = 400 }).Show();
            Dispatcher.UIThread.RunJobs();
            return panel;
        }

        private string BoundsOf(PanelAddTypes panel)
        {
            var bounds = panel.GetVisualDescendants().OfType<Button>()
                .Select(button => $"{button.Content}={new Rect(button.TranslatePoint(default, panel)!.Value, button.Bounds.Size)}");
            var text = $"{panel.Bounds.Size}|{string.Join("|", bounds)}";
            output.WriteLine(text);
            return text;
        }

        [AvaloniaFact]
        public void Show_ThreeTypesInAWindow_PutsTheButtonsInTheirColumnsAndRows()
        {
            var panel = ShowInWindow(typeof(IThreeAddables));

            Assert.Equal("400, 400|Three Addable A=4, 4, 392, 24|Three Addable B=4, 28, 392, 24|Three Addable C=4, 52, 392, 24", BoundsOf(panel));
        }

        [AvaloniaFact]
        public void Show_FiveTypesInAWindow_PutsTheButtonsInTheirColumnsAndRows()
        {
            var panel = ShowInWindow(typeof(IFiveAddables));

            Assert.Equal("400, 400|Five Addable A=4, 4, 196, 24|Five Addable B=4, 28, 196, 24|Five Addable C=4, 52, 196, 24|Five Addable D=200, 4, 196, 24|Five Addable E=200, 28, 196, 24", BoundsOf(panel));
        }

        [AvaloniaFact]
        public void Show_TenTypesInAWindow_PutsTheButtonsInTheirColumnsAndRows()
        {
            var panel = ShowInWindow(typeof(ITenAddables));

            Assert.Equal("400, 400|Ten Addable A=4, 4, 130, 24|Ten Addable B=4, 28, 130, 24|Ten Addable C=4, 52, 130, 24|Ten Addable D=4, 76, 130, 24|Ten Addable E=134, 4, 131, 24|Ten Addable F=134, 28, 131, 24|Ten Addable G=134, 52, 131, 24|Ten Addable H=134, 76, 131, 24|Ten Addable I=265, 4, 131, 24|Ten Addable J=265, 28, 131, 24", BoundsOf(panel));
        }

        [AvaloniaFact]
        public void Show_ThreeTypesInARow_PutsTheButtonsInTheirColumnsAndRows()
        {
            var panel = ShowInARow(typeof(IThreeAddables));

            Assert.Equal("106, 400|Three Addable A=4, 4, 98, 24|Three Addable B=4, 28, 98, 24|Three Addable C=4, 52, 98, 24", BoundsOf(panel));
        }

        [AvaloniaFact]
        public void Show_FiveTypesInARow_PutsTheButtonsInTheirColumnsAndRows()
        {
            var panel = ShowInARow(typeof(IFiveAddables));

            Assert.Equal("185, 400|Five Addable A=4, 4, 88, 24|Five Addable B=4, 28, 88, 24|Five Addable C=4, 52, 88, 24|Five Addable D=92, 4, 89, 24|Five Addable E=92, 28, 89, 24", BoundsOf(panel));
        }

        [AvaloniaFact]
        public void Show_TenTypesInARow_PutsTheButtonsInTheirColumnsAndRows()
        {
            var panel = ShowInARow(typeof(ITenAddables));

            Assert.Equal("263, 400|Ten Addable A=4, 4, 87, 24|Ten Addable B=4, 28, 87, 24|Ten Addable C=4, 52, 87, 24|Ten Addable D=4, 76, 87, 24|Ten Addable E=91, 4, 86, 24|Ten Addable F=91, 28, 86, 24|Ten Addable G=91, 52, 86, 24|Ten Addable H=91, 76, 86, 24|Ten Addable I=177, 4, 82, 24|Ten Addable J=177, 28, 82, 24", BoundsOf(panel));
        }

        private static List<object?> Offered(PanelAddTypes panel) =>
            ((Grid)panel.Content!).Children
                .OfType<StackPanel>()
                .SelectMany(stack => stack.Children.OfType<Button>())
                .Select(button => button.CommandParameter)
                .ToList();

        [AvaloniaFact]
        public void Offer_ATypeOfThePanelOwner_OffersIt()
        {
            var panel = new PanelAddTypes { PanelOwner = typeof(HiddenMenuOwner) };

            Assert.Contains(typeof(VisibleMenuModifier), Offered(panel));
        }

        [AvaloniaFact]
        public void Offer_AHiddenTypeOfThePanelOwner_LeavesItOut()
        {
            var panel = new PanelAddTypes { PanelOwner = typeof(HiddenMenuOwner) };

            Assert.DoesNotContain(typeof(HiddenMenuModifier), Offered(panel));
        }

        [AvaloniaFact]
        public void Offer_AHiddenTypeOfTheInterface_LeavesItOut()
        {
            var panel = new PanelAddTypes { Interface = typeof(IMenuItem) };

            Assert.Contains(typeof(VisibleMenuItem), Offered(panel));
            Assert.DoesNotContain(typeof(HiddenMenuItem), Offered(panel));
        }

        [AvaloniaFact]
        public void Offer_TheLayerMenu_LeavesOutTheRenderModifiers()
        {
            var offered = Offered(new PanelAddTypes { PanelOwner = typeof(Layer) });

            Assert.DoesNotContain(typeof(RenderRandomEntityModifier), offered);
            Assert.DoesNotContain(typeof(RenderSequenceEntityModifier), offered);
            Assert.Contains(typeof(AlphaModifier), offered);
        }

        [AvaloniaFact]
        public void Offer_TheEntityMenu_LeavesOutBillboard()
        {
            var offered = Offered(new PanelAddTypes { PanelOwner = typeof(Entity) });

            Assert.DoesNotContain(typeof(BillboardModifier), offered);
            Assert.Contains(typeof(TranslateModifier), offered);
        }

        [AvaloniaFact]
        public void Offer_TheModulatorMenu_HoldsTheRedisModulators()
        {
            var offered = Offered(new PanelAddTypes { Interface = typeof(IModulator) });

            Assert.Contains(typeof(RedisFloatModulator), offered);
            Assert.Contains(typeof(RedisBoolModulator), offered);
            Assert.Contains(typeof(RedisIntModulator), offered);
            Assert.Contains(typeof(RandomModulator), offered);
        }

        [Theory]
        [InlineData(typeof(RenderRandomEntityModifier))]
        [InlineData(typeof(RenderSequenceEntityModifier))]
        [InlineData(typeof(BillboardModifier))]
        public void Create_AHiddenType_GivesAnItemOfThatType(Type type)
        {
            var provider = TestServiceProviderFactory.Create();

            var control = provider.GetRequiredService<ControlFactory>().Create(type);

            Assert.IsType(type, control);
        }
    }
}
